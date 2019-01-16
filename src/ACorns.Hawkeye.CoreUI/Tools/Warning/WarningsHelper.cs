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
using System.Windows.Forms;
using ACorns.Hawkeye.Utils;

namespace ACorns.Hawkeye.Tools.Warning
{
	/// <summary>
	/// Summary description for WarningsHelper.
	/// </summary>
	internal class WarningsHelper
	{
		#region Instance
		private static WarningsHelper instance = new WarningsHelper();
		/// <summary>
		/// Singleton instance of the WarningsHelper.
		/// </summary>
		public static WarningsHelper Instance
		{
			get { return instance; }
		}
		#endregion

		private Hashtable dangerousProperties = new Hashtable();

		private WarningsHelper()
		{
			InitDangerousProperties();
		}

		public bool SetPropertyWarning(string propertyName, object target)
		{
			if( dangerousProperties.ContainsKey(propertyName) )
			{
				string targetType = target!=null?(" on '" + target.GetType().Name + "' "):"";
				DialogResult result = MessageBox.Show("Changing the value of the '" + propertyName + "' " + targetType + "is dangerous and can get the application in an unstable state.\r\nAre you sure you want to do this?", SystemUtils.ApplicationName, 
					MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
				return result==DialogResult.Yes;
			}
			return true;
		}

		#region InitDangerousProperties
		private void InitDangerousProperties()
		{
			dangerousProperties.Add("WindowTarget", true);
		}
		#endregion
	}
}
