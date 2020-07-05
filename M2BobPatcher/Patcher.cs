using System;
using System.Windows.Forms;

namespace M2BobPatcher {
    static class Patcher {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PatcherMainWindow());
        }
    }
}
