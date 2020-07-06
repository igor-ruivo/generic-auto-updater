namespace M2BobPatcher {
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.wholeProgressBar = new System.Windows.Forms.ProgressBar();
            this.fileProgressBar = new System.Windows.Forms.ProgressBar();
            this.starter = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.loggerDisplay = new System.Windows.Forms.Label();
            this.downloaderDisplay = new System.Windows.Forms.Label();
            this.author = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(145, 15);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(527, 135);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // wholeProgressBar
            // 
            this.wholeProgressBar.Location = new System.Drawing.Point(16, 196);
            this.wholeProgressBar.Margin = new System.Windows.Forms.Padding(4);
            this.wholeProgressBar.Name = "wholeProgressBar";
            this.wholeProgressBar.Size = new System.Drawing.Size(784, 15);
            this.wholeProgressBar.TabIndex = 1;
            // 
            // fileProgressBar
            // 
            this.fileProgressBar.Location = new System.Drawing.Point(16, 239);
            this.fileProgressBar.Margin = new System.Windows.Forms.Padding(4);
            this.fileProgressBar.Name = "fileProgressBar";
            this.fileProgressBar.Size = new System.Drawing.Size(784, 15);
            this.fileProgressBar.TabIndex = 2;
            // 
            // starter
            // 
            this.starter.Enabled = false;
            this.starter.Location = new System.Drawing.Point(697, 267);
            this.starter.Margin = new System.Windows.Forms.Padding(4);
            this.starter.Name = "starter";
            this.starter.Size = new System.Drawing.Size(103, 31);
            this.starter.TabIndex = 3;
            this.starter.Text = "Start M2Bob!";
            this.starter.UseVisualStyleBackColor = true;
            this.starter.Click += new System.EventHandler(this.starter_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(16, 281);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(107, 17);
            this.linkLabel1.TabIndex = 4;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "M2Bob Website";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // loggerDisplay
            // 
            this.loggerDisplay.AutoSize = true;
            this.loggerDisplay.Location = new System.Drawing.Point(12, 176);
            this.loggerDisplay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.loggerDisplay.Name = "loggerDisplay";
            this.loggerDisplay.Size = new System.Drawing.Size(108, 17);
            this.loggerDisplay.TabIndex = 5;
            this.loggerDisplay.Text = "Starting patch...";
            // 
            // downloaderDisplay
            // 
            this.downloaderDisplay.AutoSize = true;
            this.downloaderDisplay.Location = new System.Drawing.Point(12, 219);
            this.downloaderDisplay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.downloaderDisplay.Name = "downloaderDisplay";
            this.downloaderDisplay.Size = new System.Drawing.Size(0, 17);
            this.downloaderDisplay.TabIndex = 6;
            // 
            // author
            // 
            this.author.AutoSize = true;
            this.author.LinkColor = System.Drawing.Color.Black;
            this.author.Location = new System.Drawing.Point(341, 281);
            this.author.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.author.Name = "author";
            this.author.Size = new System.Drawing.Size(144, 17);
            this.author.TabIndex = 7;
            this.author.TabStop = true;
            this.author.Text = "Patcher by Igor Ruivo";
            this.author.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.author.VisitedLinkColor = System.Drawing.Color.Black;
            this.author.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.author_LinkClicked);
            // 
            // PatcherMainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(816, 311);
            this.Controls.Add(this.author);
            this.Controls.Add(this.downloaderDisplay);
            this.Controls.Add(this.loggerDisplay);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.starter);
            this.Controls.Add(this.fileProgressBar);
            this.Controls.Add(this.wholeProgressBar);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PatcherMainWindow";
            this.Load += new System.EventHandler(this.loadWindow);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ProgressBar wholeProgressBar;
        private System.Windows.Forms.ProgressBar fileProgressBar;
        private System.Windows.Forms.Button starter;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label loggerDisplay;
        private System.Windows.Forms.Label downloaderDisplay;
        private System.Windows.Forms.LinkLabel author;
    }
}

