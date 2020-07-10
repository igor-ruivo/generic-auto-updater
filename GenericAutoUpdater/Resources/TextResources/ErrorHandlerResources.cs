namespace M2BobPatcher.Resources.TextResources {
    /// <summary>
    /// The class with the description of all handled runtime errors.
    /// </summary>
    public static class ErrorHandlerResources {
        /// <summary>
        /// The title of the window triggered whenever there is a consistency error.
        /// </summary>
        public static readonly string ERROR_TITLE_AV = "Consistency Error";

        /// <summary>
        /// The title of the window triggered whenever there is an unknown error.
        /// </summary>
        public static readonly string ERROR_TITLE_UNKNOWN = "Unknown Error";

        /// <summary>
        /// The title of the window triggered whenever there is a network error.
        /// </summary>
        public static readonly string ERROR_TITLE_NETWORKING = "Network Error";

        /// <summary>
        /// The title of the window triggered whenever there is an IO error.
        /// </summary>
        public static readonly string ERROR_TITLE_EXPLORER = "IO Error";

        /// <summary>
        /// The text description of the window triggered whenever there is an unknown error.
        /// </summary>
        public static readonly string UNKNOWN_ERROR = "An unknown error occurred. Please contact M2Bob staff.";

        /// <summary>
        /// The text description of the window triggered whenever there is a network error.
        /// </summary>
        public static readonly string TIMEOUT_DOWNLOADING_RESOURCE = "Couldn't connect to M2Bob AutoPatcher server. Please check if M2Bob website is online or check your internet connection.";

        /// <summary>
        /// The text description of the window triggered whenever there is a consistency error.
        /// </summary>
        public static readonly string AV_FALSE_POSITIVE = "One or more files or directories are missing or were tampered after applying the patch. Please check if your AntiVirus or Microsoft Defender are at fault and then run the AutoPatcher again.";

        /// <summary>
        /// The text description of the window triggered whenever there is an IO error.
        /// </summary>
        public static readonly string ERROR_IO_EXPLORER = "An error occurred while trying to write a file or directory in its destination path. Please check if you have permission to apply the patch in this current directory, or if this current directory is being used by another process, or if its path is too long.";
    }
}