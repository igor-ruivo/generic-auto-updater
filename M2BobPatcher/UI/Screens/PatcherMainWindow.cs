using M2BobPatcher.Engine;
using M2BobPatcher.ExceptionHandler;
using M2BobPatcher.Resources.TextResources;
using M2BobPatcher.Resources.UIResources;
using M2BobPatcher.TextResources;
using M2BobPatcher.UI;
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
            switch (e.UserState) {
                case ProgressBarWrapper c1:
                    ProgressBarWrapper pbw = (ProgressBarWrapper)e.UserState;
                    switch (pbw.ProgressBar) {
                        case ProgressiveWidgetsEnum.ProgressBar.WholeProgressBar:
                            wholeProgressBar.Value = pbw.Value;
                            break;
                        case ProgressiveWidgetsEnum.ProgressBar.DownloadProgressBar:
                            fileProgressBar.Value = pbw.Value;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case LabelWrapper c2:
                    LabelWrapper lw = (LabelWrapper)e.UserState;
                    switch (lw.Label) {
                        case ProgressiveWidgetsEnum.Label.InformativeLogger:
                            loggerDisplay.Text = lw.Value;
                            break;
                        case ProgressiveWidgetsEnum.Label.DownloadLogger:
                            downloaderDisplay.Text = lw.Value;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void setupWindowProperties() {
            Text = string.Format(MainWindowResources.MAIN_WINDOW_TITLE, MainWindowResources.CURRENT_VERSION);
            downloaderDisplay.Text = PatcherEngineResources.STARTING;
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
            try {
                engine.Patch();
            }
            catch (Exception ex) {
                Handler.Handle(ex);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            starter.Enabled = true;
            loggerDisplay.Text = PatcherEngineResources.FINISHED;
        }
    }
}
