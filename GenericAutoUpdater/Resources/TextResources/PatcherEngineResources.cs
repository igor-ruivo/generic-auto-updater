namespace M2BobPatcher.Resources.TextResources {
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
        public static readonly string DOWNLOADING_OUTDATED_CONTENT = "Downloading outdated content.";

        /// <summary>
        /// The text description of the "Downloading missing content" step.
        /// </summary>
        public static readonly string DOWNLOADING_MISSING_CONTENT = "Downloading missing content.";

        /// <summary>
        /// The text description of the "Analysing local metadata" step.
        /// </summary>
        public static readonly string GENERATING_LOCAL_METADATA = "Analysing local metadata.";

        /// <summary>
        /// The text description of the "Parsing server metadata" step.
        /// </summary>
        public static readonly string PARSING_SERVER_METADATA = "Parsing server metadata.";

        /// <summary>
        /// The text description that is used on both existing labels at start-up.
        /// </summary>
        public static readonly string STARTING = "Fetching server metadata.";

        /// <summary>
        /// The text description that is used on the general label when the AutoPatcher finished with success.
        /// </summary>
        public static readonly string FINISHED = "Everything up to date. You can now start M2Bob!";

        /// <summary>
        /// The text description that is used on the downloader label when the AutoPatcher finished with success.
        /// </summary>
        public static readonly string ALL_FILES_ANALYZED = "All files analyzed in {0}.";
    }
}
