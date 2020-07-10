using GenericAutoUpdater.ExceptionHandler;
using GenericAutoUpdater.Resources.TextResources;
using System;
using System.Windows.Forms;

namespace GenericAutoUpdater {
    /// <summary>
    /// The class with the entry point for the Patcher.
    /// </summary>
    static class Patcher {
        /// <summary>
        /// The <c>Mutex</c> that provides atomic guarantee that only one application instance is running at most at any given moment.
        /// Its name is the same as the assembly's GUID.
        /// </summary>
        static readonly System.Threading.Mutex mutex = new System.Threading.Mutex(true, string.Format("Global\\{{{0}}}",
            ((System.Runtime.InteropServices.GuidAttribute)System.Reflection.Assembly.GetExecutingAssembly().
            GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false).
                GetValue(0)).Value.ToString()));

        /// <summary>
        /// The main entry point for the application.
        /// Only one application instance is allowed due to <c>Mutex</c> usage.
        /// </summary>
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