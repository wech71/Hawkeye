using System;
using System.Windows.Forms;

using ACorns.Hawkeye.Core.Utils;
using ACorns.Hawkeye.Core.Utils.CommandLine;
using ACorns.Hawkeye.Core.Options;

namespace ACorns.Hawkeye
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            if (!TryUpdate())
            {
                Arguments arguments = new Arguments(args);
                string windowHandleStr = arguments["targetwndhandle"];
                string origWndHandleStr = arguments["origwndhandle"];

                if (!(string.IsNullOrEmpty(windowHandleStr) || string.IsNullOrEmpty(origWndHandleStr)))
                {
                    IntPtr targetHandle = new IntPtr(int.Parse(windowHandleStr));
                    IntPtr origHandle = new IntPtr(int.Parse(origWndHandleStr));
                    ObjectEditor.Instance.AttachTo(targetHandle, origHandle);
                }
                else
                {
                    HawkeyeTraceListener.StartListening();
                    Application.EnableVisualStyles();

                    Form editor = ObjectEditor.Instance.CreateEditor();

                    Application.Run(editor);
                }
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
			Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show("Unhandled exception:\r\n" + ex.StackTrace, "Exception");
        }

        private static bool TryUpdate()
        {
            //TODO: auto-update feature
            return false;
        }
    }
}