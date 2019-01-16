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
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace ACorns.Hawkeye.Core.Options
{
	/// <summary>
	/// Summary description for CoreApplicationOptions.
	/// </summary>
	public sealed class CoreApplicationOptions
	{
		#region Instance

        private static CoreApplicationOptions instance = null;

		/// <summary>
		/// Singleton instance of the ApplicationOptions.
		/// </summary>
		public static CoreApplicationOptions Instance
		{
			get 
            {
                if (instance == null)
                {
                    instance = new CoreApplicationOptions();

                    try
                    {
                        instance.ReadSettings();
                    }
                    catch (Exception ex)
                    {
                        Trace.Write(string.Format("Could not read application settings: {0}", ex));
                    }
                }

                return instance; 
            }
		}

		#endregion

		private bool allowSelectOwnedObjects = true; // why should this be disabled?
		private bool allowInjectInOtherProcesses = true;
		private bool isInjected = false;
		private bool injectBasedOnRuntimeVersion = false;
		private bool saveGeneratedAssembly = false;		
		private bool automaticExtenderMonitorAndAttach = true;
		private bool advancedFeatures = true;
        private string hotKey = "Control+Shift+R";

        private string appExeName = "ACorns.Hawkeye.exe";

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreApplicationOptions"/> class.
        /// </summary>
		private CoreApplicationOptions() { }

        public string HotKey
        {
            get { return hotKey; }
        }

		public bool AllowSelectOwnedObjects
		{
			get { return allowSelectOwnedObjects; }
			set { allowSelectOwnedObjects = value; }
		}

		public bool AllowInjectInOtherProcesses
		{
			get { return allowInjectInOtherProcesses; }
			set { allowInjectInOtherProcesses = value; }
		}

		public bool InjectBasedOnRuntimeVersion
		{
			get { return injectBasedOnRuntimeVersion; }
			set { injectBasedOnRuntimeVersion = value; }
		}

		public bool IsInjected
		{
			get { return isInjected; }
			set { isInjected = value; }
		}

		public bool SaveGeneratedAssembly
		{
			get { return saveGeneratedAssembly; }
			set { saveGeneratedAssembly = value; }
		}

		public bool AutomaticExtenderMonitorAndAttach
		{
			get { return automaticExtenderMonitorAndAttach; }
			set { automaticExtenderMonitorAndAttach = value; }
		}

		public bool AdvancedFeatures
		{
			get { return advancedFeatures; }
			set { advancedFeatures = value; }
		}

		public string MutexName
		{
			get { return "Hawkeye.Inject." + Process.GetCurrentProcess().Id; }
		}

        public string AppExeName
        {
            get { return this.appExeName; }
            set { this.appExeName = value; }
        }

        public string FolderName
        {
            get
            {
#if X64
				return Path.Combine(
                    Path.GetDirectoryName(typeof(CoreApplicationOptions).Assembly.Location), "..");
#else
                return Path.GetDirectoryName(typeof(CoreApplicationOptions).Assembly.Location);
#endif
            }
        }

        private void ReadSettings()
        {
            string filename = Path.Combine(instance.FolderName, "settings.xml");
            if (!File.Exists(filename))
            {
                Trace.WriteLine(string.Format("Settings file {0} could not be found; using defaults"), filename);
                return;
            }

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(filename);
            foreach (XmlElement xe in xdoc.DocumentElement)
            {
                if (xe.Name == "hotKey")
                    instance.hotKey = xe.Attributes["value"].Value;
            }
        }
	}
}
