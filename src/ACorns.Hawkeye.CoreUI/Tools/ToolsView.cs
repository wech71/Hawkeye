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
using ACorns.Hawkeye.Core.Utils;
using ACorns.Hawkeye.Tools.Logging;

namespace ACorns.Hawkeye.Tools
{
	/// <summary>
	/// Summary description for ToolsView.
	/// </summary>
	internal class ToolsView : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabLogging;
		private LoggingUI codeLoggingUI;
		private System.Windows.Forms.TabPage tabEventLog;
		private ACorns.Hawkeye.Tools.Logging.LoggingUI eventLoggingUI;
		
		public ToolsView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			tabControl.SelectedIndex = 1;

			CodeChangeLoggingSystem.Instance.TextAdded += new TextAddedHandler(codeLoggingUI.TextAdded);
			EventLoggingSystem.Instance.TextAdded += new TextAddedHandler(eventLoggingUI.TextAdded);
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
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
			this.tabLogging = new System.Windows.Forms.TabPage();
			this.codeLoggingUI = new ACorns.Hawkeye.Tools.Logging.LoggingUI();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabEventLog = new System.Windows.Forms.TabPage();
			this.eventLoggingUI = new ACorns.Hawkeye.Tools.Logging.LoggingUI();
			this.tabLogging.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabEventLog.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabLogging
			// 
			this.tabLogging.Controls.Add(this.codeLoggingUI);
			this.tabLogging.Location = new System.Drawing.Point(4, 22);
			this.tabLogging.Name = "tabLogging";
			this.tabLogging.Size = new System.Drawing.Size(392, 142);
			this.tabLogging.TabIndex = 0;
			this.tabLogging.Text = "Code Log";
			// 
			// codeLoggingUI
			// 
			this.codeLoggingUI.Dock = System.Windows.Forms.DockStyle.Fill;
			this.codeLoggingUI.Location = new System.Drawing.Point(0, 0);
			this.codeLoggingUI.Name = "codeLoggingUI";
			this.codeLoggingUI.Size = new System.Drawing.Size(392, 142);
			this.codeLoggingUI.TabIndex = 0;
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabLogging);
			this.tabControl.Controls.Add(this.tabEventLog);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.ItemSize = new System.Drawing.Size(50, 18);
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(400, 168);
			this.tabControl.TabIndex = 1;
			// 
			// tabEventLog
			// 
			this.tabEventLog.Controls.Add(this.eventLoggingUI);
			this.tabEventLog.Location = new System.Drawing.Point(4, 22);
			this.tabEventLog.Name = "tabEventLog";
			this.tabEventLog.Size = new System.Drawing.Size(392, 142);
			this.tabEventLog.TabIndex = 1;
			this.tabEventLog.Text = "Event Log";
			// 
			// loggingUI1
			// 
			this.eventLoggingUI.Dock = System.Windows.Forms.DockStyle.Fill;
			this.eventLoggingUI.Location = new System.Drawing.Point(0, 0);
			this.eventLoggingUI.Name = "eventLoggingUI";
			this.eventLoggingUI.Size = new System.Drawing.Size(392, 142);
			this.eventLoggingUI.TabIndex = 0;
			// 
			// ToolsView
			// 
			this.Controls.Add(this.tabControl);
			this.Name = "ToolsView";
			this.Size = new System.Drawing.Size(400, 168);
			this.tabLogging.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.tabEventLog.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		protected override void OnHandleDestroyed(EventArgs e)
		{
			CodeChangeLoggingSystem.Instance.TextAdded -= new TextAddedHandler(codeLoggingUI.TextAdded);
			EventLoggingSystem.Instance.TextAdded -= new TextAddedHandler(eventLoggingUI.TextAdded);

			base.OnHandleDestroyed (e);
		}

		public ToolWindowEnum ActiveToolWindow
		{
			get
			{
				if ( this.tabControl.SelectedTab == tabLogging )
					return ToolWindowEnum.CodeLog;
				else if ( this.tabControl.SelectedTab == tabEventLog )
					return ToolWindowEnum.EventLog;
				
				return ToolWindowEnum.Unknown;
			} 
			set
			{
				switch (value)
				{
					case ToolWindowEnum.CodeLog:
						tabControl.SelectedTab = tabLogging;
						return;
					case ToolWindowEnum.EventLog:
						tabControl.SelectedTab = tabEventLog;
						return;
				}
			}
		}
	}
}
