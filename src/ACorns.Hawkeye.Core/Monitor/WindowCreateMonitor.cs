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
using ACorns.Hawkeye.Core.Generate;

namespace ACorns.Hawkeye.Tools.Monitor
{
    /// <summary>
    /// Base class that will be inherited when listening for events.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class WindowCreateMonitor : EventController
    {
        public event CreateWindowHandler WindowCreated;

        protected WindowCreateMonitor()
        {
        }

        public void HandleCreated(string handleType, IntPtr handleValue, int currentHandleCount)
        {
            if (handleType != "Window")
                return;

            Control createdControl = Control.FromHandle(handleValue);

            if (createdControl == null)
                return;

            if (WindowCreated != null)
            {
                WindowCreated(createdControl);
            }
        }
    }
}
