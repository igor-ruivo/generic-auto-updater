namespace M2BobPatcher.Resources.TextResources {
    /// <summary>
    /// The class with the text resources of all main window's textual fields.
    /// </summary>
    public static class MainWindowResources {
        /// <summary>
        /// The M2Bob main website.
        /// </summary>
        public static readonly string M2BOB_WEBSITE = "https://your-website/";

        /// <summary>
        /// The author main website.
        /// </summary>
        public static readonly string AUTHOR_WEBSITE = "https://www.linkedin.com/in/igor-ruivo/";

        /// <summary>
        /// The current version of the AutoPatcher.
        /// </summary>
        public static readonly string CURRENT_VERSION = "v" + System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;

        /// <summary>
        /// The title of the main window.
        /// </summary>
        public static readonly string MAIN_WINDOW_TITLE = "M2Bob Patcher {0}";

        /// <summary>
        /// The M2bob launcher.
        /// </summary>
        public static readonly string M2BOB_STARTER = "your-exe.exe";

        /// <summary>
        /// The text description of the window triggered whenever there is already another instance of the AutoPatcher running.
        /// </summary>
        public static readonly string ALREADY_RUNNING = "Another instance of the AutoPatcher is already running.";

        /// <summary>
        /// The title of the window triggered whenever there is already another instance of the AutoPatcher running.
        /// </summary>
        public static readonly string ALREADY_RUNNING_ERROR = "Error Starting AutoPatcher";
    }
}
