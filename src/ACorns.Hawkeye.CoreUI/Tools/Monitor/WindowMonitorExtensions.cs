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
using System.Windows.Forms;
using ACorns.Hawkeye.Options;
using ACorns.Hawkeye.Utils.Generate;
using ACorns.Hawkeye.Core.Generate;

namespace ACorns.Hawkeye.Tools.Monitor
{
    /// <summary>
    /// Handler for monitoring new windows
    /// </summary>
    internal class WindowMonitorExtensions
    {
        public WindowMonitorExtensions()
        {
            WindowMonitor.Instance.StartMonitoring();
            WindowMonitor.Instance.WindowCreated += new CreateWindowHandler(Instance_WindowCreated);
        }

        private void Instance_WindowCreated(Control createdControl)
        {
            if (createdControl == null)
                return;

            Type createdControlType = createdControl.GetType();
            if (ApplicationOptions.Instance.HasDynamicExtenders(createdControlType))
            {
                DynamicExtenderInfo extender = ApplicationOptions.Instance.GetDynamicExtender(createdControlType);
                if (extender != null) extender.CreateExtender(createdControl);
                // TODO: log if extender == null
            }
        }
    }
}
