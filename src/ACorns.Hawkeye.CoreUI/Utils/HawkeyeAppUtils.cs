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
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using ACorns.Hawkeye.Core.Utils;

namespace ACorns.Hawkeye.Utils
{
	public abstract class HawkeyeAppUtils
	{
		public const string RESOURCES = "ACorns.Hawkeye.Resources.";

		public static string Is64(bool value)
		{
			if (value)
				return "x64 ";
			else
				return "";
		}
		public static string AppName
		{
			get
			{
				return "Hawkeye " + Is64(NativeUtils.IsCurrentProcessX64) + ": ";
			}
		}
		public static string ApplicationName
		{
			get
			{
                return "Hawkeye " + Is64(NativeUtils.IsCurrentProcessX64) + typeof(HawkeyeAppUtils).Assembly.GetName().Version.ToString(3);
			}
		}
		public static string FullApplicationName
		{
			get { return ApplicationName + " - The .Net Runtime Object Editor"; }
		}
	}
	
    /// <summary>
	/// Summary description for SystemUtils.
	/// </summary>
	internal sealed class SystemUtils : HawkeyeAppUtils
	{
		private SystemUtils(){}


		

		
		/// <summary>
		/// Check if a window is from a managed process.
		/// .Net1.0/.Net1.1/.Net2.0 ClassNames: WindowsForms10.Window.8.app.0.33c0d9d
		/// .Net3.0(WPF) ClassNames: HwndWrapper[appname.exe;;b9d69842-a4fd-43e5-a6eb-d0f83c420544]
		/// </summary>
		/// <param name="className"></param>
		/// <returns></returns>
		public static bool IsManagedByClass(string className)
		{
			if (className != null)
			{
				//Trace.WriteLine("Checking class:" + className);
				if (className.StartsWith("WindowsForms10"))
					return true;
				if (className.StartsWith("HwndWrapper[") && className.EndsWith("]"))
					return true;
			}
			return false;
		}

		public static Bitmap LoadBitmap(string name)
		{
			Bitmap bitmap1 = (Bitmap)SystemUtils.LoadImage(name);
			bitmap1.MakeTransparent(Color.FromArgb(0xc0, 0xc0, 0xc0));
			return bitmap1;
		}

		public static Icon LoadIcon(string name)
		{
			if (name == null)
			{
				return null;
			}
            Stream stream1 = typeof(SystemUtils).Assembly.GetManifestResourceStream(RESOURCES + name);
			if (stream1 == null)
			{
				throw new ApplicationException("The image [" + name + "] not found in assembly [" + typeof(SystemUtils).Assembly.FullName + "]");
			}
			return new Icon(stream1);
		}

		public static Image LoadImage(string name)
		{
			if (name == null)
			{
				return null;
			}
            Stream stream1 = typeof(SystemUtils).Assembly.GetManifestResourceStream(RESOURCES + name);
			if (stream1 == null)
			{
				throw new ApplicationException("The image [" + name + "] not found in assembly [" + typeof(SystemUtils).Assembly.FullName + "]");
			}
			return Image.FromStream(stream1);
		}
	}
}
