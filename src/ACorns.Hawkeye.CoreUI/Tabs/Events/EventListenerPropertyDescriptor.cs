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
using System.Collections;
using System.ComponentModel;
using System.Reflection;

using ACorns.Hawkeye.Utils;
using ACorns.Hawkeye.Tools.Reflector;

namespace ACorns.Hawkeye.Tabs.Events
{
	internal class EventListenerPropertyDescriptor : AbstractPropertyDescriptor, IRealValueHolder, IShowSourceCodeHandler, IDynamicInvoke
	{
		private EventPropertyDescriptor eventDescriptor;
		private Delegate handler;

		public EventListenerPropertyDescriptor(EventPropertyDescriptor eventDescriptor, Delegate handler)
			: base(handler.Method.Name)
		{
			this.eventDescriptor = eventDescriptor;
			this.handler = handler;
		}

		protected override void FillAttributes(IList attributeList)
		{
			base.FillAttributes(attributeList);
			attributeList.Add(new RefreshPropertiesAttribute(RefreshProperties.Repaint));
		}

		#region AbstractPropertyDescriptor

		public override Type ComponentType
		{
			get { return eventDescriptor.ComponentType; }
		}

		public override object GetValue(object component)
		{
			return handler.Target;
		}

		public override Type PropertyType
		{
			get { return typeof (string); }
		}

		#endregion

		#region IRealValueHolder Members

		public object RealValue
		{
			get { return handler.Target; }
		}

		#endregion

		#region IShowSourceCodeDescriptor Members

		public void ShowSourceCode()
		{
			ReflectorRouter.Instance.ShowMethod(handler.Method.DeclaringType, handler.Method);
		}

		#endregion

		#region IDynamicInvoke Members

		public void DynamicInvoke()
		{
			// Dynamically build all parameters for this invoke. We know some delegate types so we are going to use them as a nice factory.
			EventUtils.DynamicInvoke(handler, eventDescriptor.Component);
		}
		#endregion
	}
}