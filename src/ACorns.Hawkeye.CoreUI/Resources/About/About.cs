using System;
//using System.ComponentModel;
using System.Diagnostics;
//using System.Drawing;
//using System.Resources;
//using System.Windows.Forms;
using ACorns.Hawkeye.Utils;

namespace ACorns.Hawkeye.Resources.About
{
    internal class About : System.Windows.Forms.Form
    {
        private System.ComponentModel.Container components = null;

        public About()
        {
            this.InitializeComponent();
        }

        private void About_Load(object sender, System.EventArgs e)
        {
            this.Text = HawkeyeAppUtils.FullApplicationName;
        }

        private void backHomeBaby_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.backHomeBaby.Text);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ACorns.Hawkeye.Resources.About.About));
            this.logo = new System.Windows.Forms.PictureBox();
            this.labelName = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.backHomeBaby = new System.Windows.Forms.LinkLabel();
            this.allTheBlaBlaLabel = new System.Windows.Forms.Label();
            this.theBordingLegalStuff = new System.Windows.Forms.Label();
            this.rightsNstuf = new System.Windows.Forms.Label();
            base.SuspendLayout();
            this.logo.Image = (System.Drawing.Image) resources.GetObject("logo.Image");
            this.logo.Location = new System.Drawing.Point(46, 16);
            this.logo.Name = "logo";
            this.logo.Size = new System.Drawing.Size(32, 32);
            this.logo.TabIndex = 0;
            this.logo.TabStop = false;
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            this.labelName.Location = new System.Drawing.Point(60, 76);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(304, 32);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Hawkeye: Aim. Zoom. Reveal!";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.pictureBox1.Image = (System.Drawing.Image) resources.GetObject("pictureBox1.Image");
            this.pictureBox1.Location = new System.Drawing.Point(304, 0x10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox2.Image = (System.Drawing.Image) resources.GetObject("pictureBox2.Image");
            this.pictureBox2.Location = new System.Drawing.Point(94, 8);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(184, 64);
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            this.backHomeBaby.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.backHomeBaby.Location = new System.Drawing.Point(52, 128);
            this.backHomeBaby.Name = "backHomeBaby";
            this.backHomeBaby.Size = new System.Drawing.Size(320, 16);
            this.backHomeBaby.TabIndex = 4;
            this.backHomeBaby.TabStop = true;
            this.backHomeBaby.Text = "http://hawkeye.codeplex.com/";
            this.backHomeBaby.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.backHomeBaby.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.backHomeBaby_LinkClicked);
            this.allTheBlaBlaLabel.Location = new System.Drawing.Point(28, 96);
            this.allTheBlaBlaLabel.Name = "allTheBlaBlaLabel";
            this.allTheBlaBlaLabel.Size = new System.Drawing.Size(368, 32);
            this.allTheBlaBlaLabel.TabIndex = 5;
            this.allTheBlaBlaLabel.Text = "Hawkeye: The only tool that allows you to edit any .Net objects at runtime.";
            this.allTheBlaBlaLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.theBordingLegalStuff.Location = new System.Drawing.Point(80, 148);
            this.theBordingLegalStuff.Name = "theBordingLegalStuff";
            this.theBordingLegalStuff.Size = new System.Drawing.Size(288, 16);
            this.theBordingLegalStuff.TabIndex = 6;
            this.theBordingLegalStuff.Text = "Author: Corneliu I. Tusnea";
            this.theBordingLegalStuff.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.rightsNstuf.Location = new System.Drawing.Point(12, 160);
            this.rightsNstuf.Name = "rightsNstuf";
            this.rightsNstuf.Size = new System.Drawing.Size(400, 26);
            this.rightsNstuf.TabIndex = 7;
            this.rightsNstuf.Text = "This tool and its source code are released under the Microsoft Reciprocal License (Ms-RL).";
            this.rightsNstuf.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(424, 190);
            this.Controls.Add(this.rightsNstuf);
            this.Controls.Add(this.theBordingLegalStuff);
            this.Controls.Add(this.allTheBlaBlaLabel);
            this.Controls.Add(this.backHomeBaby);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.logo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hawkeye: Aim. Zoom. Reveal!";
            this.Load += new EventHandler(this.About_Load);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label allTheBlaBlaLabel;
        private System.Windows.Forms.LinkLabel backHomeBaby;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.PictureBox logo;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label rightsNstuf;
        private System.Windows.Forms.Label theBordingLegalStuff;
    }
}

