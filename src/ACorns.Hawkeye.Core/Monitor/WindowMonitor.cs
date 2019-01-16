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
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using ACorns.Hawkeye.Core.Generate;
using ACorns.Hawkeye.Core.Options;

namespace ACorns.Hawkeye.Tools.Monitor
{
    public delegate void CreateWindowHandler(Control createdControl);

    /// <summary>
    /// Application wide window creation monitor hook.
    /// </summary>
    public class WindowMonitor
    {
        /// <summary>
        /// A new window was just created.
        /// </summary>
        public event CreateWindowHandler WindowCreated;

        #region Instance

        private static WindowMonitor instance = new WindowMonitor();

        /// <summary>
        /// Singleton instance of the WindowMonitor.
        /// </summary>
        public static WindowMonitor Instance
        {
            get { return instance; }
        }

        #endregion

        private Type handlerCollector;
        //private WindowCreateMonitor handleAddedController;
        private bool monitoring = false;

        private WindowMonitor()
        {
            if (!CoreApplicationOptions.Instance.AutomaticExtenderMonitorAndAttach)
                return;
        }

        public void StartMonitoring()
        {
            if (monitoring)
                return;

            // Too Easy ;) - monitor the HandleAdded 

            // Try to grab the class - we have two chances:
            // 1.1: System.Windows.Forms, System.HandleCollector
            // 2.0: System.Windows.Forms, System.Internal.HandleCollector

            Assembly winFormsAs = typeof(System.Windows.Forms.Form).Assembly;

            handlerCollector = winFormsAs.GetType("System.Internal.HandleCollector");
            if (handlerCollector == null)
                handlerCollector = winFormsAs.GetType("System.HandleCollector");

            EventInfo handleAddedEvent = handlerCollector.GetEvent("HandleAdded", BindingFlags.NonPublic | BindingFlags.Static);

            WindowCreateMonitor handleAddedController = (WindowCreateMonitor)ClassGenerator.Instance.GenerateHandler(handleAddedEvent, typeof(WindowCreateMonitor), "HandleCreated");

            handleAddedController.DynamicAttach(handleAddedEvent, "HandleAdded");

            handleAddedController.WindowCreated += new CreateWindowHandler(handleAddedController_WindowCreated);

            monitoring = true;
        }

        private void handleAddedController_WindowCreated(Control createdControl)
        {
            if (WindowCreated != null)
            {
                WindowCreated(createdControl);
            }
        }
    }
}