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
using System.Globalization;
using System.Reflection;
using ACorns.Hawkeye.Utils;
using ACorns.Hawkeye.Core.Utils.Accessors;

namespace ACorns.Hawkeye.Tabs.Events
{
	internal class EventInfoConverter : TypeConverter
	{
		// standard winforms event key objects: "private static readonly object EventLayout;"

		private EventPropertyDescriptor eventDescriptor;
		private ArrayList eventListeners;
		private EventHandlerList eventHandlerList;
		private FieldAccesor handlerAccesor;
		private FieldAccesor keyAccesor;

		public EventInfoConverter(EventPropertyDescriptor eventDescriptor)
		{
			this.eventDescriptor = eventDescriptor;
		}

		#region Convert

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return true;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return true;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string strValue = value as string;
				strValue = strValue.Replace(' ', '_');
				return Enum.Parse(typeof (EnumHelperEnum), strValue as string);
			}
			else
			{
				return value;
			}
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof (string) && value != null)
			{
				return value.ToString().Replace('_',' ');
			}
			else
			{
				return value;
			}
		}

		#endregion

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			return true;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList names = new ArrayList();
			foreach ( string name in Enum.GetNames(typeof (EnumHelperEnum)))
			{
				names.Add(name.Replace('_', ' '));
			}
			return new StandardValuesCollection(names);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			if ( eventListeners == null || eventListeners.Count == 0 )
				return false;
			else
				return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return GetProperties();
		}

		public PropertyDescriptorCollection GetProperties()
		{
			if (eventListeners == null)
			{
				ReadListeners();
			}
			if (eventListeners != null)
			{
				return new PropertyDescriptorCollection((PropertyDescriptor[]) eventListeners.ToArray(typeof (PropertyDescriptor)));
			}
			else
			{
				return null;
			}
		}

		#region Read Listeners

		internal void ReadListeners()
		{
			eventListeners = new ArrayList();
			// for examples we use event with name "Load"
			// first try to read key representing the static eventKey (EventLoad as key object static)
			FieldAccesor eventKeyAccesor = new FieldAccesor(eventDescriptor.Component, "Event" + eventDescriptor.Name);
			if (ReadEventHandlersFromHandlerList(eventKeyAccesor))
			{	// there is a key like "Event" + event name ("eg: EventLoad") representing the key in the eventHandlerList
				return;
			}
			if( eventDescriptor.Name.EndsWith("Changed") )
			{	// try "EventBackColor" for "BackColorChanged" event
				string shortName = eventDescriptor.Name.Substring(0, eventDescriptor.Name.IndexOf("Changed") );
				eventKeyAccesor = new FieldAccesor(eventDescriptor.Component, "Event" + shortName);
				if (ReadEventHandlersFromHandlerList(eventKeyAccesor))
				{	// there is a key like "Event" + event name ("eg: EventLoad") representing the key in the eventHandlerList
					return;
				}
			}
			eventKeyAccesor = new FieldAccesor(eventDescriptor.Component, "EVENT_" + eventDescriptor.Name);
			if (ReadEventHandlersFromHandlerList(eventKeyAccesor))
			{	// there is a key like "Event" + event name ("eg: EventLoad") representing the key in the eventHandlerList
				return;
			}

			// try to read the delegate on the object directly!
			// try "Load"
			FieldAccesor delegateDirectAccesor = new FieldAccesor(eventDescriptor.Component, eventDescriptor.Name);
			if (!delegateDirectAccesor.IsValid)
			{ // try "onLoadDelegate"
				delegateDirectAccesor = new FieldAccesor(eventDescriptor.Component, "on" + eventDescriptor.Name + "Delegate");
			}
			if (!delegateDirectAccesor.IsValid)
			{ // try "onLoad"
				delegateDirectAccesor = new FieldAccesor(eventDescriptor.Component, "on" + eventDescriptor.Name);
			}

			if (delegateDirectAccesor.IsValid)
			{
				delegateDirectAccesor.Get();
				Delegate eventHandlers = delegateDirectAccesor.Value as Delegate;
				if (eventHandlers != null)
				{
					eventListeners = new ArrayList();
					AddListenersFromDelegate(eventHandlers);
				}
			}
			else
			{ // sorry - give up - don't know how to read the implementation of this event :(
			}
		}

		private void AddListenersFromDelegate(Delegate eventHandlers)
		{
			Delegate[] invocationList = eventHandlers.GetInvocationList();
			foreach (Delegate del in invocationList)
			{
				if ( del is MulticastDelegate && del != eventHandlers )
				{
					MulticastDelegate multiDel = del as MulticastDelegate;
					AddListenersFromDelegate(multiDel);
					continue;
				}
				eventListeners.Add(new EventListenerPropertyDescriptor(eventDescriptor, del));
			}
		}

		#region Use EventHandlerList 

		private bool ReadEventHandlersFromHandlerList(FieldAccesor eventKeyAccesor)
		{
			if ( !eventKeyAccesor.IsValid )
				return false;

			if (eventListeners == null)
			{
				eventListeners = new ArrayList();
			}

			eventKeyAccesor.Get();
			object eventKey = eventKeyAccesor.Value;
			if (eventKey == null)
			{	// no data
				return true;
			}

			eventHandlerList = eventDescriptor.EventHandlerList;
			if (eventHandlerList != null)
			{
				object entry = GetHead();
				if (entry != null)
				{
					do
					{
						// first read the target
						if (keyAccesor == null)
						{
							keyAccesor = new FieldAccesor(entry, "key");
						}
						keyAccesor.Target = entry;
						keyAccesor.Get();
						if (keyAccesor.Value == eventKey)
						{
							if (handlerAccesor == null)
							{
								handlerAccesor = new FieldAccesor(entry, "handler");
							}

							handlerAccesor.Target = entry;
							handlerAccesor.Get();

							if (handlerAccesor.Value != null)
							{
								AddListenersFromDelegate(handlerAccesor.Value as Delegate);
								//eventListeners.Add(new EventListenerPropertyDescriptor(eventDescriptor, handlerAccesor.Value as Delegate));
							}
						}
						entry = GetListEntry(entry);
					} while (entry != null);
				}
			}
			return true;
		}

		#endregion

		//[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
		public object GetHead()
		{
			FieldInfo field = eventHandlerList.GetType().GetField("head", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
			if (field != null)
			{
				return field.GetValue(eventHandlerList);
			}
			else
			{
				return null;
			}
		}

		private FieldInfo nextField = null;

		public object GetListEntry(object entry)
		{
			if (entry != null)
			{
				if (nextField == null)
				{
					nextField = entry.GetType().GetField("next", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
				}
				if (nextField != null)
				{
					return nextField.GetValue(entry);
				}
			}
			return null;
		}

		#endregion

		public ArrayList EventListeners
		{
			get { return this.eventListeners; }
		}

		public override string ToString()
		{
			if (eventListeners == null)
			{
				return "("+ eventDescriptor.EventInfo.EventHandlerType.Name +")";
			}
			else
			{
				return "{Listeners:" + this.eventListeners.Count + "}";
			}
		}
	}
}