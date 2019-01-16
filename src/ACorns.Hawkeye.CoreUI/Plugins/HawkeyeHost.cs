using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using ACorns.Hawkeye.Tabs.Toolbar;

namespace ACorns.Hawkeye.Plugins
{
	internal class HawkeyeHost : IHawkeyeHost
	{
		private ToolBar toolbar;
		public HawkeyeHost(ToolBar toolbar)
		{
			this.toolbar = toolbar;
		}

		#region IHawkeyeHost Members

		public ToolBarButton AddToolbarButton(string text, Image image, EventHandler eventHandler)
		{
			return ToolbarUtils.AddButton(-1, toolbar, text, image, eventHandler);
		}

		#endregion
	}
}
