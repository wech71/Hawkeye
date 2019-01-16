namespace ACorns.Hawkeye
{
    partial class HawkeyeEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HawkeyeEditor));
            this.propertyGrid = new ACorns.Hawkeye.Tabs.XPropertyGrid();
            this.toolBar = new System.Windows.Forms.ToolBar();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.windowFinder = new ACorns.Hawkeye.WindowFinder();
            this.txtType = new System.Windows.Forms.TextBox();
            this.txtToString = new System.Windows.Forms.TextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mainPanel = new System.Windows.Forms.Panel();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.CommandsActiveLinkColor = System.Drawing.SystemColors.ActiveCaption;
            this.propertyGrid.CommandsDisabledLinkColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGrid.CommandsLinkColor = System.Drawing.SystemColors.ActiveCaption;
            this.propertyGrid.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.ExternalToolBar = this.toolBar;
            this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid.Location = new System.Drawing.Point(0, 64);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propertyGrid.Size = new System.Drawing.Size(304, 302);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.SelectRequest += new ACorns.Hawkeye.Tabs.SelectedObjectRequestHandler(this.propertyGrid_SelectRequest);
            this.propertyGrid.AddToolbarButtons += new System.EventHandler(this.propertyGrid_AddToolbarButtons);
            // 
            // toolBar
            // 
            this.toolBar.DropDownArrows = true;
            this.toolBar.ImageList = this.imageList;
            this.toolBar.Location = new System.Drawing.Point(0, 36);
            this.toolBar.Name = "toolBar";
            this.toolBar.ShowToolTips = true;
            this.toolBar.Size = new System.Drawing.Size(304, 28);
            this.toolBar.TabIndex = 3;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Silver;
            this.imageList.Images.SetKeyName(0, "");
            this.imageList.Images.SetKeyName(1, "LeftArrow.bmp");
            this.imageList.Images.SetKeyName(2, "RightArrow.bmp");
            this.imageList.Images.SetKeyName(3, "UpArrow.bmp");
            this.imageList.Images.SetKeyName(4, "");
            this.imageList.Images.SetKeyName(5, "");
            this.imageList.Images.SetKeyName(6, "");
            this.imageList.Images.SetKeyName(7, "");
            this.imageList.Images.SetKeyName(8, "");
            this.imageList.Images.SetKeyName(9, "");
            this.imageList.Images.SetKeyName(10, "");
            this.imageList.Images.SetKeyName(11, "");
            this.imageList.Images.SetKeyName(12, "");
            this.imageList.Images.SetKeyName(13, "");
            this.imageList.Images.SetKeyName(14, "");
            this.imageList.Images.SetKeyName(15, "");
            this.imageList.Images.SetKeyName(16, "");
            this.imageList.Images.SetKeyName(17, "");
            this.imageList.Images.SetKeyName(18, "");
            this.imageList.Images.SetKeyName(19, "");
            this.imageList.Images.SetKeyName(20, "");
            this.imageList.Images.SetKeyName(21, "");
            this.imageList.Images.SetKeyName(22, "");
            this.imageList.Images.SetKeyName(23, "");
            // 
            // windowFinder
            // 
            this.windowFinder.BackColor = System.Drawing.Color.White;
            this.windowFinder.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("windowFinder.BackgroundImage")));
            this.windowFinder.Location = new System.Drawing.Point(0, 1);
            this.windowFinder.Name = "windowFinder";
            // TODO: Code generation for 'this.windowFinder.SelectedHandle' failed because of Exception 'Invalid Primitive Type: System.IntPtr. Consider using CodeObjectCreateExpression.'.
            this.windowFinder.Size = new System.Drawing.Size(32, 32);
            this.windowFinder.TabIndex = 0;
            this.toolTip.SetToolTip(this.windowFinder, "WindowFinder - click & drag to select any .Net window from any process.");
            this.windowFinder.ActiveWindowSelected += new System.EventHandler(this.windowFinder_ActiveWindowSelected);
            this.windowFinder.ActiveWindowChanged += new System.EventHandler(this.windowFinder_ActiveWindowChanged);
            this.windowFinder.MouseDown += new System.Windows.Forms.MouseEventHandler(this.windowFinder_MouseDown);
            // 
            // txtType
            // 
            this.txtType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtType.Location = new System.Drawing.Point(32, 0);
            this.txtType.Name = "txtType";
            this.txtType.ReadOnly = true;
            this.txtType.Size = new System.Drawing.Size(272, 20);
            this.txtType.TabIndex = 1;
            this.txtType.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip.SetToolTip(this.txtType, "Object Type");
            // 
            // txtToString
            // 
            this.txtToString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtToString.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtToString.Location = new System.Drawing.Point(32, 16);
            this.txtToString.Name = "txtToString";
            this.txtToString.ReadOnly = true;
            this.txtToString.Size = new System.Drawing.Size(272, 20);
            this.txtToString.TabIndex = 2;
            this.txtToString.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip.SetToolTip(this.txtToString, "Object.ToString()");
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.windowFinder);
            this.mainPanel.Controls.Add(this.txtType);
            this.mainPanel.Controls.Add(this.txtToString);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(304, 36);
            this.mainPanel.TabIndex = 4;
            this.mainPanel.LocationChanged += new System.EventHandler(this.mainPanel_LocationChanged);
            // 
            // HawkeyeEditor
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(304, 366);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.toolBar);
            this.Controls.Add(this.mainPanel);
            this.Name = "HawkeyeEditor";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Hawkeye: The .Net Runtime Object Editor";
            this.Deactivate += new System.EventHandler(this.RuntimeEditor_Deactivate);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.RuntimeEditor_Closing);
            this.LocationChanged += new System.EventHandler(this.RuntimeEditor_LocationChanged);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Panel mainPanel;
        private ACorns.Hawkeye.Utils.UI.MessageBalloon messageBalloon;
        private ACorns.Hawkeye.Tabs.XPropertyGrid propertyGrid;
        private System.Windows.Forms.ToolBarButton showToolsButton;
        private System.Windows.Forms.ToolBar toolBar;
        private ACorns.Hawkeye.Tools.ToolsWindow toolsWindow;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TextBox txtToString;
        private System.Windows.Forms.TextBox txtType;
        private WindowFinder windowFinder;

        #endregion
    }
}