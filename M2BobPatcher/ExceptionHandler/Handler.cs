using M2BobPatcher.ExceptionHandler.Exceptions;
using M2BobPatcher.Resources.TextResources;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace M2BobPatcher.ExceptionHandler {
    /// <summary>
    /// The class responsible for all exception handling during the patch.
    /// </summary>
    static class Handler {
        /// <summary>
        /// Handles the received <c>Exception</c> based on its type.
        /// Any un-cased exception triggers an unknown error prompt.
        /// </summary>
        public static void Handle(Exception ex) {
            switch (ex) {
                case AggregateException e1:
                    Handle(e1.InnerExceptions.First());
                    break;
                // The assignment to these local variables is needed.
                case FileNotFoundException e1:
                case DirectoryNotFoundException e2:
                case DataTamperedException e3:
                    ShowError(ErrorHandlerResources.AV_FALSE_POSITIVE, ErrorHandlerResources.ERROR_TITLE_AV);
                    break;
                case WebException e1:
                case HttpRequestException e2:
                case InvalidDataException e3:
                case DecoderFallbackException e4:
                case ObjectDisposedException e5:
                    ShowError(ErrorHandlerResources.TIMEOUT_DOWNLOADING_RESOURCE, ErrorHandlerResources.ERROR_TITLE_NETWORKING);
                    break;
                case SecurityException e1:
                case UnauthorizedAccessException e2:
                case PathTooLongException e3:
                case IOException e4:
                    ShowError(ErrorHandlerResources.ERROR_IO_EXPLORER, ErrorHandlerResources.ERROR_TITLE_EXPLORER);
                    break;
                default:
                    ShowError(ErrorHandlerResources.UNKNOWN_ERROR, ErrorHandlerResources.ERROR_TITLE_UNKNOWN);
                    break;
            }
        }

        /// <summary>
        /// Informs the user, through a <c>MessageBox</c> whose text and caption are received in argument, that something went wrong while patching.
        /// Exits the application terminating all Threads after the user clicks in the OK button.
        /// </summary>
        private static void ShowError(string text, string caption) {
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }
    }
}