using System.Windows.Forms;

namespace M2BobPatcher.UI {
    class UIComponents {

        private static Label LoggerDisplay;
        private static Label FileLoggerDisplay;
        private static Button Starter;
        private static ProgressBar FileProgressBar;
        private static ProgressBar WholeProgressBar;

        delegate void SetTextCallback(Label output, string text);
        delegate void SetToggleCallback(bool state);
        delegate void SetProgressCallback(ProgressBar bar, int progressPercentage);

        public UIComponents(Label loggerDisplay, Label fileLoggerDisplay, Button starter, ProgressBar fileProgressBar, ProgressBar wholeProgressBar) {
            LoggerDisplay = loggerDisplay;
            FileLoggerDisplay = fileLoggerDisplay;
            Starter = starter;
            FileProgressBar = fileProgressBar;
            WholeProgressBar = wholeProgressBar;
        }

        public int GetProgressBarProgress() {
            return WholeProgressBar.Value;
        }
        public void Log(string message, bool throughCommonLogger) {
            if (throughCommonLogger)
                SetOutputText(LoggerDisplay, message);
            else
                SetOutputText(FileLoggerDisplay, message);
        }

        public void Toggle(bool state) {
            SetToggleState(state);
        }

        public void RegisterProgress(int progress, bool throughFileProgressBar) {
            if (throughFileProgressBar)
                SetProgress(FileProgressBar, progress);
            else
                SetProgress(WholeProgressBar, progress);
        }

        private static void SetProgress(ProgressBar bar, int progressPercentage) {
            if (bar.InvokeRequired) {
                SetProgressCallback d = new SetProgressCallback(SetProgress);
                bar.Invoke(d, new object[] { bar, progressPercentage });
            } else
                bar.Value = progressPercentage;
        }

        private static void SetOutputText(Label output, string text) {
            if (output.InvokeRequired) {
                SetTextCallback d = new SetTextCallback(SetOutputText);
                output.Invoke(d, new object[] { output, text });
            }
            else
                output.Text = text;
        }

        private static void SetToggleState(bool state) {
            if (Starter.InvokeRequired) {
                SetToggleCallback d = new SetToggleCallback(SetToggleState);
                Starter.Invoke(d, new object[] { state });
            }
            else
                Starter.Enabled = state;
        }
    }
}
