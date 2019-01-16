using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Reflector
{
    /// <summary>
    /// Allows an external program to remotely control Reflector.
    /// This class is adapted from RemoteController.cs found in 
    /// .NET Reflector addins project (http://www.codeplex.com/reflectoraddins)
    /// </summary>
	internal static class RemoteController
	{
        private static IntPtr targetWindow = IntPtr.Zero;
        
        #region Win32 Interop

        private const int WM_COPYDATA = 0x4A;

        private delegate bool EnumWindowsCallback(IntPtr hwnd, int lparam);

        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumWindowsCallback callback, int lparam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref CopyDataStruct lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct CopyDataStruct
        {
            public IntPtr Padding;
            public int Size;
            public IntPtr Buffer;

            public CopyDataStruct(IntPtr padding, int size, IntPtr buffer)
            {
                this.Padding = padding;
                this.Size = size;
                this.Buffer = buffer;
            }
        }

        #endregion

        #region Remote controller API

        public static bool LoadAssembly(string fileName)
		{
			return Send("LoadAssembly\n" + fileName);
		}

		public static bool SelectEventDeclaration(string key)
		{
			return Send("SelectEventDeclaration\n" + key);
		}

		public static bool SelectFieldDeclaration(string key)
		{
			return Send("SelectFieldDeclaration\n" + key);
		}

		public static bool SelectMethodDeclaration(string key)
		{
			return Send("SelectMethodDeclaration\n" + key);
		}

		public static bool SelectPropertyDeclaration(string key)
		{
			return Send("SelectPropertyDeclaration\n" + key);
		}

		public static bool SelectTypeDeclaration(string key)
		{
			return Send("SelectTypeDeclaration\n" + key);
		}

        public static bool UnloadAssembly(string fileName)
        {
            return Send("UnloadAssembly\n" + fileName);
        }

        public static bool Available
        {
            get { return Send("Available\n4.0.0.0");  }
        }

        #endregion

		private static bool Send(string message)
		{
            targetWindow = IntPtr.Zero;

            // We can't use a simple FindWindow, because Reflector title 
            // can vary: we must detect its window title starts with a known value; 
            // not simply it is equal to a known value. See the EnumWindow method.
            EnumWindows(new EnumWindowsCallback(EnumWindow), 0);

            if (targetWindow != IntPtr.Zero)
            {
                char[] chars = message.ToCharArray();
                CopyDataStruct data = new CopyDataStruct();
                data.Padding = IntPtr.Zero;
                data.Size = chars.Length * 2;
                data.Buffer = Marshal.AllocHGlobal(data.Size);
                Marshal.Copy(chars, 0, data.Buffer, chars.Length);
                bool result = SendMessage(targetWindow, WM_COPYDATA, IntPtr.Zero, ref data);
                Marshal.FreeHGlobal(data.Buffer);
                return result;
            }

            return false;
		}

        private static bool EnumWindow(IntPtr handle, int lparam)
        {
            StringBuilder titleBuilder = new StringBuilder(256);
            GetWindowText(handle, titleBuilder, 256);

            //TODO: when multiple Reflector windows are found, allow the user to choose which one to use.

            // FIX: issue http://hawkeye.codeplex.com/workitem/7784
            // Reflector 7 (beta) title starts with ".NET Reflector"
            if (titleBuilder.ToString().StartsWith(".NET Reflector"))
                targetWindow = handle;
            else if (titleBuilder.ToString().StartsWith("Red Gate's .NET Reflector"))
                targetWindow = handle;
            else if (titleBuilder.ToString().StartsWith("Lutz Roeder's .NET Reflector"))
                targetWindow = handle;
            
            return true;
        }	
	}
}

