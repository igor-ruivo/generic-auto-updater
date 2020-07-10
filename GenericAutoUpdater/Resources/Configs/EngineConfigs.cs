namespace GenericAutoUpdater.Resources.Configs {
    /// <summary>
    /// The class with the required configuration to be used by the engine.
    /// </summary>
    public static class EngineConfigs {
        /// <summary>
        /// The link to the server's metadata file.
        /// </summary>
        public static readonly string PATCH_METADATA = "http://your-server/patchlist.txt";

        /// <summary>
        /// The time (in milliseconds) that the engine will spend sleeping before performing any consistency checks.
        /// </summary>
        public static readonly int MS_TO_WAIT_FOR_AV_FALSE_POSITIVES = 1000;
    }
}
