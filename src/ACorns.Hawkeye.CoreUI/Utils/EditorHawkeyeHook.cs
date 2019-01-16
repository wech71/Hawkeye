using System;
using System.Collections.Generic;
using System.Text;
using ACorns.Hawkeye.Utils;

namespace ACorns.Hawkeye.Core.Utils
{
	public class EditorHawkeyeHook : HawkeyeHook
	{
		public override void OnInstallHook(byte[] data)
		{
			base.OnInstallHook(data);

			ObjectEditor.Instance.EnableHotKey();

			IntPtr originalHawkeyeWindow = (IntPtr)BitConverter.ToInt32(data, 0);
			IntPtr spyWindow = (IntPtr)BitConverter.ToInt32(data, 4);

			ObjectEditor.Instance.Show();
			ObjectEditor.Instance.ActiveEditor.SelectedWindowHandle = spyWindow;

			// close original window
			NativeUtils.SendMessage(originalHawkeyeWindow, 0x0010, IntPtr.Zero, IntPtr.Zero); // close
		}
	}
}
