using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M2BobPatcher.Resources.Configs {
    public static class DownloaderConfigs {
        public static readonly int MAX_DOWNLOAD_RETRIES_PER_FILE = 5;
        public static readonly int INTERVAL_MS_BETWEEN_DOWNLOAD_RETRIES = 1000;
    }
}
