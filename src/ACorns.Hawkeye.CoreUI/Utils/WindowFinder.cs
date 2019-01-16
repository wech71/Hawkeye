/* ****************************************************************************
 *  Hawkeye - The .Net Runtime Object Editor
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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ACorns.Hawkeye.Plugins;
using ACorns.Hawkeye.Utils;
using ACorns.Hawkeye.Utils.UI;
using ACorns.Hawkeye.Core.Utils;

namespace ACorns.Hawkeye
{
	/// <summary>
	/// WindowFinder - Control to help find other windows/controls.
	/// </summary>
	[DefaultEvent("ActiveWindowChanged")]
	internal class WindowFinder : UserControl
	{
		public event EventHandler ActiveWindowChanged;
		public event EventHandler ActiveWindowSelected;


		private bool searching = false;
		private WindowProperties window;
		private Point lastPoint = Point.Empty;

		public WindowFinder()
		{
			window = new WindowProperties();
			this.MouseDown += new MouseEventHandler(WindowFinder_MouseDown);
			this.Size = new Size(32, 32);

			InitializeComponent();

			this.BackgroundImage = SystemUtils.LoadImage("Hawkeye.gif");
		}

		protected override void Dispose(bool disposing)
		{
			window.Dispose();
			base.Dispose(disposing);
		}

		#region Designer Generated

		private void InitializeComponent()
		{
			this.BackColor = System.Drawing.Color.White;
			this.Name = "WindowFinder";
			this.Size = new System.Drawing.Size(32, 32);
		}

		#endregion

		#region DetectedWindowProperties

		public WindowProperties Window
		{
			get { return window; }
		}

		#endregion

		#region Start/Stop Search

		public void StartSearch()
		{
			searching = true;

			Cursor.Current = new Cursor(GetType().Assembly.GetManifestResourceStream(SystemUtils.RESOURCES + "Eye.cur"));

			Capture = true;

			this.MouseMove += new MouseEventHandler(WindowFinder_MouseMove);
			this.MouseUp += new MouseEventHandler(WindowFinder_MouseUp);
		}

		public void EndSearch()
		{
			this.MouseMove -= new MouseEventHandler(WindowFinder_MouseMove);
			Capture = false;
			searching = false;
			Cursor.Current = Cursors.Default;

			if (ActiveWindowSelected != null)
			{
				ActiveWindowSelected(this, EventArgs.Empty);
			}
		}

		#endregion

		private void WindowFinder_MouseDown(object sender, MouseEventArgs e)
		{
			if (!searching)
				StartSearch();
		}

		private void WindowFinder_MouseMove(object sender, MouseEventArgs e)
		{
			if (!searching)
				EndSearch();

			// Grab the window from the screen location of the mouse.
			Point newPoint = this.PointToScreen(new Point(e.X, e.Y));
			POINT windowPoint = POINT.FromPoint(newPoint);
			IntPtr found = NativeUtils.WindowFromPoint(windowPoint);

			// we have a valid window handle
			if (found != IntPtr.Zero)
			{
				// give it another try, it might be a child window (disabled, hidden .. something else)
				// offset the point to be a client point of the active window
				if (NativeUtils.ScreenToClient(found, ref windowPoint))
				{
					// check if there is some hidden/disabled child window at this point
					IntPtr childWindow = NativeUtils.ChildWindowFromPoint(found, windowPoint);
					if (childWindow != IntPtr.Zero)
					{ // great, we have the inner child
						found = childWindow;
					}
				}
			}

			// Is this the same window as the last detected one?
			if (lastPoint!=newPoint)
			{
				lastPoint=newPoint;
				if ( window.SetWindowHandle(found, lastPoint) )
				{
					//Trace.WriteLine("FoundWindow:" + window.Name + ":" + window.Text + " Managed:" + window.IsManaged);
					InvokeActiveWindowChanged();
				}
			}
		}

		private void InvokeActiveWindowChanged()
		{
			if (ActiveWindowChanged != null)
				ActiveWindowChanged(this, EventArgs.Empty);
		}

		private void WindowFinder_MouseUp(object sender, MouseEventArgs e)
		{
			EndSearch();
		}

		public object SelectedObject
		{
			get { return window.ActiveObject; }
		}

		[Browsable(false)]
		public IntPtr SelectedHandle
		{
			get { return window.DetectedWindow; }
			set
			{
				this.window.SetWindowHandle(value, Point.Empty);
				InvokeActiveWindowChanged();
			}
		}

		public bool IsManagedByClassName
		{
			get { return this.window.IsManagedByClassName; }
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			pevent.Graphics.FillRectangle(SystemBrushes.Control, pevent.ClipRectangle);
			base.OnPaintBackground (pevent);
		}
	}
}