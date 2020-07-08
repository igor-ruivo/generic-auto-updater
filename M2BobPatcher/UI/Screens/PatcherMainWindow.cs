using M2BobPatcher.Engine;
using M2BobPatcher.Resources.TextResources;
using M2BobPatcher.TextResources;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace M2BobPatcher {
    public partial class PatcherMainWindow : Form {
        public PatcherMainWindow() {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            if (e.ProgressPercentage < 0) {
                string message = (string)e.UserState;
                switch (e.ProgressPercentage) {
                    case -1:
                        loggerDisplay.Text = message;
                        break;
                    case -2:
                        downloaderDisplay.Text = message;
                        break;
                    default:
                        break;
                }
            }
            else {
                if ((bool)e.UserState)
                    fileProgressBar.Value = e.ProgressPercentage;
                else
                    wholeProgressBar.Value = e.ProgressPercentage;
            }
        }

        private void setupWindowProperties() {
            Text = string.Format(MainWindowResources.MAIN_WINDOW_TITLE, MainWindowResources.CURRENT_VERSION);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
        }

        private void loadWindow(object sender, EventArgs e) {
            setupWindowProperties();
            backgroundWorker1.RunWorkerAsync();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(MainWindowResources.M2BOB_WEBSITE);
        }

        private void starter_Click(object sender, EventArgs e) {
            Process.Start(MainWindowResources.M2BOB_STARTER);
            Application.Exit();
        }

        private void author_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(MainWindowResources.AUTHOR_WEBSITE);
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            BackgroundWorker bw = sender as BackgroundWorker;
            IPatcherEngine engine = new PatcherEngine(bw);
            engine.Patch();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            starter.Enabled = true;
            loggerDisplay.Text = PatcherEngineResources.FINISHED;
        }
    }
}
