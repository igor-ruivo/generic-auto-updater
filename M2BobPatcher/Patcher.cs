using M2BobPatcher.ExceptionHandler;
using M2BobPatcher.Resources.TextResources;
using System;
using System.Windows.Forms;

namespace M2BobPatcher {
    static class Patcher {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static readonly System.Threading.Mutex mutex = new System.Threading.Mutex(true, string.Format("Global\\{{{0}}}", "{7A241E4E-6BA9-4F7A-8060-2BE66E9E9D60}"));
        [STAThread]
        static void Main() {
            if (mutex.WaitOne(TimeSpan.Zero, true)) {
                try {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new PatcherMainWindow());
                }
                catch (Exception ex) {
                    Handler.Handle(ex);
                }
                finally {
                    mutex.ReleaseMutex();
                }
            }
            else
                MessageBox.Show(MainWindowResources.ALREADY_RUNNING, MainWindowResources.ALREADY_RUNNING_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
