using ACorns.Hawkeye.Utils;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ACorns.Hawkeye.Tools
{
    internal class ToolsWindow : Form
    {
        private Container components = null;
        private ToolsView toolsView;

        public ToolsWindow()
        {
            this.InitializeComponent();
            base.Icon = SystemUtils.LoadIcon("Tools.ico");
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
            this.toolsView = new ToolsView();
            base.SuspendLayout();
            this.toolsView.Dock = DockStyle.Fill;
            this.toolsView.Location = new Point(0, 0);
            this.toolsView.Name = "toolsView";
            this.toolsView.Size = new Size(0x1b0, 0xde);
            this.toolsView.TabIndex = 0;
            this.AutoScaleBaseSize = new Size(5, 13);
            base.ClientSize = new Size(0x1b0, 0xde);
            base.Controls.Add(this.toolsView);
            base.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            base.Name = "ToolsWindow";
            base.StartPosition = FormStartPosition.Manual;
            this.Text = "Tools Window";
            base.ResumeLayout(false);
        }

        public ToolWindowEnum ActiveToolWindow
        {
            get
            {
                return this.toolsView.ActiveToolWindow;
            }
            set
            {
                this.toolsView.ActiveToolWindow = value;
            }
        }
    }
}

