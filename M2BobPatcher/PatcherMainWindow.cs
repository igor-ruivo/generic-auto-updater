using M2BobPatcher.TextResources;
using M2BobPatcher.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

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
            IPatcherEngine engine = new PatcherEngine(loggerDisplay, downloaderDisplay, starter);
            Task.Factory.StartNew(() => engine.Patch());
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(MainWindow.M2BOB_WEBSITE);
        }

        private void starter_Click(object sender, EventArgs e) {
            Process.Start(MainWindow.M2BOB_STARTER);
        }
    }
}
