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
using ACorns.Hawkeye.Utils;
using ACorns.Hawkeye.Injector;
using System.Threading;
using System.IO;
using ACorns.Hawkeye.Core.Options;
using ACorns.Hawkeye.Core.Utils;
//#if HOOK

namespace ACorns.Hawkeye.Core
{
	/// <summary>
	/// Helper used to hook into other processed.
	/// </summary>
	public class HawkeyeHook
#if HOOK
: IHookInstall
#endif
	{
		private Mutex processInjectedMutex = null;

		public HawkeyeHook()
		{
		}

		public bool Hook(IntPtr targetWindowHandle, IntPtr thisHandle)
		{
			if ( !CoreApplicationOptions.Instance.AllowInjectInOtherProcesses )
				return false;

			try
			{
				bool createdNew;
				processInjectedMutex = new Mutex(true, CoreApplicationOptions.Instance.MutexName, out createdNew);
				if (!createdNew)
				{
					// no other hawkeye is injected
					throw new ApplicationException("Hawkeye is already injected in remote process.");
				}
			}
			finally
			{
				if (processInjectedMutex != null)
				{
					processInjectedMutex.Close();
				}
			}

			try
			{
				if (targetWindowHandle != IntPtr.Zero)
				{
					IntPtr processId;
					IntPtr threadId;
					NativeUtils.GetWindowThreadAndProcess(targetWindowHandle, out threadId, out processId);

					int panelHandle = thisHandle.ToInt32();
					int targetHandle = targetWindowHandle.ToInt32();

					byte[] b1 = BitConverter.GetBytes(panelHandle);
					byte[] b2 = BitConverter.GetBytes(targetHandle);

					byte[] data = new byte[b1.Length + b2.Length];
					Array.Copy(b1, data, b1.Length);
					Array.Copy(b2, 0, data, b1.Length, b2.Length);

					string assemblyToRegister = this.GetType().Assembly.Location;
					string classToHook = this.GetType().FullName;

					// Register to Pickup an idle message from the queue
					// try to read the runtime version of the destination process.
					string className = NativeUtils.GetClassName(targetWindowHandle);
					if ( className != null )
					{
						if (CoreApplicationOptions.Instance.InjectBasedOnRuntimeVersion)
						{
							if ( className.StartsWith("WindowsForms10") )
							{
								assemblyToRegister = assemblyToRegister.Replace("2", "1");	// use .Net 1.1
								Trace.WriteLine("Target window is a .Net 1.1. Loading .Net1.1 RuntimeEditor");
							}
							else if ( className.StartsWith("WindowsForms11") )
							{
								assemblyToRegister = assemblyToRegister.Replace("1", "2");	// use .Net 2.0
								Trace.WriteLine("Target window is a .Net 2.0. Loading .Net2.0 RuntimeEditor");
							}
						}
					}

#if HOOK
					bool x64Process = NativeUtils.IsProcessX64(processId);
					bool thisIsx64 = NativeUtils.IsCurrentProcessX64;

					if (x64Process != thisIsx64)
					{	// we're in trouble ;) We need to change the x version of Hawkeye
						string targetCommandLine = String.Format("/targetwndhandle:{0} /origwndhandle:{1}", targetWindowHandle.ToInt32(), thisHandle.ToInt32());

                        // BUG: when injected, Application.ExecutablePath returns the injected application path, 
                        // not Hawkeye path!
                        //string folder = Path.GetDirectoryName(Application.ExecutablePath); 
                        string folder = Path.GetDirectoryName(GetType().Assembly.Location);

						string exe = "..\\" + CoreApplicationOptions.Instance.AppExeName;
						if (x64Process) exe = "x64\\" + CoreApplicationOptions.Instance.AppExeName;

						string runExe = Path.Combine(folder, exe);
                        Trace.WriteLine(string.Format("Running {0}", runExe));

                        if (!File.Exists(runExe))
                        {
                            Trace.WriteLine(string.Format("Could not find Hawkeye executable at path {0}", runExe));
                            return false;
                        }
                        
                        Process.Start(runExe, targetCommandLine);
						return true;
					}

					HookHelper.InstallIdleHandler(processId.ToInt32(), threadId.ToInt32(), assemblyToRegister, classToHook, data);
#endif

					// send an idle ;;)
					NativeUtils.SendMessage(targetWindowHandle, 0, IntPtr.Zero, IntPtr.Zero);
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine("RuntimeEditor Hook: Hook:" + ex.ToString(), "Hawkeye");
				return false;
			}
		}

		#region IHookInstall

		public virtual void OnInstallHook(byte[] data)
		{
			CoreApplicationOptions.Instance.IsInjected = true;

			// mark that we are already in the new process
			processInjectedMutex = new Mutex(true, CoreApplicationOptions.Instance.MutexName);
		}

		#endregion
	}
}