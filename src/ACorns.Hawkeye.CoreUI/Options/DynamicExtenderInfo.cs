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
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Windows.Forms;
using ACorns.Hawkeye.Public;
using ACorns.Hawkeye.Utils;
using ACorns.Hawkeye.Core.Options;

namespace ACorns.Hawkeye.Options
{
    /// <summary>
    /// Summary description for DynamicExtenderInfo.
    /// </summary>
    internal class DynamicExtenderInfo
    {
        private readonly string className;
        private readonly string name;
        private readonly Type type;
        private readonly string assemblyName;

        public DynamicExtenderInfo(string name, string className, string assemblyName)
        {
            this.className = className;
            this.name = name;
            this.assemblyName = assemblyName;
        }

        public DynamicExtenderInfo(string name, Type type)
        {
            this.className = name;
            this.type = type;
        }

        public string Name
        {
            get { return name; }
        }

        public string ClassName
        {
            get { return className; }
        }

        public string AssemblyName
        {
            get { return assemblyName; }
        }

        public void CreateExtender(object target)
        {
            try
            {
                IDynamicSubclass subclass;

                if (type == null)
                {
                    string fullName = Path.Combine(CoreApplicationOptions.Instance.FolderName, assemblyName) + ".dll";
                    Assembly.LoadFrom(fullName);

                    ObjectHandle handle = Activator.CreateInstance(assemblyName, className);
                    subclass = handle.Unwrap() as IDynamicSubclass;
                }
                else
                {
                    subclass = Activator.CreateInstance(type) as IDynamicSubclass;
                }

                subclass.Attach(target);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Could not create extender on:" + target + ":\r\n" + ex.ToString());
                //MessageBox.Show("Could not create extender:\r\n" + ex.ToString(), SystemUtils.FullApplicationName);
            }
        }
    }
}
