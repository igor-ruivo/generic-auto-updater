using M2BobPatcher.Resources.Configs;
using M2BobPatcher.Resources.TextResources;
using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace M2BobPatcher.Downloaders {

    class WebClientDownloader : IDownloader {

        private static BackgroundWorker BW;

        public WebClientDownloader(BackgroundWorker bw) {
            BW = bw;
        }

        public byte[] DownloadData(string address, string expectedHash) {
            Task<byte[]> data = null;
            using (WebClient client = new WebClient()) {
                client.DownloadProgressChanged += (s, e) => {
                    BW.ReportProgress(e.ProgressPercentage, true);
                };
                int tries = 0;
                while(true) {
                    try {
                        data = client.DownloadDataTaskAsync(new Uri(address));
                        data.Wait();
                        return data.Result;
                    }
                    catch (Exception ex) {
                        if (ex is WebException || ex is AggregateException) {
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
}
