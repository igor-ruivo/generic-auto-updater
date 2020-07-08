using M2BobPatcher.TextResources;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace M2BobPatcher {
    static class Patcher {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            if (PriorProcess() != null) {
                MessageBox.Show(MainWindowResources.ALREADY_RUNNING, MainWindowResources.ALREADY_RUNNING_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PatcherMainWindow());
        }

        public static Process PriorProcess() {
            Process curr = Process.GetCurrentProcess();
            Process[] procs = Process.GetProcessesByName(curr.ProcessName);
            foreach (Process p in procs) {
                if ((p.Id != curr.Id) &&
                    (p.MainModule.FileName == curr.MainModule.FileName))
                    return p;
            }
            return null;
        }
    }
}
