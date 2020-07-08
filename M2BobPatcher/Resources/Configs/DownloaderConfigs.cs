namespace M2BobPatcher.Resources.Configs {
    public static class DownloaderConfigs {
        public static readonly int MAX_DOWNLOAD_RETRIES_PER_FILE = 5;
        public static readonly int INTERVAL_MS_BETWEEN_DOWNLOAD_RETRIES = 1000;
        public static readonly int TIMEOUT_MS_WAITING_FOR_READ = 10000;
        public static readonly int BUFFER_SIZE = 8192;
        public static readonly int INFORM_PROGRESS_ONE_IN_X_READS = 10;
    }
}
