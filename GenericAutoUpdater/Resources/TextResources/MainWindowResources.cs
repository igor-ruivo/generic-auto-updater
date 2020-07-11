namespace GenericAutoUpdater.Resources.TextResources {
    /// <summary>
    /// The class with the text resources of all main window's textual fields.
    /// </summary>
    public static class MainWindowResources {
        /// <summary>
        /// Your main website.
        /// </summary>
        public static readonly string WEBSITE = "https://your-website/";

        /// <summary>
        /// The author main website.
        /// </summary>
        public static readonly string AUTHOR_WEBSITE = "https://github.com/igor-ruivo/generic-auto-updater";

        /// <summary>
        /// The current version of the Auto-Updater.
        /// </summary>
        public static readonly string CURRENT_VERSION = "v" + System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;

        /// <summary>
        /// The title of the main window.
        /// </summary>
        public static readonly string MAIN_WINDOW_TITLE = "Generic Auto-Updater {0}";

        /// <summary>
        /// Your client's launcher.
        /// </summary>
        public static readonly string STARTER = "your-exe.exe";

        /// <summary>
        /// The text description of the window triggered whenever there is already another instance of the Auto-Updater running.
        /// </summary>
        public static readonly string ALREADY_RUNNING = "Another instance of the Auto-Updater is already running.";

        /// <summary>
        /// The title of the window triggered whenever there is already another instance of the Auto-Updater running.
        /// </summary>
        public static readonly string ALREADY_RUNNING_ERROR = "Error Starting Auto-Updater";
    }
}
