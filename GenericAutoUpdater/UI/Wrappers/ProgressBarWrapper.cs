namespace GenericAutoUpdater.UI.Wrappers {
    /// <summary>
    /// The class used to model the wrapper of a <c>ProgressBar</c>.
    /// </summary>
    class ProgressBarWrapper : IWidgetWrapper {
        /// <summary>
        /// The ProgressBar's representation as an enum
        /// </summary>
        public ProgressiveWidgetsEnum.ProgressBar ProgressBar { get; }

        /// <summary>
        /// The progress percentage of the progress bar.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Initializes a new instance of the <c>ProgressBarWrapper</c> class with the specified <c>ProgressiveWidgetsEnum.ProgressBar</c> and value.
        /// </summary>
        public ProgressBarWrapper(ProgressiveWidgetsEnum.ProgressBar progressBar, int value) {
            ProgressBar = progressBar;
            Value = value;
        }
    }
}
