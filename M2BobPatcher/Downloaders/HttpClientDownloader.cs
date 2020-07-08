using M2BobPatcher.Hash;
using M2BobPatcher.Resources;
using M2BobPatcher.Resources.Configs;
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
                    if (ex is ObjectDisposedException || ex is AggregateException && Utils.AggregateContainsObjectDisposedException((AggregateException)ex))
                        throw;
                    Thread.Sleep(DownloaderConfigs.INTERVAL_MS_BETWEEN_DOWNLOAD_RETRIES);
                    if (++tries == DownloaderConfigs.MAX_DOWNLOAD_RETRIES_PER_FILE)
                        throw;
                    continue;
                }
            }
        }

        private static async Task<byte[]> Download(BackgroundWorker bw, string address, string expectedHash) {
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
                        bw.ReportProgress(0, true);
                        do {
                            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(DownloaderConfigs.TIMEOUT_MS_WAITING_FOR_READ))) {
                                cts.Token.Register(() => contentStream.Close());
                                int read = await contentStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                                if (read == 0)
                                    moreLeftToRead = false;
                                else {
                                    await ms.WriteAsync(buffer, 0, read);
                                    totalRead += read;
                                    totalReads += 1;
                                    if (totalReads % DownloaderConfigs.INFORM_PROGRESS_ONE_IN_X_READS == 0 && totalRead / fileSize * 100 > lastMark) {
                                        lastMark = Convert.ToInt32(totalRead / fileSize * 100);
                                        bw.ReportProgress(lastMark, true);
                                    }
                                }
                            }
                        }
                        while (moreLeftToRead);
                        byte[] result = ms.ToArray();
                        if (expectedHash == null)
                            Utils.PerformPatchDirectorySanityCheck(result);
                        else
                            if (!Md5HashFactory.NormalizeMd5(Md5HashFactory.GeneratedMd5HashFromByteArray(result)).Equals(expectedHash))
                            throw new InvalidDataException();
                        bw.ReportProgress(100, true);
                        return result;
                    }
                }
            }
        }
    }
}