namespace GenericAutoUpdater.Resources.TextResources {
    /// <summary>
    /// The class with the description of all the engine's steps and sub-steps.
    /// </summary>
    public static class PatcherEngineResources {
        /// <summary>
        /// The text description of the current step.
        /// </summary>
        public static readonly string STEP = "Step {0} of {1}: ";

        /// <summary>
        /// The text description of the "Checking for outdated content" step.
        /// </summary>
        public static readonly string CHECKING_OUTDATED_CONTENT = "Checking for outdated content.";

        /// <summary>
        /// The text description of the "Checking for missing content" step.
        /// </summary>
        public static readonly string CHECKING_MISSING_CONTENT = "Checking for missing content.";

        /// <summary>
        /// The text description of the "Downloading outdated content" step.
        /// </summary>
        public static readonly string DOWNLOADING_OUTDATED_CONTENT = "Downloading outdated content:";

        /// <summary>
        /// The text description of the "Downloading missing content" step.
        /// </summary>
        public static readonly string DOWNLOADING_MISSING_CONTENT = "Downloading missing content:";

        /// <summary>
        /// The text description of the "Analysing local metadata" step.
        /// </summary>
        public static readonly string GENERATING_LOCAL_METADATA = "Analysing local metadata.";

        /// <summary>
        /// The text description of the "Parsing server metadata" step.
        /// </summary>
        public static readonly string PARSING_SERVER_METADATA = "Parsing server metadata.";

        /// <summary>
        /// The text description that is used on the downloader label at start-up.
        /// </summary>
        public static readonly string STARTING_DOWNLOADER_LOGGER = "Ready to fetch server metadata.";

        /// <summary>
        /// The text description that is used on the downloader label while fetching server metadata.
        /// </summary>
        public static readonly string FETCHING = "Fetching server metadata.";

        /// <summary>
        /// The text description that is used on the main logger label at start-up.
        /// </summary>
        public static readonly string STARTING_MAIN_LOGGER = "Ready to start patching.";

        /// <summary>
        /// The text description that is used on the general label when the Auto-Updater finished with success.
        /// </summary>
        public static readonly string FINISHED = "Everything up to date. You can now start the client!";

        /// <summary>
        /// The text description that is used on the downloader label when the Auto-Updater finished with success.
        /// </summary>
        public static readonly string ALL_FILES_ANALYZED = "All files analyzed in {0}.";

        /// <summary>
        /// The text description that is used on the file count label when the Auto-Updater is downloading resources.
        /// </summary>
        public static readonly string FILE_COUNT = "File {0} of {1} ({2}%)";
    }
}
