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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ACorns.Hawkeye.Utils;
using ACorns.Hawkeye.Utils.Generate;
using ACorns.Hawkeye.Utils.Menus;
using ACorns.Hawkeye.Core.Generate;

namespace ACorns.Hawkeye.Tabs.Events
{
	/// <summary>
	/// InstanceEventsTab 
	/// </summary>
	internal class InstanceEventsTab : PropertyTab, ICustomMenuHandler, IPropertyGridTab
	{
		private XPropertyGrid propertyGrid;
		private CustomMenuItem invokeMenuItem;
		private CustomMenuItem clearAllListenersForThisObject;
		private CustomMenuItem clearAllListeners;

		public InstanceEventsTab()
		{
		}

		public override Bitmap Bitmap
		{
			get 
			{
				return SystemUtils.LoadBitmap("Tabs.Events.bmp");
			}
		}

		public override string TabName
		{
			get { return "Events"; }
		}

		public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
		{
			return GetProperties(null, component, attributes);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
		{
			EventInfoConverter eventConverter = component as EventInfoConverter;
			if (eventConverter != null)
			{
				return eventConverter.GetProperties();
			}
			else
			{
				return DescriptorUtils.GetInstanceEvents(component);
			}
		}

		#region ICustomMenuHandler Members

		public void RegisterMenuItems(System.Windows.Forms.ContextMenuStrip contextMenu)
		{
			invokeMenuItem = new CustomMenuItem(this, "Invoke", new EventHandler(OnInvokeEvent));
			clearAllListenersForThisObject = new CustomMenuItem(this, "Remove Hawkeye's listeners for this object", new EventHandler(ClearListenersForThisObject));
			clearAllListeners = new CustomMenuItem(this, "Remove ALL Hawkeye's Listeners", new EventHandler(ClearListeners));

			contextMenu.Items.Insert(1, invokeMenuItem);
            contextMenu.Items.Add(clearAllListenersForThisObject);
            contextMenu.Items.Add(clearAllListeners);

			invokeMenuItem.Enabled = false;
			clearAllListenersForThisObject.Enabled = true;
			clearAllListeners.Enabled = true;
		}

		private void ClearListeners(object sender, EventArgs e)
		{
			EventControllers.Instance.RemoveAll();
			
			this.propertyGrid.Refresh();
		}

		private void ClearListenersForThisObject(object sender, EventArgs e)
		{
			EventControllers.Instance.RemoveAll(propertyGrid.SelectedObject);

			this.propertyGrid.Refresh();
		}

		private void OnInvokeEvent(object sender, EventArgs e)
		{
			IDynamicInvoke dynInvoke = InvokableActiveItem();
			if ( dynInvoke != null )
			{
				dynInvoke.DynamicInvoke();
			}
		}

		private IDynamicInvoke InvokableActiveItem()
		{
			// grab the active event
			GridItem selectedItem = propertyGrid.SelectedGridItem;
			if ( selectedItem != null )
			{
				PropertyDescriptor descriptor = selectedItem.PropertyDescriptor;
				return descriptor as IDynamicInvoke;	
			}
			return null;
		}

		#endregion

		#region IPropertyGridTab Members
		XPropertyGrid IPropertyGridTab.PropertyGrid
		{
			get
			{
				return propertyGrid;
			}
			set
			{
				this.propertyGrid = value;
				if ( propertyGrid != null )
				{
					propertyGrid.SelectedGridItemChanged += new SelectedGridItemChangedEventHandler(propertyGrid_SelectedGridItemChanged);
				}
			}
		}

		#endregion

		private void propertyGrid_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
		{
			if ( propertyGrid.SelectedTab == this )
			{
				IDynamicInvoke dynInvoke = InvokableActiveItem();
				this.invokeMenuItem.Enabled = dynInvoke != null;
			}
		}
	}
}