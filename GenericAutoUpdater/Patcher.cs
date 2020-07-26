using GenericAutoUpdater.ExceptionHandler;
using GenericAutoUpdater.Resources.TextResources;
using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace GenericAutoUpdater {
    /// <summary>
    /// The class with the entry point for the Auto-Updater.
    /// </summary>
    static class Patcher {
        /// <summary>
        /// The main entry point for the application.
        /// Only one application instance is allowed due to <c>Mutex</c> usage.
        /// </summary>
        [STAThread]
        static void Main() {
            MutexAccessRule allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            MutexSecurity securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);
            using (Mutex mutex = new Mutex(false, string.Format("Global\\{{{0}}}",
            ((System.Runtime.InteropServices.GuidAttribute)System.Reflection.Assembly.GetExecutingAssembly().
            GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false).
                GetValue(0)).Value.ToString()), out _, securitySettings)) {
                bool hasHandle = false;
                try {
                    try {
                        hasHandle = mutex.WaitOne(TimeSpan.Zero, false);
                        if (!hasHandle) {
                            MessageBox.Show(MainWindowResources.ALREADY_RUNNING, MainWindowResources.ALREADY_RUNNING_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                    catch (AbandonedMutexException) {
                        hasHandle = true;
                    }
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new PatcherMainWindow());
                }
                catch (Exception ex) {
                    Handler.Handle(ex);
                }
                finally {
                    if (hasHandle)
                        mutex.ReleaseMutex();
                }
            }
        }
    }
}