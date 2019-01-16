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
using System.Collections;
using System.Text;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Drawing;
using System.Windows.Forms;

namespace ACorns.Hawkeye.Plugins
{
	internal class PluginManager
	{
		#region Instance

		private static PluginManager instance = new PluginManager();

		/// <summary>
		/// Singleton instance of the PluginManager.
		/// </summary>
		public static PluginManager Instance
		{
			get { return instance; }
		}

		#endregion

		private PluginManager()
		{
		}

		private IList finderExtenders = new ArrayList();
		private ToolBar toolbar;
		private HawkeyeHost host;

		public void Initialize(ToolBar toolbar)
		{
			this.toolbar = toolbar;
			host = new HawkeyeHost(toolbar);
			LoadPlugins();
		}

		private void LoadPlugins()
		{
			string loc = this.GetType().Assembly.Location;
			Configuration config = ConfigurationManager.OpenExeConfiguration(loc);

			KeyValueConfigurationCollection appSettings = config.AppSettings.Settings;
			foreach (KeyValueConfigurationElement keyValue in appSettings)
			{
				string key = keyValue.Key;
				if (key.StartsWith("Plugin."))
				{
					string pluginInfo = keyValue.Value;
					LoadPlugin(pluginInfo);
				}
			}
		}

		private void LoadPlugin(string pluginInfo)
		{
			string[] parts = pluginInfo.Split(new char[] { ',' });

			try
			{
				ObjectHandle handle = Activator.CreateInstance(parts[1].Trim(), parts[0].Trim());
				if (handle != null)
				{
					IFinderExtender extender = handle.Unwrap() as IFinderExtender;
					if (extender != null)
					{
						extender.Initialize(host);
						finderExtenders.Add(extender);
						return;
					}
				}
				Trace.WriteLine("Could not load plugin:" + pluginInfo);
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Could not load plugin:" + pluginInfo + "." + ex.ToString());
			}
		}

		public void AddPlugin(IFinderExtender extender)
		{
			this.finderExtenders.Add(extender);
		}

		#region Properties
		public IList FinderExtenders
		{
			get { return this.finderExtenders; }
		}
		#endregion

		public bool ResolveSelection(Point location, object lastKnowSelectedObject, ref object selectedObject)
		{
			foreach ( IFinderExtender extender in this.finderExtenders )
			{
				if ( extender.ResolveSelection(location, lastKnowSelectedObject, ref selectedObject) )
					return true;
			}
			return false;
		}
		public void OnSelectedObjectChanged(object selectedObject)
		{
			foreach (IFinderExtender extender in this.finderExtenders)
			{
				extender.OnSelectedObjectChanged(selectedObject);
			}
		}
	}
}
