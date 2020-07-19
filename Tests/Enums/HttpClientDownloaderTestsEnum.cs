namespace Tests.Enums {
    /// <summary>
    /// The class with the enums used in the HttpClientDownloaderTest class.
    /// </summary>
    public static class HttpClientDownloaderTestsEnum {
        /// <summary>
        /// The enum containing the different possible server behaviours.
        /// </summary>
        public enum ServerBehaviours {
            /// <summary>
            /// A server with normal latency and behaviour.
            /// </summary>
            Normal,

            /// <summary>
            /// A server with tolerable simulated random latency.
            /// </summary>
            Latency,

            /// <summary>
            /// A server which forces a timeout during a read.
            /// </summary>
            TimeoutDuringRead,

            /// <summary>
            /// A server whose response to requests are inconsistent.
            /// </summary>
            Inconsistent
        };
    }
}
