using M2BobPatcher.Hash;
using M2BobPatcher.Resources.Configs;
using M2BobPatcher.Resources.TextResources;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace M2BobPatcher.Downloaders {

    class HttpClientDownloader : IDownloader {

        private static readonly HttpClient HttpClient = new HttpClient();
        private static Action<int, bool> ProgressFunction;

        public HttpClientDownloader(Action<int, bool> progressFunction) {
            ProgressFunction = progressFunction;
        }

        private async Task<byte[]> Download(string address, string expectedHash) {
            using (HttpResponseMessage response = HttpClient.GetAsync(address, HttpCompletionOption.ResponseHeadersRead).Result) {
                response.EnsureSuccessStatusCode();
                float fileSize = (float)response.Content.Headers.ContentLength;
                using (Stream contentStream = await response.Content.ReadAsStreamAsync()) {
                    using (MemoryStream ms = new MemoryStream()) {
                        var totalRead = 0L;
                        var totalReads = 0L;
                        var buffer = new byte[8192];
                        var isMoreToRead = true;
                        var lastMark = 0;
                        ProgressFunction(0, true);
                        do {
                            var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                            if (read < 8192) {
                                isMoreToRead = false;
                            } else {
                                await ms.WriteAsync(buffer, 0, read);

                                totalRead += read;
                                totalReads += 1;
                                if (totalReads % 10 == 0 && totalRead / fileSize * 100 > lastMark) {
                                    lastMark = Convert.ToInt32(totalRead / fileSize * 100);
                                    ProgressFunction(lastMark, true);
                                }
                            }
                        }
                        while (isMoreToRead);
                        byte[] result = ms.ToArray();
                        if (expectedHash != null && !Md5HashFactory.NormalizeMd5(Md5HashFactory.GeneratedMd5HashFromByteArray(result)).Equals(expectedHash))
                            throw new InvalidDataException();
                        ProgressFunction(100, true);
                        return result;
                    }
                }
            }
        }

        public byte[] DownloadData(string address, string expectedHash) {
            int tries = 0;
            while (true) {
                try {
                    Task<byte[]> data = Download(address, expectedHash);
                    data.Wait();
                    return data.Result;
                } catch (Exception ex) {
                    if (ex is HttpRequestException || ex is AggregateException || ex is InvalidDataException) {
                        Thread.Sleep(DownloaderConfigs.INTERVAL_MS_BETWEEN_DOWNLOAD_RETRIES);
                        if (++tries == DownloaderConfigs.MAX_DOWNLOAD_RETRIES_PER_FILE)
                            throw;
                        continue;
                    }
                    throw;
                }
            }
        }
    }
}
