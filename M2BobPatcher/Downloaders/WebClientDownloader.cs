using M2BobPatcher.Resources.Configs;
using M2BobPatcher.Resources.TextResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M2BobPatcher.Downloaders {
    static class WebClientDownloader {

        public static string DownloadString(string address) {
            string text = null;
            using (WebClient client = new WebClient()) {
                int tries = 0;
                for (; tries < DownloaderConfigs.MAX_DOWNLOAD_RETRIES_PER_FILE; tries++) {
                    try {
                        text = client.DownloadString(address);
                    }
                    catch(WebException) {
                        Thread.Sleep(DownloaderConfigs.INTERVAL_MS_BETWEEN_DOWNLOAD_RETRIES);
                        continue;
                    }
                    catch(Exception e) {
                        Console.WriteLine(DownloaderResources.ERROR_WHILE_DOWNLOADING_FILE, address);
                        Console.WriteLine(e.ToString());
                    }
                    break;
                }
                if(tries == DownloaderConfigs.MAX_DOWNLOAD_RETRIES_PER_FILE)
                    Console.WriteLine(DownloaderResources.TIMEOUT_DOWNLOADING_FILE, address);
            }
            return text;
        }

        public static byte[] DownloadData(string address) {
            byte[] data = null;
            using (WebClient client = new WebClient()) {
                int tries = 0;
                for (; tries < DownloaderConfigs.MAX_DOWNLOAD_RETRIES_PER_FILE; tries++) {
                    try {
                        data = client.DownloadData(address);
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
