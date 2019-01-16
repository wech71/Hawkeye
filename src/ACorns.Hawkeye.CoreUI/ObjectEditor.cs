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
using System.Windows.Forms;
using ACorns.Hawkeye.Options;
using ACorns.Hawkeye.Tools.Monitor;
using ACorns.Hawkeye.Utils;
using ACorns.Hawkeye.Core.Utils.Hotkey;
using ACorns.Hawkeye.Core.Utils;
using ACorns.Hawkeye.Core.Options;
using ACorns.Hawkeye.Core;
using ACorns.Hawkeye.Plugins;

namespace ACorns.Hawkeye
{
    /// <summary>
    /// Singleton class that takes care of showing the Runtime ObjectEditor form with it's window finder.
    /// To use this you have to enable it:
    /// <code>ACorns.Hawkeye.ObjectEditor.Instance.Enable();</code>
    /// The default shortcut key used it "Control+Shift+R".
    /// If you want to use a different shortcut, change the <see cref="ObjectEditor.HotKey"/>
    /// </summary>
    public sealed class ObjectEditor
    {
        #region Instance

        private static ObjectEditor instance = new ObjectEditor();

        /// <summary>
        /// Singleton instance of the ObjectEditor.
        /// </summary>
        public static ObjectEditor Instance
        {
            get { return instance; }
        }

        #endregion

        private bool hotKeyEnabled = false;
        private HotKeyWatch hotKeyWatch = null;
        private HawkeyeEditor hawkeyeEditor = null;

        private WindowMonitorExtensions windowMonitorExtensions = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectEditor"/> class.
        /// </summary>
        private ObjectEditor()
        {
            try
            {
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            }
            catch (Exception) { }

            HawkeyeTraceListener.StartListening();
        }

        /// <summary>
        /// Enable the Object Editor to listen for the shortcut key.
        /// </summary>
        /// <returns></returns>
        public void EnableHotKey()
        {
            if (hotKeyEnabled) return; // already enabled.

            string hotKey = CoreApplicationOptions.Instance.HotKey;
            if (string.IsNullOrEmpty(hotKey)) return;

            hotKeyWatch = new HotKeyWatch();
            if (!hotKeyWatch.RegisterHotKey(hotKey)) return; // didn't work

            hotKeyWatch.HotKeyPressed += new EventHandler(hotKeyWatch_HotKeyPressed);
            hotKeyEnabled = true;
            Trace.WriteLine("ObjectEditor's hotkey enabled: " + hotKey);
        }

        ///// <summary>
        ///// Disable the object editor.
        ///// </summary>
        //public void DisableHotKey()
        //{
        //    if (!hotKeyEnabled) return;

        //    hotKeyWatch.HotKeyPressed -= new EventHandler(hotKeyWatch_HotKeyPressed);
        //    hotKeyWatch.UnregisterKey();
        //    hotKeyWatch = null;
        //    hotKeyEnabled = false;
        //    Trace.WriteLine("ObjectEditor's hotkey disabled.");
        //}

        /// <summary>
        /// Show the object editor form.
        /// </summary>
        public void Show()
        {
            object activeSelectedObject = null;
            if (hawkeyeEditor != null)
            {
                activeSelectedObject = hawkeyeEditor.SelectedObject;

                // Disconnect any possible Open Forms
                foreach (Form frm in Application.OpenForms)
                {
                    if (frm.Owner == hawkeyeEditor)
                        frm.Owner = null;
                }

                hawkeyeEditor.Close();
            }

            hawkeyeEditor = new HawkeyeEditor();
            hawkeyeEditor.Show();
            hawkeyeEditor.Closed += new EventHandler(runtimeEditor_Closed);
            hawkeyeEditor.SelectedObject = activeSelectedObject;
            hawkeyeEditor.Activate();
        }

        ///// <summary>
        ///// Show the editor with the selectedObject selected.
        ///// </summary>
        ///// <param name="selectObject">The object to be selected in the editor</param>
        //public void Show(object selectObject)
        //{
        //    if (hawkeyeEditor != null) hawkeyeEditor.Close();

        //    hawkeyeEditor = new HawkeyeEditor();
        //    hawkeyeEditor.Show();
        //    hawkeyeEditor.Closed += new EventHandler(runtimeEditor_Closed);
        //    hawkeyeEditor.SelectedObject = selectObject;
        //}

        internal HawkeyeEditor ActiveEditor
        {
            get { return this.hawkeyeEditor; }
            set { this.hawkeyeEditor = value; }
        }

        //public object SelectedObject
        //{
        //    get { return hawkeyeEditor.SelectedObject; }
        //    set { hawkeyeEditor.SelectedObject = value; }
        //}

        public Form CreateEditor()
        {
            if (hawkeyeEditor != null) hawkeyeEditor.Close();

            hawkeyeEditor = new HawkeyeEditor();
            return hawkeyeEditor;
        }


        public void EnableDynamicExtenders()
        {
            // Start Message Monitor
            CoreApplicationOptions.Instance.AutomaticExtenderMonitorAndAttach = true;
            CoreApplicationOptions.Instance.AllowInjectInOtherProcesses = false;
            CoreApplicationOptions.Instance.AllowSelectOwnedObjects = false;
            CoreApplicationOptions.Instance.SaveGeneratedAssembly = false;

            windowMonitorExtensions = new WindowMonitorExtensions();
        }

        public void AttachTo(IntPtr windowHandle, IntPtr origHandle)
        {
            EditorHawkeyeHook hook = new EditorHawkeyeHook();
            hook.Hook(windowHandle, origHandle);
        }

        private void hotKeyWatch_HotKeyPressed(object sender, EventArgs e) { Show(); }

        private void runtimeEditor_Closed(object sender, EventArgs e) { hawkeyeEditor = null; }

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Trace.WriteLine("Application_ThreadException:" + e.Exception.ToString(), "Hawkeye");
            if (e.Exception is NullReferenceException)
            {
                if (hawkeyeEditor != null && hawkeyeEditor.Disposing)
                    return;
            }

            MessageBox.Show("There was an unhandled exception:" + e.Exception.ToString(), 
                SystemUtils.ApplicationName);
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            Trace.WriteLine("Exiting:" + SystemUtils.ApplicationName, "Hawkeye");
        }
    }
}