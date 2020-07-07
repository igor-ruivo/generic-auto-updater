using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using M2BobPatcher.Engine;
using M2BobPatcher.TextResources;

namespace M2BobPatcher.ExceptionHandler {
    class Handler : IExceptionHandler {
        private IPatcherEngine Engine;

        public Handler(IPatcherEngine patcherEngine) {
            Engine = patcherEngine;
        }

        void IExceptionHandler.Handle(Exception ex) {
            switch (ex) {
                case AggregateException e1:
                    ((IExceptionHandler)this).Handle(e1.InnerExceptions.First());
                    break;
                case FileNotFoundException e2:
                case DirectoryNotFoundException e3:
                    Repatch(ErrorHandlerResources.AV_FALSE_POSITIVE, ErrorHandlerResources.ERROR_TITLE_AV);
                    break;
                case WebException e4:
                case HttpRequestException e5:
                    Repatch(ErrorHandlerResources.TIMEOUT_DOWNLOADING_RESOURCE, ErrorHandlerResources.ERROR_TITLE_NETWORKING);
                    break;
                default:
                    throw ex;
            }
        }

        private void Repatch(string text, string caption) {
            switch (MessageBox.Show(text, caption, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error)) {
                case DialogResult.Cancel:
                    Application.Exit();
                    break;
                default:
                    Engine.Patch();
                    break;
            }
        }
    }
}
