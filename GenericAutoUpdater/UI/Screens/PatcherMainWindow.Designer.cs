namespace GenericAutoUpdater {
    partial class PatcherMainWindow {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatcherMainWindow));
            this.wholeProgressBar = new System.Windows.Forms.ProgressBar();
            this.fileProgressBar = new System.Windows.Forms.ProgressBar();
            this.starter = new System.Windows.Forms.Button();
            this.loggerDisplay = new System.Windows.Forms.Label();
            this.downloaderDisplay = new System.Windows.Forms.Label();
            this.author = new System.Windows.Forms.LinkLabel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // wholeProgressBar
            // 
            this.wholeProgressBar.Location = new System.Drawing.Point(12, 92);
            this.wholeProgressBar.Name = "wholeProgressBar";
            this.wholeProgressBar.Size = new System.Drawing.Size(542, 10);
            this.wholeProgressBar.TabIndex = 1;
            // 
            // fileProgressBar
            // 
            this.fileProgressBar.Location = new System.Drawing.Point(12, 130);
            this.fileProgressBar.Name = "fileProgressBar";
            this.fileProgressBar.Size = new System.Drawing.Size(542, 10);
            this.fileProgressBar.TabIndex = 2;
            // 
            // starter
            // 
            this.starter.Enabled = false;
            this.starter.Location = new System.Drawing.Point(464, 28);
            this.starter.Name = "starter";
            this.starter.Size = new System.Drawing.Size(91, 25);
            this.starter.TabIndex = 3;
            this.starter.Text = "Launch app!";
            this.starter.UseVisualStyleBackColor = true;
            this.starter.Click += new System.EventHandler(this.starter_Click);
            // 
            // loggerDisplay
            // 
            this.loggerDisplay.AutoSize = true;
            this.loggerDisplay.Location = new System.Drawing.Point(13, 76);
            this.loggerDisplay.Name = "loggerDisplay";
            this.loggerDisplay.Size = new System.Drawing.Size(36, 13);
            this.loggerDisplay.TabIndex = 5;
            this.loggerDisplay.Text = "logger";
            // 
            // downloaderDisplay
            // 
            this.downloaderDisplay.AutoSize = true;
            this.downloaderDisplay.Location = new System.Drawing.Point(13, 114);
            this.downloaderDisplay.Name = "downloaderDisplay";
            this.downloaderDisplay.Size = new System.Drawing.Size(62, 13);
            this.downloaderDisplay.TabIndex = 6;
            this.downloaderDisplay.Text = "downloader";
            // 
            // author
            // 
            this.author.AutoSize = true;
            this.author.LinkColor = System.Drawing.Color.Black;
            this.author.Location = new System.Drawing.Point(14, 40);
            this.author.Name = "author";
            this.author.Size = new System.Drawing.Size(110, 13);
            this.author.TabIndex = 7;
            this.author.TabStop = true;
            this.author.Text = "Auto-Updater by Igor Ruivo";
            this.author.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.author.VisitedLinkColor = System.Drawing.Color.Black;
            this.author.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.author_LinkClicked);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.author);
            this.groupBox1.Controls.Add(this.starter);
            this.groupBox1.Location = new System.Drawing.Point(-1, 146);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(568, 85);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(367, 28);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 25);
            this.button1.TabIndex = 8;
            this.button1.Text = "Your Website";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(258, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(73, 74);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // PatcherMainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(566, 211);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.wholeProgressBar);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.fileProgressBar);
            this.Controls.Add(this.loggerDisplay);
            this.Controls.Add(this.downloaderDisplay);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PatcherMainWindow";
            this.Load += new System.EventHandler(this.loadWindow);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ProgressBar wholeProgressBar;
        private System.Windows.Forms.ProgressBar fileProgressBar;
        private System.Windows.Forms.Button starter;
        private System.Windows.Forms.Label loggerDisplay;
        private System.Windows.Forms.Label downloaderDisplay;
        private System.Windows.Forms.LinkLabel author;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

