using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                case FileNotFoundException e1:
                case AggregateException e2:
                case DirectoryNotFoundException e3:
                    Repatch(ErrorHandlerResources.AV_FALSE_POSITIVE, ErrorHandlerResources.ERROR_TITLE);
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
