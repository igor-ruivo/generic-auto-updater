using M2BobPatcher.UI;
using M2BobPatcher.UI.Wrappers;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace M2BobPatcher.Resources {
    /// <summary>
    /// A class with various utilities, used throughout all the patch.
    /// </summary>
    static class Utils {
        /// <summary>
        /// Performs a sanity check on the received <c>byte[]</c>, assuming it corresponds to the downloaded server metadata file.
        /// Returns the received data as a string if everything goes right, and throws an <c>InvalidDataException()</c> otherwise.
        /// </summary>
        public static string PerformPatchDirectorySanityCheck(byte[] data) {
            string serverMetadata = Encoding.Default.GetString(data);
            string patchDirectory = serverMetadata.Trim().Split(new[] { "\n" }, StringSplitOptions.None)[0];
            // Make sure that the first line of the file is really an url.
            if (!Uri.TryCreate(patchDirectory, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                throw new InvalidDataException();
            return Encoding.Default.GetString(data);
        }

        /// <summary>
        /// Checks if the received <c>AggregateException</c> contains at least one <c>ObjectDisposedException</c>.
        /// </summary>
        public static bool AggregateContainsObjectDisposedException(AggregateException ex) {
            foreach (Exception exception in ex.InnerExceptions)
                if (exception is ObjectDisposedException)
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
    }
}
