namespace GenericAutoUpdater.Resources.Configs {
    /// <summary>
    /// The class with the required configuration to be used by the downloader.
    /// </summary>
    public static class DownloaderConfigs {
        /// <summary>
        /// The number of attempts to successfully download a file before throwing a specific Exception.
        /// </summary>
        public static readonly int MAX_DOWNLOAD_RETRIES_PER_FILE = 3;

        /// <summary>
        /// The base time (in milliseconds) that the downloader will spend sleeping before retrying a failed download.
        /// </summary>
        public static readonly int BASE_MS_SLEEP_TIME_BETWEEN_DOWNLOAD_RETRIES = 1000;

        /// <summary>
        /// The time (in milliseconds) that the downloader will spend waiting for the current read request to be completed before closing the stream by force, thus throwing an <c>ObjectDisposedException</c>.
        /// </summary>
        public static readonly int TIMEOUT_MS_WAITING_FOR_READ = 10000;

        /// <summary>
        /// The downloader's buffer size, in bytes.
        /// </summary>
        public static readonly int BUFFER_SIZE = 8192;

        /// <summary>
        /// How many logs to the UI Thread will the downloader always omit between performed reads.
        /// </summary>
        public static readonly int INFORM_PROGRESS_EVERY_X_READS = 10;

        /// <summary>
        /// The weight of a measured speed sample on the current file's average download speed.
        /// Use a value from the interval [0, 1].
        /// </summary>
        public static readonly float SAMPLE_SPEED_WEIGHT = 0.125f;
    }
}
