/* ****************************************************************************
 *  Hawkeye - The .Net Runtime Object Editor - Loader
 * 
 * Copyright (c) 2005 Corneliu I. Tusnea
 * 
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the author be held liable for any damages arising from 
 * the use of this software.
 * Permission to use, copy, modify, distribute and sell this software for any 
 * purpose is hereby granted without fee, provided that the above copyright 
 * notice appear in all copies and that both that copyright notice and this 
 * permission notice appear in supporting documentation.
 * 
 * Corneliu I. Tusnea (corneliutusnea@yahoo.com.au)
 * http://www.acorns.com.au/hawkeye/
 * ****************************************************************************/

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ACorns.Hawkeye.Tools.Logging
{
	/// <summary>
	/// Summary description for LoggingUI.
	/// </summary>
	internal class LoggingUI : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TextBox txtLog;
		private System.Windows.Forms.ToolBar toolBar;
		private System.Windows.Forms.ToolBarButton btnCopy;
		private System.Windows.Forms.ToolBarButton btnClear;
		private System.Windows.Forms.ImageList imageList;

		public LoggingUI()
		{
			InitializeComponent();

			txtLog.Select();
		}

		#region Component Designer generated code
		private System.ComponentModel.IContainer components;
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion
		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(LoggingUI));
			this.txtLog = new System.Windows.Forms.TextBox();
			this.toolBar = new System.Windows.Forms.ToolBar();
			this.btnCopy = new System.Windows.Forms.ToolBarButton();
			this.btnClear = new System.Windows.Forms.ToolBarButton();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// txtLog
			// 
			this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtLog.Location = new System.Drawing.Point(0, 28);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtLog.Size = new System.Drawing.Size(396, 84);
			this.txtLog.TabIndex = 0;
			this.txtLog.Text = "";
			// 
			// toolBar
			// 
			this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					   this.btnCopy,
																					   this.btnClear});
			this.toolBar.DropDownArrows = true;
			this.toolBar.ImageList = this.imageList;
			this.toolBar.Location = new System.Drawing.Point(0, 0);
			this.toolBar.Name = "toolBar";
			this.toolBar.ShowToolTips = true;
			this.toolBar.Size = new System.Drawing.Size(396, 28);
			this.toolBar.TabIndex = 1;
			this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
			// 
			// btnCopy
			// 
			this.btnCopy.ImageIndex = 0;
			this.btnCopy.ToolTipText = "Copy Contents";
			// 
			// btnClear
			// 
			this.btnClear.ImageIndex = 1;
			this.btnClear.ToolTipText = "Clear Contents";
			// 
			// imageList
			// 
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Silver;
			// 
			// LoggingUI
			// 
			this.Controls.Add(this.txtLog);
			this.Controls.Add(this.toolBar);
			this.Name = "LoggingUI";
			this.Size = new System.Drawing.Size(396, 112);
			this.Load += new System.EventHandler(this.LoggingUI_Load);
			this.ResumeLayout(false);

		}
		#endregion
		
		private void btnCopy_Click(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject(txtLog.Text);
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			txtLog.Clear();
		}

		public void TextAdded(string newText)
		{
			txtLog.AppendText(newText);
			txtLog.SelectionStart = txtLog.Text.Length;
			txtLog.ScrollToCaret();
		}

		private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if ( e.Button == btnCopy )
			{
				btnCopy_Click(null, EventArgs.Empty);
			}
			else
				if ( e.Button == btnClear )
				{
					btnClear_Click(null, EventArgs.Empty);
				}
		}

		private void LoggingUI_Load(object sender, System.EventArgs e)
		{
			toolBar.Dock = DockStyle.Left;
		}
	}
}
