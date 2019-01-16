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
using ACorns.Hawkeye.Core.Options;

namespace ACorns.Hawkeye.Options
{
	/// <summary>
	/// Summary description for ApplicationOptions.
	/// </summary>
	internal sealed class ApplicationOptions
	{
		#region Instance

		private static readonly ApplicationOptions instance = new ApplicationOptions();

		/// <summary>
		/// Singleton instance of the ApplicationOptions.
		/// </summary>
		public static ApplicationOptions Instance
		{
			get { return instance; }
		}

		#endregion

		private int maxIEnumerableChildsToShow = 50;
		private bool showBalloonHelp = false;
		private Hashtable dynamicExtenders = new Hashtable();

		private ApplicationOptions()
		{
#if X64
			showBalloonHelp = false;
#endif
            DiscoverExtenders();
		}

		public int MaxIEnumerableChildsToShow
		{
			get { return maxIEnumerableChildsToShow; }
			set { maxIEnumerableChildsToShow = value; }
		}

		public bool ShowBalloonHelp
		{
			get { return showBalloonHelp; }
			set { showBalloonHelp = value; }
        }

        #region Extenders Management

        public bool HasDynamicExtenders(Type type)
		{
			return dynamicExtenders.ContainsKey(type.FullName);
		}
		
		public void RegisterExtenderInfo(string extendedClass, string extenderName, string extenderClassName, string extenderAssembly)
		{
			dynamicExtenders.Add(extendedClass, 
                new DynamicExtenderInfo(extenderName, extenderClassName, extenderAssembly));
        }

        public DynamicExtenderInfo GetDynamicExtender(Type type)
        {
            return dynamicExtenders[type.FullName] as DynamicExtenderInfo;
        }

        private void DiscoverExtenders()
        {
            // TODO --> app.config
            const string searchBoxExtenderAssembly = "ACorns.PropertyGridExtender";
            const string searchBoxExtenderClass = "ACorns.PropertyGridExtender.SearchPropGrid";
            RegisterExtenderInfo("System.Windows.Forms.PropertyGrid", "Search Box",
                searchBoxExtenderClass, searchBoxExtenderAssembly);
            RegisterExtenderInfo("System.Windows.Forms.PropertyGridInternal.PropertyGridView", "Search Box",
                searchBoxExtenderClass, searchBoxExtenderAssembly);

            const string snapshotExtenderAssembly = "Acorns.Hawkeye.DynamicExtender.Snapshot";
            const string snapshotExtenderClass = "Acorns.Hawkeye.DynamicExtender.Snapshot.SnapshotExtender";
            RegisterExtenderInfo("ACorns.Hawkeye.Public.IHawkeyeEditor", "Snapshot",
                snapshotExtenderClass, snapshotExtenderAssembly);

            //const string tfsExtendersAssembly = "ACorns.Hawkeye.DynamicExtender.TFSQueryResult";
            //const string tfsQuickSearchExtenderClass = "ACorns.Hawkeye.DynamicExtender.TFSQueryResult.TFSResultExtender";
            //const string tfsGotoLineExtenderClass = "ACorns.Hawkeye.DynamicExtender.TFSQueryResult.VSTSUnitTestGotoException";
            //RegisterExtenderInfo("Microsoft.VisualStudio.TeamFoundation.WorkItemTracking.ResultView", "TFS Quick Search",
            //    tfsQuickSearchExtenderClass, tfsExtendersAssembly);
            //RegisterExtenderInfo("Microsoft.VisualStudio.TestTools.Vsip.AutoSizedTextBox", "TFS UnitTest Goto Line",
            //    tfsGotoLineExtenderClass, tfsExtendersAssembly);
        }

        #endregion
    }
}
