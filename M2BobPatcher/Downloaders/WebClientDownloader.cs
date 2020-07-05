using M2BobPatcher.Resources.Configs;
using M2BobPatcher.Resources.TextResources;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace M2BobPatcher.Downloaders {
    class WebClientDownloader {

        public static async Task<byte[]> DownloadData(string address, Action<int, bool> progressFunction) {
            byte[] data = null;
            using (WebClient client = new WebClient()) {
                client.DownloadProgressChanged += (s, e) => {
                    progressFunction(e.ProgressPercentage, true);
                };
                int tries = 0;
                for (; tries < DownloaderConfigs.MAX_DOWNLOAD_RETRIES_PER_FILE; tries++) {
                    try {
                        data = await client.DownloadDataTaskAsync(new Uri(address));
                    }
                    catch (WebException) {
                        Thread.Sleep(DownloaderConfigs.INTERVAL_MS_BETWEEN_DOWNLOAD_RETRIES);
                        continue;
                    }
                    catch (Exception e) {
                        Console.WriteLine(DownloaderResources.ERROR_WHILE_DOWNLOADING_FILE, address);
                        Console.WriteLine(e.ToString());
                    }
                    break;
                }
                if (tries == DownloaderConfigs.MAX_DOWNLOAD_RETRIES_PER_FILE)
                    Console.WriteLine(DownloaderResources.TIMEOUT_DOWNLOADING_FILE, address);
            }
            return data;
        }
    }
}
