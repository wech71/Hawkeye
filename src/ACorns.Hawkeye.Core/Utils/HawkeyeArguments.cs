using System;
using ACorns.Hawkeye.Core.Utils.CommandLine;

namespace ACorns.Hawkeye.Core.Utils
{
	public class HawkeyeArguments
	{
		private Arguments arguments;

		public HawkeyeArguments(string[] args)
		{
			this.arguments = new Arguments(args);
		}

		private IntPtr IntPtrValue(string name)
		{
			string text = this.arguments[name];
			if (!string.IsNullOrEmpty(text))
			{
				return new IntPtr(int.Parse(text));
			}
			return IntPtr.Zero;
		}

		public IntPtr OrigWndHandle
		{
			get
			{
				return this.IntPtrValue("origwndhandle");
			}
		}

		public IntPtr TargetWndHandle
		{
			get
			{
				return this.IntPtrValue("targetwndhandle");
			}
		}
	}
}

