using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GenericAutoUpdater.Hash;
using GenericAutoUpdater.Resources;
using GenericAutoUpdater.Resources.Configs;
using GenericAutoUpdater.Resources.TextResources;
using GenericAutoUpdater.UI;

namespace GenericAutoUpdater.Downloaders {
    /// <summary>
    /// An error-handler and user-friendly downloader class, used to download any file needed by the Auto-Updater through http.
    /// </summary>
    static class HttpClientDownloader {
        /// <summary>
        /// The downloader client itself.
        /// </summary>
        private static readonly HttpClient HttpClient = new HttpClient();

        /// <summary>
        /// This method tries to download a specific file (<c>address</c>) a pre-defined number of times (<c>DownloaderConfigs.MAX_DOWNLOAD_RETRIES_PER_FILE</c>).
        /// It is also responsible for putting the BackgroundWorker Thread to sleep a pre-defined amount of time (<c>DownloaderConfigs.INTERVAL_MS_BETWEEN_DOWNLOAD_RETRIES</c>) between each download retry.
        /// It uses the Download method for that purpose, calling it with the received arguments.
        /// If no file path was received in argument (i.e., string.Empty), it waits for the returned Download's <c>Task</c> to finish before returning the downloaded content as an in-memory <c>byte[]</c>.
        /// If there is a file path specified (i.e., different from string.Empty), this method returns null after the Download is finished.
        /// </summary>
        public static byte[] DownloadData(BackgroundWorker bw, string address, string expectedHash, string file) {
            int tries = 0;
            while (true) {
                try {
                    Task<byte[]> data = Download(bw, address, expectedHash, file);
                    if (data == null)
                        return null;
                    data.Wait();
                    return data.Result;
                } catch (Exception ex) {
                    // This kind of Exception is thrown when the Download's contentStream is closed by force due to a read timeout. No need to sleep in this case.
                    if (ex is ObjectDisposedException || ex is AggregateException exception && Utils.AggregateContainsObjectDisposedException(exception))
                        throw;
                    Thread.Sleep(DownloaderConfigs.INTERVAL_MS_BETWEEN_DOWNLOAD_RETRIES);
                    if (++tries == DownloaderConfigs.MAX_DOWNLOAD_RETRIES_PER_FILE)
                        throw;
                    continue;
                }
            }
        }

        /// <summary>
        /// This method tries to download a specific file from a specific url (<c>address</c>).
        /// If no file path was received (i.e., file parameter equals empty string), this method returns an in-memory byte[] with the downloaded file.
        /// If there is a file path in the parameter path, the download is performed directly into a file with the same name and relative path as the one in the address and this method returns null.
        /// If it completes the download, it checks if the hash of the downloaded file equals the expected hash of that same file (expectedHash), throwing an <c>InvalidDataException</c> if it doesn't.
        /// If the expected hash is null, it assumes that the downloaded file is the server metadata and executes the respective sanity check on it, throwing an <c>InvalidDataException</c> if something is wrong with it.
        /// This method also logs the download progress to the respective progress bar through the BackgroundWorker (bw), whenever it assumes it is necessary.
        /// </summary>
        private static async Task<byte[]> Download(BackgroundWorker bw, string address, string expectedHash, string file) {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Utils.Progress(bw, 0, ProgressiveWidgetsEnum.ProgressBar.DownloadProgressBar);
            using (HttpResponseMessage response = HttpClient.GetAsync(address, HttpCompletionOption.ResponseHeadersRead).Result) {
                response.EnsureSuccessStatusCode();
                long fileSize = (long)response.Content.Headers.ContentLength;
                using (Stream contentStream = await response.Content.ReadAsStreamAsync()) {
                    using (MemoryStream ms = new MemoryStream()) {
                        long totalRead = 0;
                        long totalReads = 0;
                        byte[] buffer = new byte[DownloaderConfigs.BUFFER_SIZE];
                        bool moreLeftToRead = true;
                        int lastMark = 0;
                        float speedAverage = 0;
                        using (FileStream fileStream = file.Equals(string.Empty) ? null : File.OpenWrite(file)) {
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
                                            await ms.WriteAsync(buffer, 0, read);
                                        else
                                            await fileStream.WriteAsync(buffer, 0, read);
                                        totalRead += read;
                                        totalReads++;
                                        // It only attempts to log the download progress every x reads (DownloaderConfigs.INFORM_PROGRESS_EVERY_X_READS), due to performance reasons.
                                        if (totalReads % DownloaderConfigs.INFORM_PROGRESS_EVERY_X_READS == 0) {
                                            speedAverage = RecalculateSpeedAverage((float)totalRead / sw.ElapsedMilliseconds, speedAverage);
                                            Utils.Log(bw, string.Format(DownloaderResources.DOWNLOAD_DATA, Utils.BytesToString(totalRead, 2), Utils.BytesToString(fileSize, 2), Utils.BytesToString(Convert.ToInt64(speedAverage * 1000), 1)), ProgressiveWidgetsEnum.Label.DownloadSpeedLogger);
                                            if ((float)totalRead / fileSize * 100 > lastMark) {
                                                lastMark = Convert.ToInt32((float)totalRead / fileSize * 100);
                                                Utils.Progress(bw, lastMark, ProgressiveWidgetsEnum.ProgressBar.DownloadProgressBar);
                                            }
                                        }
                                    }
                                }
                            }
                            while (moreLeftToRead);
                        }
                        sw.Stop();
                        byte[] result = ms.ToArray();
                        if (expectedHash == null)
                            Utils.PerformPatchDirectorySanityCheck(result);
                        else
                            if (!Md5HashFactory.GeneratedMd5HashFromFile(file).Equals(expectedHash))
                            throw new InvalidDataException();
                        Utils.Progress(bw, 100, ProgressiveWidgetsEnum.ProgressBar.DownloadProgressBar);
                        return file.Equals(string.Empty) ? result : null;
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
    }
}