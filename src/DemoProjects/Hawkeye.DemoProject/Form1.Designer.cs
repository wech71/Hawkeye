namespace Hawkeye.DemoProject
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.exitButton = new System.Windows.Forms.Button();
            this.thePanel = new System.Windows.Forms.Panel();
            this.aButton = new System.Windows.Forms.Button();
            this.theLabel = new Hawkeye.DemoProject.MyLabel();
            this.thePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // exitButton
            // 
            this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exitButton.Location = new System.Drawing.Point(197, 142);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 23);
            this.exitButton.TabIndex = 0;
            this.exitButton.Text = "E&xit";
            this.exitButton.UseVisualStyleBackColor = true;
            // 
            // thePanel
            // 
            this.thePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.thePanel.Controls.Add(this.aButton);
            this.thePanel.Location = new System.Drawing.Point(12, 71);
            this.thePanel.Name = "thePanel";
            this.thePanel.Size = new System.Drawing.Size(260, 65);
            this.thePanel.TabIndex = 2;
            // 
            // aButton
            // 
            this.aButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.aButton.Location = new System.Drawing.Point(44, 21);
            this.aButton.Name = "aButton";
            this.aButton.Size = new System.Drawing.Size(172, 23);
            this.aButton.TabIndex = 0;
            this.aButton.Text = "My parent is a Panel";
            this.aButton.UseVisualStyleBackColor = true;
            // 
            // theLabel
            // 
            this.theLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.theLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.theLabel.Location = new System.Drawing.Point(12, 9);
            this.theLabel.Name = "theLabel";
            this.theLabel.Size = new System.Drawing.Size(260, 59);
            this.theLabel.TabIndex = 1;
            this.theLabel.Text = "XXX";
            this.theLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.theLabel.TheText = "XXX";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 177);
            this.Controls.Add(this.thePanel);
            this.Controls.Add(this.theLabel);
            this.Controls.Add(this.exitButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.thePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button exitButton;
        private Hawkeye.DemoProject.MyLabel theLabel;
        private System.Windows.Forms.Panel thePanel;
        private System.Windows.Forms.Button aButton;
    }
}