using GenericAutoUpdater.Hash;
using GenericAutoUpdater.Resources;
using GenericAutoUpdater.Resources.Configs;
using GenericAutoUpdater.Resources.TextResources;
using GenericAutoUpdater.UI;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GenericAutoUpdater.Downloaders {
    /// <summary>
    /// An error-handler and user-friendly downloader class, used to download any file needed by the Auto-Updater through http.
    /// </summary>
    public class HttpClientDownloader : IDownloader {
        /// <summary>
        /// The downloader client itself.
        /// </summary>
        private static HttpClient HttpClient;

        /// <summary>
        /// The <c>IHasher</c> used to compute the hash of every downloaded file, if there is an expectedHash to compare it to.
        /// </summary>
        private static IHasher Hasher;

        /// <summary>
        /// The <c>BackgroundWorker</c> used to inform the UI Thread of the current download's progress.
        /// </summary>
        private static BackgroundWorker BW;

        /// <summary>
        /// Initializes a new instance of the <c>HttpClientDownloader</c> class with the specified <c>BackgroundWorker</c> and with the specified <c>IHasher</c>.
        /// </summary>
        public HttpClientDownloader(BackgroundWorker bw, IHasher hasher) {
            BW = bw;
            Hasher = hasher;
            HttpClient = new HttpClient(new HttpClientHandler() {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
            SetupDefaultHeaders();
        }

        /// <summary>
        /// This method tries to download a specific file (<c>address</c>) a pre-defined number of times (<c>DownloaderConfigs.MAX_DOWNLOAD_RETRIES_PER_FILE</c>).
        /// If an expectedHash is used in this call, the hash of the downloaded file is compared to the expected hash received, triggering an InvalidDataException if they differ.
        /// It returns the downloaded content as a <c>byte[]</c>.
        /// </summary>
        public byte[] DownloadDataToMemory(string address, string expectedHash = null) {
            return DownloadData(address, expectedHash);
        }

        /// <summary>
        /// This method tries to download a specific file (<c>address</c>) a pre-defined number of times (<c>DownloaderConfigs.MAX_DOWNLOAD_RETRIES_PER_FILE</c>).
        /// If an expectedHash is used in this call, the hash of the downloaded file is compared to the expected hash received, triggering an InvalidDataException if they differ.
        /// The downloaded content is saved in the disk, in the specified directory (filePath).
        /// </summary>
        public void DownloadDataToFile(string address, string filePath, string expectedHash = null) {
            DownloadData(address, expectedHash, filePath);
        }

        /// <summary>
        /// This method tries to download a specific file (<c>address</c>) a pre-defined number of times (<c>DownloaderConfigs.MAX_DOWNLOAD_RETRIES_PER_FILE</c>).
        /// It is also responsible for putting the BackgroundWorker Thread to sleep a pre-defined amount of time (<c>DownloaderConfigs.INTERVAL_MS_BETWEEN_DOWNLOAD_RETRIES</c>) between each download retry.
        /// It uses the Download method for that purpose, calling it with the received arguments.
        /// If no file path was received in argument (i.e., string.Empty), it waits for the returned Download's <c>Task</c> to finish before returning the downloaded content as an in-memory <c>byte[]</c>.
        /// If there is a file path specified (i.e., different from string.Empty), this method returns null after the Download is finished.
        /// </summary>
        private static byte[] DownloadData(string address, string expectedHash, string filePath = null) {
            int tries = 0;
            while (true) {
                try {
                    Task<byte[]> data = Download(address, expectedHash, filePath);
                    if (data == null)
                        return null;
                    data.Wait();
                    return data.Result;
                }
                catch (Exception ex) {
                    if (!ExceptionTypeShouldRetry(ex))
                        throw;
                    Thread.Sleep(ComputeNextSleepTime(DownloaderConfigs.BASE_MS_SLEEP_TIME_BETWEEN_DOWNLOAD_RETRIES, tries));
                    if (++tries == DownloaderConfigs.MAX_DOWNLOAD_RETRIES_PER_FILE)
                        throw;
                    continue;
                }
            }
        }

        /// <summary>
        /// This method tries to download a specific file from a specific url (<c>address</c>).
        /// If no file path was received (i.e., file parameter is null), this method returns an in-memory byte[] with the downloaded file.
        /// If there is a file path in the parameter path, the download is performed directly into a file with the same name and relative path as the one in the address and this method returns null.
        /// If it completes the download and an expectedHash was received (i.e., the respective parameter isn't null), it checks if the hash of the downloaded file equals the expected hash of that same file (expectedHash), throwing an <c>InvalidDataException</c> if it doesn't.
        /// This method also logs the download progress to the respective progress bar through the BackgroundWorker (bw), whenever it assumes it is necessary.
        /// </summary>
        private static async Task<byte[]> Download(string address, string expectedHash, string file) {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Utils.Progress(BW, 0, ProgressiveWidgetsEnum.ProgressBar.DownloadProgressBar);
            using (HttpResponseMessage response = HttpClient.GetAsync(address, HttpCompletionOption.ResponseHeadersRead).Result) {
                if (!response.IsSuccessStatusCode)
                    HandleStatusCode(response);
                response.EnsureSuccessStatusCode();
                long fileSize = response.Content.Headers.ContentLength.GetValueOrDefault();
                using (Stream contentStream = await response.Content.ReadAsStreamAsync()) {
                    long totalRead = 0;
                    long totalReads = 0;
                    byte[] buffer = new byte[DownloaderConfigs.BUFFER_SIZE];
                    bool moreLeftToRead = true;
                    int lastMark = 0;
                    float speedAverage = 0;
                    using (MemoryStream memoryStream = file != null ? null : new MemoryStream()) {
                        using (FileStream fileStream = file == null ? null : File.Open(file, FileMode.Create)) {
                            do {
                                // A CancellationTokenSource is used to close the contentStream by force if it doesn't finish some ReadAsync()
                                // under a specific amount of time (DownloaderConfigs.TIMEOUT_MS_WAITING_FOR_READ),
                                // throwing an ObjectDisposedException or an AggregateException containing it, if that's the case.
                                using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(DownloaderConfigs.TIMEOUT_MS_WAITING_FOR_READ))) {
                                    cts.Token.Register(() => contentStream.Close());
                                    int read = await contentStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                                    if (read == 0)
                                        moreLeftToRead = false;
                                    else {
                                        if (fileStream == null)
                                            await memoryStream.WriteAsync(buffer, 0, read);
                                        else
                                            await fileStream.WriteAsync(buffer, 0, read);
                                        totalRead += read;
                                        totalReads++;
                                        // It only attempts to log the download progress every x reads (DownloaderConfigs.INFORM_PROGRESS_EVERY_X_READS), due to performance reasons.
                                        if (fileSize != 0 && totalReads % DownloaderConfigs.INFORM_PROGRESS_EVERY_X_READS == 0) {
                                            speedAverage = RecalculateSpeedAverage((float)totalRead / sw.ElapsedMilliseconds, speedAverage);
                                            Utils.Log(BW, string.Format(DownloaderResources.DOWNLOAD_DATA, Utils.BytesToString(totalRead, 2), Utils.BytesToString(fileSize, 2), Utils.BytesToString(Convert.ToInt64(speedAverage * 1000), 1)), ProgressiveWidgetsEnum.Label.DownloadSpeedLogger);
                                            if ((float)totalRead / fileSize * 100 > lastMark) {
                                                lastMark = Convert.ToInt32((float)totalRead / fileSize * 100);
                                                Utils.Progress(BW, lastMark, ProgressiveWidgetsEnum.ProgressBar.DownloadProgressBar);
                                            }
                                        }
                                    }
                                }
                            }
                            while (moreLeftToRead);
                        }
                        sw.Stop();
                        if (fileSize != 0 && totalRead != fileSize || expectedHash != null && !Hasher.GeneratedHashFromFile(file).Equals(expectedHash))
                            throw new InvalidDataException();
                        Utils.Progress(BW, 100, ProgressiveWidgetsEnum.ProgressBar.DownloadProgressBar);
                        return file == null ? memoryStream.ToArray() : null;
                    }
                }
            }
        }

        /// <summary>
        /// Recalculates the average speed of the current download based on the new speed sample just measured.
        /// It weights the new speed sample using a previously defined ratio (DownloaderConfigs.SAMPLE_SPEED_WEIGHT).
        /// </summary>
        private static float RecalculateSpeedAverage(float sample, float speedAverage) {
            return speedAverage == 0 ? sample :
                sample * DownloaderConfigs.SAMPLE_SPEED_WEIGHT + speedAverage * (1 - DownloaderConfigs.SAMPLE_SPEED_WEIGHT);
        }

        /// <summary>
        /// Handles the received status code of the HTTP request performed. This method is only called when the status code isn't a Success Status code.
        /// When the status code is a Server Error, the download flow is continued so that a retry is done, if the limit of retries hasn't yet been reached.
        /// When the status code isn't a Server Error, there is no point in retrying the Download, thus a <c>HttpRequestException</c> is trown - denied of a retry.
        /// </summary>
        private static void HandleStatusCode(HttpResponseMessage response) {
            HttpStatusCode code = response.StatusCode;
            if (!response.IsSuccessStatusCode)
                switch ((int)code) {
                    case int c when c >= 500:
                        break;
                    default:
                        throw new HttpRequestException();
                }
        }

        /// <summary>
        /// Sets up the default headers of the HttpClient to be used when performing HTTP operations.
        /// </summary>
        private static void SetupDefaultHeaders() {
            HttpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml");
            HttpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            HttpClient.DefaultRequestHeaders.Add("Accept-Charset", "ISO-8859-1");
        }

        /// <summary>
        /// This method returns true if the received exception is eligible for a retry, or false otherwise.
        /// </summary>
        private static bool ExceptionTypeShouldRetry(Exception ex) {
            // When a HTTP status code in the range of [300 - 499] is received there is no point in retrying.
            // When the Download's contentStream is closed by force due to a read timeout there is no point in retrying.
            // There is no point in retrying IOExceptions.
            return !(ex is AggregateException exception && (Utils.AggregateContainsSpecificException(exception, new ObjectDisposedException("")) || Utils.AggregateContainsSpecificException(exception, new HttpRequestException()) || Utils.AggregateContainsSpecificException(exception, new IOException()))
                        || ex is HttpRequestException || ex is ObjectDisposedException || ex is IOException);
        }

        /// <summary>
        /// This method calculates the next sleep time by multiplying the base sleep time (DownloaderConfigs.BASE_MS_SLEEP_TIME_BETWEEN_DOWNLOAD_RETRIES) by a random floating-point number in the interval [0, 1[.
        /// It then applies an exponential backoff strategy to the wait time.
        /// </summary>
        private static int ComputeNextSleepTime(int baseTimer, int tries) {
            return Convert.ToInt32(Math.Pow(2, tries) * baseTimer * (1 + new Random().NextDouble()));
        }
    }
}