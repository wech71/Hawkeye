using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ACorns.Hawkeye.Plugins
{
	public interface IHawkeyeHost
	{
		ToolBarButton AddToolbarButton(string text, Image image, EventHandler eventHandler);
	}
}
