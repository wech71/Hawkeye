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
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;

using ACorns.Hawkeye.Utils.UI;
using ACorns.Hawkeye.Core.UI;

namespace ACorns.Hawkeye.Core.Utils
{
    public class NativeUtils
    {
        public class WINDOWCMD
        {
            public const int GW_HWNDFIRST = 0;
            public const int GW_HWNDLAST = 1;
            public const int GW_HWNDNEXT = 2;
            public const int GW_HWNDPREV = 3;
            public const int GW_OWNER = 4;
            public const int GW_CHILD = 5;
        };

        public class PROCESSACCESS
        {
            public const int PROCESS_QUERY_INFORMATION = 0x0400;
        }

        public class WM
        {
            public const int WM_CREATE = 0x0001;
        }

        public class RDW
        {
            public const uint RDW_INVALIDATE = 0x0001;
            public const uint RDW_INTERNALPAINT = 0x0002;
            public const uint RDW_ERASE = 0x0004;

            public const uint RDW_VALIDATE = 0x0008;
            public const uint RDW_NOINTERNALPAINT = 0x0010;
            public const uint RDW_NOERASE = 0x0020;

            public const uint RDW_NOCHILDREN = 0x0040;
            public const uint RDW_ALLCHILDREN = 0x0080;

            public const uint RDW_UPDATENOW = 0x0100;
            public const uint RDW_ERASENOW = 0x0200;

            public const uint RDW_FRAME = 0x0400;
            public const uint RDW_NOFRAME = 0x0800;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr ChildWindowFromPoint(IntPtr hWndParent, POINT Point);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreatePen(int fnPenStyle, int nWidth, uint crColor);

        public delegate bool IsWow64ProcessHandler(IntPtr processHandle, out bool is64Process);

        public delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);

        [DllImport("user32.dll")]
        public static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hwnd, ref RECT rc);

        [DllImport("user32.dll")]
        public static extern bool EnumThreadWindows(int threadId, WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hwnd, out IntPtr processID);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int width, int height, bool repaint);

        public static Point NativeScreenToClient(IntPtr window, Point originalPoint)
        {
            POINT point = new POINT(originalPoint.X, originalPoint.Y);
            if (NativeUtils.ScreenToClient(window, ref point))
                return new Point(point.x, point.y);
            return Point.Empty;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lpRect, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern IntPtr GetClientRect(IntPtr hwnd, ref RECT rc);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // FIX: http://hawkeye.codeplex.com/workitem/7791
        [DllImport("kernel32.dll")]
        public static extern int GetSystemWow64Directory(
            [In, Out] StringBuilder lpBuffer, [MarshalAs(UnmanagedType.U4)] uint size);

        private static bool wow64DirectoryChecked = false;
        private static bool wow64DirectoryExists = false;

        public static string GetWindowText(IntPtr hwnd)
        {
            int bufLen = GetWindowTextLength(hwnd) + 1;

            StringBuilder buffer = new StringBuilder(bufLen);
            GetWindowText(hwnd, buffer, bufLen);

            return buffer.ToString();
        }

        public static string GetClassName(IntPtr hwnd)
        {
            StringBuilder buffer = new StringBuilder(256);
            GetClassName(hwnd, buffer, buffer.Capacity);
            return buffer.ToString();
        }

        public static bool IsTargetInDifferentProcess(IntPtr targetWindowHandle)
        {
            IntPtr targetProcessId;
            IntPtr targetThreadId = GetWindowThreadProcessId(targetWindowHandle, out targetProcessId);

            if (targetProcessId == IntPtr.Zero)
                return true;

            IntPtr thisProcessId = new IntPtr(Process.GetCurrentProcess().Id);
            return thisProcessId != targetProcessId;
        }

        private static IsWow64ProcessHandler isWow64Process;
        private static bool isWow64ProcessChecked = false;

        private static IsWow64ProcessHandler IsWow64Process
        {
            get
            {
#if NET2
                if (isWow64ProcessChecked) return isWow64Process;

                IntPtr kernel32Module = GetModuleHandle("kernel32");
                IntPtr isWow64ProcessPtr = GetProcAddress(kernel32Module, "IsWow64Process");

                if (isWow64ProcessPtr != IntPtr.Zero) isWow64Process =
                    (IsWow64ProcessHandler)Marshal.GetDelegateForFunctionPointer(isWow64ProcessPtr, typeof(IsWow64ProcessHandler));

                isWow64ProcessChecked = true;
                return isWow64Process;
#else
                return null;
#endif
            }
        }

        public static bool IsCurrentProcessX64
        {
            get
            {
                //return IsProcessX64(new IntPtr(Process.GetCurrentProcess().Id));
                // Here is a better test (at least for the running process)
                return Marshal.SizeOf(typeof(IntPtr)) == 8; // 8 bytes = 64 bits
            }
        }

        // FIX: http://hawkeye.codeplex.com/workitem/7791
        private static bool IsWindowsX64
        {
            get
            {
                try
                {
                    if (!wow64DirectoryChecked)
                    {
                        StringBuilder builder = new StringBuilder(256);
                        int result = GetSystemWow64Directory(builder, (uint)256);
                        wow64DirectoryChecked = true;
                        wow64DirectoryExists = result != 0; 
                    }
                    
                    // we have a Wow64 path ==> we run an x64 OS
                    return wow64DirectoryExists;
                }
                catch (Exception ex)
                {
                    // For debugging purpose
                    Exception debugException = ex;

                    // Well if GetSystemWow64Directory failed, let's suppose we 
                    // are running an x86 box; anyway other parts of the code may
                    // throw as well...
                    return false; 
                }
            }
        }

        public static bool IsProcessX64(IntPtr processId)
        {
            // FIX: http://hawkeye.codeplex.com/workitem/7791
            // First determine whether we are running an x86 or x64 OS
            if (!IsWindowsX64) return false;

            if (IsWow64Process == null) return false;

            int procId = processId.ToInt32();
            IntPtr processHandle = OpenProcess(PROCESSACCESS.PROCESS_QUERY_INFORMATION, false, procId);

            bool is64Process = false;
            try
            {
                IsWow64Process(processHandle, out is64Process);
                int lastError = Marshal.GetLastWin32Error();
                if (lastError != 0) Trace.WriteLine("IsWow64Process LastError:" + lastError);
            }
            finally { CloseHandle(processHandle); }

            return !is64Process;
        }

        public static IntPtr GetProcessForWindow(IntPtr windowHandle)
        {
            IntPtr processId;
            IntPtr threadId = NativeUtils.GetWindowThreadProcessId(windowHandle, out processId);
            return processId;
        }

        public static void GetWindowThreadAndProcess(IntPtr windowHandle, out IntPtr threadId, out IntPtr processId)
        {
            threadId = NativeUtils.GetWindowThreadProcessId(windowHandle, out processId);
        }
    }
}