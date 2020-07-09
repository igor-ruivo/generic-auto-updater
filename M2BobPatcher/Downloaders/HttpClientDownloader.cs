using M2BobPatcher.Hash;
using M2BobPatcher.Resources;
using M2BobPatcher.Resources.Configs;
using M2BobPatcher.UI;
using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace M2BobPatcher.Downloaders {

    static class HttpClientDownloader {

        private static readonly HttpClient HttpClient = new HttpClient();

        public static byte[] DownloadData(BackgroundWorker bw, string address, string expectedHash) {
            int tries = 0;
            while (true) {
                try {
                    Task<byte[]> data = Download(bw, address, expectedHash);
                    data.Wait();
                    return data.Result;
                }
                catch (Exception ex) {
                    // This kind of Exception already slept.
                    if (ex is ObjectDisposedException || ex is AggregateException exception && Utils.AggregateContainsObjectDisposedException(exception))
                        throw;
                    Thread.Sleep(DownloaderConfigs.INTERVAL_MS_BETWEEN_DOWNLOAD_RETRIES);
                    if (++tries == DownloaderConfigs.MAX_DOWNLOAD_RETRIES_PER_FILE)
                        throw;
                    continue;
                }
            }
        }

        private static async Task<byte[]> Download(BackgroundWorker bw, string address, string expectedHash) {
            Utils.Progress(bw, 0, ProgressiveWidgetsEnum.ProgressBar.DownloadProgressBar);
            using (HttpResponseMessage response = HttpClient.GetAsync(address, HttpCompletionOption.ResponseHeadersRead).Result) {
                response.EnsureSuccessStatusCode();
                float fileSize = (float)response.Content.Headers.ContentLength;
                using (Stream contentStream = await response.Content.ReadAsStreamAsync()) {
                    using (MemoryStream ms = new MemoryStream()) {
                        long totalRead = 0;
                        long totalReads = 0;
                        byte[] buffer = new byte[DownloaderConfigs.BUFFER_SIZE];
                        bool moreLeftToRead = true;
                        int lastMark = 0;
                        do {
                            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(DownloaderConfigs.TIMEOUT_MS_WAITING_FOR_READ))) {
                                cts.Token.Register(() => contentStream.Close());
                                int read = await contentStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                                if (read == 0)
                                    moreLeftToRead = false;
                                else {
                                    await ms.WriteAsync(buffer, 0, read);
                                    totalRead += read;
                                    totalReads++;
                                    if (totalReads % DownloaderConfigs.INFORM_PROGRESS_EVERY_X_READS == 0 && totalRead / fileSize * 100 > lastMark) {
                                        lastMark = Convert.ToInt32(totalRead / fileSize * 100);
                                        Utils.Progress(bw, lastMark, ProgressiveWidgetsEnum.ProgressBar.DownloadProgressBar);
                                    }
                                }
                            }
                        }
                        while (moreLeftToRead);
                        byte[] result = ms.ToArray();
                        if (expectedHash == null)
                            Utils.PerformPatchDirectorySanityCheck(result);
                        else
                            if (!Md5HashFactory.GeneratedMd5HashFromByteArray(result).Equals(expectedHash))
                            throw new InvalidDataException();
                        Utils.Progress(bw, 100, ProgressiveWidgetsEnum.ProgressBar.DownloadProgressBar);
                        return result;
                    }
                }
            }
        }
    }
}