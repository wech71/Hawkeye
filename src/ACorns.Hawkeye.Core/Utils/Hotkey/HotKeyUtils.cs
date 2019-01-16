using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ACorns.Hawkeye.Core.Utils.Hotkey
{
	internal sealed class HotKeyUtils
	{
		private HotKeyUtils()
		{
		}

		private static FsKeyModifiers CheckModifier(Keys key, Keys keyModifier, FsKeyModifiers fsModifier)
		{
			if ((key & keyModifier) == keyModifier)
			{
				return fsModifier;
			}
			return FsKeyModifiers.None;
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern int GlobalAddAtom(string lpString);
		[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		internal static extern ushort GlobalDeleteAtom(int nAtom);
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool RegisterHotKey(IntPtr hWnd, int keyId, FsKeyModifiers fsModifiers, Keys vk);
		public static bool RegisterKey(Control parentForm, int hotKeyId, string keys)
		{
			if (keys == null)
			{
				keys = "";
			}
			Keys key = Keys.None;
			try
			{
				if (keys.Trim().Length == 0)
				{
					Trace.WriteLine("keys is empty!");
				}
				else
				{
					string[] textArray = keys.Split(new char[] { '+' });
					bool flag = true;
					foreach (string text in textArray)
					{
						if (flag)
						{
							key = (Keys)Enum.Parse(typeof(Keys), text);
							flag = false;
						}
						else
						{
							key |= (Keys)Enum.Parse(typeof(Keys), text);
						}
					}
				}
			}
			catch (Exception exception)
			{
				Trace.WriteLine(exception.Message);
			}
			if (key == Keys.None)
			{
				return false;
			}
			return RegisterKey(parentForm, hotKeyId, key);
		}

		public static bool RegisterKey(Control parentForm, int id, Keys key)
		{
			FsKeyModifiers fsModifiers = FsKeyModifiers.None;
			fsModifiers |= CheckModifier(key, Keys.Control, FsKeyModifiers.Control);
			fsModifiers |= CheckModifier(key, Keys.Alt, FsKeyModifiers.Alt);
			fsModifiers |= CheckModifier(key, Keys.Shift, FsKeyModifiers.Shift);
			fsModifiers |= CheckModifier(key, Keys.LWin, FsKeyModifiers.Windows);
			fsModifiers |= CheckModifier(key, Keys.RWin, FsKeyModifiers.Windows);
			return RegisterHotKey(parentForm.Handle, id, fsModifiers, key & Keys.KeyCode);
		}

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
		public static bool UnregisterKey(Control parentForm, int hotKeyId)
		{
			return UnregisterHotKey(parentForm.Handle, hotKeyId);
		}

		[Flags]
		private enum FsKeyModifiers
		{
			Alt = 1,
			Control = 2,
			None = 0,
			Shift = 4,
			Windows = 8
		}
	}
}

