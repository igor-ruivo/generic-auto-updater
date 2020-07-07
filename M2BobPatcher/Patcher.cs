using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using M2BobPatcher.TextResources;

namespace M2BobPatcher {
    static class Patcher {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, "AutoPatcherInstanceMutex", out createdNew)) {
                if (createdNew) {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new PatcherMainWindow());
                } else {
                    MessageBox.Show(MainWindowResources.ALREADY_RUNNING, MainWindowResources.ALREADY_RUNNING_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
        }
    }
}
