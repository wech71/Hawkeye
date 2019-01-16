using ACorns.Hawkeye.Core.Utils;
using System;
using System.Diagnostics;
using System.IO;
using ACorns.Hawkeye.Core.Options;

namespace ACorns.Hawkeye.Core.Utils
{
	public class HawkeyeTraceListener : TraceListener
	{
        private static bool listening = false;

		private readonly string LOG_FILE_NAME = @"\Hawkeye.log";
		private readonly string logFileName;

        public static void StartListening()
        {
            if (listening)
                return;
            Trace.Listeners.Add( new HawkeyeTraceListener() );
			Trace.WriteLine("Starting ..." );
            listening = true;
        }

		private HawkeyeTraceListener()
		{
            logFileName = CoreApplicationOptions.Instance.FolderName + LOG_FILE_NAME;
			try
			{
				if (File.Exists(this.logFileName))
				{
					FileInfo info = new FileInfo(this.logFileName);
					if (info.Length > 0x7d000)
					{
						File.Delete(this.logFileName);
					}
				}
			}
			catch
			{
			}
		}

		public override void Write(string message)
		{
			try
			{
				using (StreamWriter writer = new StreamWriter(this.logFileName, true))
				{
					if (writer != null)
					{
						writer.Write(message);
					}
				}
			}
			catch
			{
			}
		}

		public override void WriteLine(string message)
		{
			try
			{
				using (StreamWriter writer = new StreamWriter(this.logFileName, true))
				{
					if (writer != null)
					{
						writer.WriteLine(message);
					}
				}
			}
			catch
			{
			}
		}
	}
}

