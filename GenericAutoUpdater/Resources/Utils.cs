using GenericAutoUpdater.UI;
using GenericAutoUpdater.UI.Wrappers;
using System;
using System.ComponentModel;

namespace GenericAutoUpdater.Resources {
    /// <summary>
    /// A class with various utilities, used throughout all the patch.
    /// </summary>
    public static class Utils {
        /// <summary>
        /// Checks if the received <c>AggregateException</c> contains at least one exception of the type of the exception received in the second parameter (another).
        /// </summary>
        public static bool AggregateContainsSpecificException(AggregateException ex, Exception another) {
            foreach (Exception exception in ex.InnerExceptions)
                if (exception.GetType() == another.GetType())
                    return true;
            return false;
        }

        /// <summary>
        /// Requests the UI Thread, through the BackgroundWorker (bw), to log the received message in the specified Label (label).
        /// </summary>
        public static void Log(BackgroundWorker bw, string message, ProgressiveWidgetsEnum.Label label) {
            bw.ReportProgress(0, new LabelWrapper(label, message));
        }

        /// <summary>
        /// Requests the UI Thread, through the BackgroundWorker (bw), to log the received progress in the specified ProgressBar (progressBar).
        /// </summary>
        public static void Progress(BackgroundWorker bw, int progress, ProgressiveWidgetsEnum.ProgressBar progressBar) {
            bw.ReportProgress(0, new ProgressBarWrapper(progressBar, progress));
        }

        /// <summary>
        /// Converts a byte number into a more readable unit.
        /// </summary>
        public static string BytesToString(long byteCount, int decimalPlaces) {
            string[] suf = { " B", " KB", " MB", " GB", " TB", " PB", " EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int index = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1000)));
            double num = Math.Round(bytes / Math.Pow(1000, index), decimalPlaces);
            return (Math.Sign(byteCount) * num).ToString() + suf[index];
        }
    }
}
