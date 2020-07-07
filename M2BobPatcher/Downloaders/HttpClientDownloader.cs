using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using M2BobPatcher.Hash;
using M2BobPatcher.Resources.Configs;

namespace M2BobPatcher.Downloaders {

    class HttpClientDownloader : IDownloader {

        private static readonly HttpClient HttpClient = new HttpClient();
        private static BackgroundWorker BW;

        public HttpClientDownloader(BackgroundWorker bw) {
            BW = bw;
        }

        private async Task<byte[]> Download(string address, string expectedHash) {
            using (HttpResponseMessage response = HttpClient.GetAsync(address, HttpCompletionOption.ResponseHeadersRead).Result) {
                response.EnsureSuccessStatusCode();
                float fileSize = (float)response.Content.Headers.ContentLength;
                using (Stream contentStream = await response.Content.ReadAsStreamAsync()) {
                    using (MemoryStream ms = new MemoryStream()) {
                        var totalRead = 0L;
                        var totalReads = 0L;
                        var buffer = new byte[DownloaderConfigs.BUFFER_SIZE];
                        var isMoreToRead = true;
                        var lastMark = 0;
                        BW.ReportProgress(0, true);
                        do {
                            Console.WriteLine("Oportunity");
                            var read = contentStream.ReadAsync(buffer, 0, buffer.Length);
                            var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(DownloaderConfigs.TIMEOUT_MS_WAITING_FOR_READ));
                            await Task.WhenAny(read, timeoutTask);
                            if (!read.IsCompleted)
                                throw new TimeoutException();
                            if (read.Result == 0) {
                                isMoreToRead = false;
                            } else {
                                await ms.WriteAsync(buffer, 0, read.Result);

                                totalRead += read.Result;
                                totalReads += 1;
                                if (totalReads % DownloaderConfigs.INFORM_PROGRESS_ONE_IN_X_READS == 0 && totalRead / fileSize * 100 > lastMark) {
                                    lastMark = Convert.ToInt32(totalRead / fileSize * 100);
                                    BW.ReportProgress(lastMark, true);
                                }
                            }
                        }
                        while (isMoreToRead);
                        byte[] result = ms.ToArray();
                        if (expectedHash == null) {
                            // check result sanity
                            // assume it's the server's metadata file.
                            Uri uriResult;
                            string serverMetadata = System.Text.Encoding.Default.GetString(result);
                            string patchDirectory = serverMetadata.Trim().Split(new[] { "\n" }, StringSplitOptions.None)[0];
                            if (!Uri.TryCreate(patchDirectory, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                                throw new InvalidDataException();
                        } else
                            if (!Md5HashFactory.NormalizeMd5(Md5HashFactory.GeneratedMd5HashFromByteArray(result)).Equals(expectedHash))
                            throw new InvalidDataException();
                        BW.ReportProgress(100, true);
                        return result;
                    }
                }
            }
        }

        private bool AggregateContainsTimeout(AggregateException ex) {
            foreach (Exception exception in ex.InnerExceptions)
                if (exception is TimeoutException)
                    return true;
            return false;
        }

        public byte[] DownloadData(string address, string expectedHash) {
            int tries = 0;
            while (true) {
                try {
                    Task<byte[]> data = Download(address, expectedHash);
                    data.Wait();
                    return data.Result;
                } catch (Exception ex) {
                    if(!(ex is TimeoutException || ex is AggregateException && AggregateContainsTimeout((AggregateException)ex)))
                        Thread.Sleep(DownloaderConfigs.INTERVAL_MS_BETWEEN_DOWNLOAD_RETRIES);
                    if (++tries == DownloaderConfigs.MAX_DOWNLOAD_RETRIES_PER_FILE)
                        throw;
                    continue;
                }
            }
        }
    }
}
