namespace M2BobPatcher.UI.Wrappers {
    class ProgressBarWrapper : IWidgetWrapper {

        public ProgressiveWidgetsEnum.ProgressBar ProgressBar { get; }

        public int Value { get; }

        public ProgressBarWrapper(ProgressiveWidgetsEnum.ProgressBar progressBar, int value) {
            ProgressBar = progressBar;
            Value = value;
        }
    }
}
