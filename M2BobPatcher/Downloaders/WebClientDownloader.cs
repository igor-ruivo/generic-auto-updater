using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace M2BobPatcher.Downloaders {
    static class WebClientDownloader {

        public static string DownloadString(string address) {
            string text;
            using (var client = new WebClient()) {
                text = client.DownloadString(address);
            }
            return text;
        }

        public static byte[] DownloadData(string address) {
            byte[] data;
            using (var client = new WebClient()) {
                data = client.DownloadData(address);
            }
            return data;
        }
    }
}
