using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using ACorns.Hawkeye.Core.Utils.Accessors;

namespace ACorns.Hawkeye.Tabs.ProcessInfo
{
	public class ApplicationInfo
	{
		private GCDetails gcInfo = new GCDetails();
		private SqlConnectionPoolInfo connectionPool;

		[Category("Application")]
		public object CurrentApplication
		{
			get
			{
				return Application.ProductName;
			}
		}

		[Category("Thread")]
		public Context CurrentContext
		{
			get
			{
				return Thread.CurrentContext;
			}
		}

		[Category("Domain")]
		public AppDomain CurrentDomain
		{
			get
			{
				return AppDomain.CurrentDomain;
			}
		}

		[Category("Thread")]
		public IPrincipal CurrentPrincipal
		{
			get
			{
				return Thread.CurrentPrincipal;
			}
		}

		[Category("Process")]
		public Process CurrentProcess
		{
			get
			{
				return Process.GetCurrentProcess();
			}
		}

		[Category("Thread")]
		public Thread CurrentThread
		{
			get
			{
				return Thread.CurrentThread;
			}
		}

		private FieldAccesor systemEventsAcc = new FieldAccesor(typeof(SystemEvents), "systemEvents");
		public object SystemEvents
		{
			get { return systemEventsAcc.Get(); }
		}

		public GCDetails GCInfo
		{
			get
			{
				return this.gcInfo;
			}
		}

		public SqlConnectionPoolInfo ConnectionPools
		{
			get
			{
				if (connectionPool == null)
					connectionPool = new SqlConnectionPoolInfo();
				return connectionPool;
			}
		}

		public class GCDetails
		{
			private string Format(long value)
			{
				return value.ToString("###,##0");
			}

			public override string ToString()
			{
				return ("{Mem:" + this.TotalMemoryKb + " Max:" + this.WorkingSetKb + "}");
			}

			[RefreshProperties(RefreshProperties.All), Description("Gets the maximum allowable working set size for the associated process.")]
			public int MaxWorkingSetKb
			{
				get
				{
					return (Process.GetCurrentProcess().MaxWorkingSet.ToInt32() / 0x400);
				}
				set
				{
					Process.GetCurrentProcess().MaxWorkingSet = new IntPtr(value * 0x400);
				}
			}

			[Description("Gets or Sets the Minimum allowable working set size for the associated process."), RefreshProperties(RefreshProperties.All)]
			public int MinWorkingSetKb
			{
				get
				{
					return (Process.GetCurrentProcess().MinWorkingSet.ToInt32() / 0x400);
				}
				set
				{
					Process.GetCurrentProcess().MinWorkingSet = new IntPtr(value * 0x400);
				}
			}

			[RefreshProperties(RefreshProperties.All), Description("Retrieves the number of bytes currently thought to be allocated (after a collect)")]
			public string TotalMemoryKb
			{
				get
				{
					return this.Format(GC.GetTotalMemory(true) / ((long)0x400));
				}
			}

			[Description("Gets the amount of physical memory allocated for the associated process.")]
			public string WorkingSetKb
			{
				get
				{
					return this.Format(Process.GetCurrentProcess().WorkingSet64 / ((long)0x400));
				}
			}
		}
	}
}

