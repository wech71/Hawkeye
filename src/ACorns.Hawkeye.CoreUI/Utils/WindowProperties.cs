using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ACorns.Hawkeye.Plugins;
using ACorns.Hawkeye.Core.Utils;
using ACorns.Hawkeye.Core.UI;

namespace ACorns.Hawkeye.Utils
{
	/// <summary>
	/// Summary description for WindowProperties.
	/// </summary>
	internal class WindowProperties : IDisposable
	{
		private static Pen drawPen = new Pen(Brushes.Red, 2);
		private IntPtr detectedWindow = IntPtr.Zero;
		private Point lastPoint = Point.Empty;

		private object selectedObject;

		public IntPtr DetectedWindow
		{
			get { return detectedWindow; }
		}

		public object ActiveObject
		{
			get { return selectedObject; }
		}

		public object ActiveWindow
		{
			get
			{
				if (detectedWindow != IntPtr.Zero)
				{
					return Control.FromHandle(detectedWindow);
				}
				else
				{
					return null;
				}
			}
		}

		public string ClassName
		{
			get
			{
				if (!IsValid)
					return null;
				return NativeUtils.GetClassName(detectedWindow);
			}
		}

		public bool IsManagedByClassName
		{
			get
			{
				string className = ClassName;
				if (className != null && className.StartsWith("WindowsForms10"))
				{
					return true;
				}
				else
				{
					return false;
				}
				//Match match = classNameRegex.Match(ClassName);
				//return match.Success;
			}
		}

		public bool IsValid
		{
			get { return detectedWindow != IntPtr.Zero; }
		}

		public bool IsManaged
		{
			get { return ActiveWindow != null; }
		}

		internal bool SetWindowHandle(IntPtr handle, Point lastPoint)
		{
			Refresh();
			this.lastPoint = lastPoint;
			this.detectedWindow = handle;
			Refresh();
			Highlight();

			bool changed = false;
			object activeWindow = ActiveWindow;
			if ( activeWindow != selectedObject )
			{
				changed = true;
				selectedObject = activeWindow;
			}

			// try to load plugins

			Point checkPoint = lastPoint;
			if ( detectedWindow != IntPtr.Zero )
				checkPoint = NativeUtils.NativeScreenToClient(detectedWindow, lastPoint);

			object lastSelected = detectedWindow;

			if (checkPoint != Point.Empty && PluginManager.Instance.ResolveSelection(checkPoint, lastSelected, ref selectedObject))
			{
				changed = true;
			}
				
			return changed;
		}

		public void Refresh()
		{
			if (!IsValid)
				return;
			IntPtr toUpdate = this.detectedWindow;
			IntPtr parentWindow = NativeUtils.GetParent(toUpdate);
			if (parentWindow != IntPtr.Zero)
			{
				toUpdate = parentWindow; // using parent
			}

			NativeUtils.InvalidateRect(toUpdate, IntPtr.Zero, true);
			NativeUtils.UpdateWindow(toUpdate);
			bool result = NativeUtils.RedrawWindow(toUpdate, IntPtr.Zero, IntPtr.Zero, NativeUtils.RDW.RDW_FRAME | NativeUtils.RDW.RDW_INVALIDATE | NativeUtils.RDW.RDW_UPDATENOW | NativeUtils.RDW.RDW_ERASENOW | NativeUtils.RDW.RDW_ALLCHILDREN);
		}

		public void Highlight()
		{
			IntPtr windowDC;
			RECT windowRect = new RECT(0, 0, 0, 0);
			NativeUtils.GetWindowRect(detectedWindow, ref windowRect);

			IntPtr parentWindow = NativeUtils.GetParent(detectedWindow);
			windowDC = NativeUtils.GetWindowDC(detectedWindow);
			if (windowDC != IntPtr.Zero)
			{
				Graphics graph = Graphics.FromHdc(windowDC, detectedWindow);
				graph.DrawRectangle(drawPen, 1, 1, windowRect.Width - 2, windowRect.Height - 2);
				graph.Dispose();
				NativeUtils.ReleaseDC(detectedWindow, windowDC);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			Refresh();
		}

		#endregion
	}
}
