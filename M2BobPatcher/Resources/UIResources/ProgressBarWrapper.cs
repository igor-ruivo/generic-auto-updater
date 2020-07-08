using M2BobPatcher.UI;

namespace M2BobPatcher.Resources.UIResources {
    class ProgressBarWrapper : IWidgetWrapper {

        public ProgressiveWidgetsEnum.ProgressBar ProgressBar { get; }

        public int Value { get; }

        public ProgressBarWrapper(ProgressiveWidgetsEnum.ProgressBar progressBar, int value) {
            ProgressBar = progressBar;
            Value = value;
        }
    }
}
