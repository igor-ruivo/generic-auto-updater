using M2BobPatcher.Engine;
using M2BobPatcher.TextResources;
using M2BobPatcher.UI;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace M2BobPatcher {
    public partial class PatcherMainWindow : Form {
        public PatcherMainWindow() {
            InitializeComponent();
        }

        private void setupWindowProperties() {
            this.Text = string.Format(MainWindow.MAIN_WINDOW_TITLE, MainWindow.CURRENT_VERSION);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void loadWindow(object sender, EventArgs e) {
            setupWindowProperties();
            IPatcherEngine engine = new PatcherEngine(new UIComponents(loggerDisplay, downloaderDisplay, starter, fileProgressBar, wholeProgressBar));
            Task.Factory.StartNew(() => engine.Patch());
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(MainWindow.M2BOB_WEBSITE);
        }

        private void starter_Click(object sender, EventArgs e) {
            Process.Start(MainWindow.M2BOB_STARTER);
            Application.Exit();
        }

        private void author_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(MainWindow.AUTHOR_WEBSITE);
        }
    }
}
