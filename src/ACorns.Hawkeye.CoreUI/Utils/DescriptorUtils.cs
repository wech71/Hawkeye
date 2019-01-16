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
using System.Diagnostics;
using System.Reflection;
using ACorns.Hawkeye.Tabs.Events;
using ACorns.Hawkeye.Tabs.Fields;

namespace ACorns.Hawkeye.Utils
{
	internal interface IAllowSelect
	{
	}
	/// <summary>
	/// PropertyDescriptorUtils 
	/// </summary>
	internal sealed class DescriptorUtils
	{
		private DescriptorUtils(){}
		// Avoid reading this properties (just for safely and stability)
		private static Hashtable avoidableProperties = new Hashtable();

		static DescriptorUtils()
		{
			avoidableProperties.Add("System.Windows.Forms.Control.ShowParams", null);
			avoidableProperties.Add("System.Windows.Forms.Control.ActiveXAmbientBackColor", null);
			avoidableProperties.Add("System.Windows.Forms.Control.ActiveXAmbientFont", null);
			avoidableProperties.Add("System.Windows.Forms.Control.ActiveXAmbientForeColor", null);
			avoidableProperties.Add("System.Windows.Forms.Control.ActiveXEventsFrozen", null);
			avoidableProperties.Add("System.Windows.Forms.Control.ActiveXHWNDParent", null);
			avoidableProperties.Add("System.Windows.Forms.Control.ActiveXInstance", null);
			avoidableProperties.Add("System.Windows.Forms.Form.ShowParams", null);
		}

		public static PropertyDescriptorCollection GetAllProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties;
			PropertyDescriptorCollection childElements = null;

			Attribute[] attributeArray1 = new Attribute[] {};
			attributes = attributeArray1; // replace the attribute array to allow all properties to be visible.

			if ( component is IEnumerable )
			{
				childElements = ConverterUtils.GetEnumerableChildsAsProperties(context, component, attributes);
			}

			if (context == null)
			{
				//properties = TypeDescriptor.GetProperties(component, attributes);
				properties = GetAllPropertiesCustom(component, attributes);
			}
			else
			{
				RemapPropertyDescriptor remapDescriptor = context.PropertyDescriptor as RemapPropertyDescriptor;
				if (remapDescriptor != null)
				{
					component = remapDescriptor.OriginalComponent;
				}

				TypeConverter converter1 = (context.PropertyDescriptor == null) ? TypeDescriptor.GetConverter(component) : context.PropertyDescriptor.Converter;
				if ((converter1 != null) && converter1.GetPropertiesSupported(context))
				{
//					properties = converter1.GetProperties(context, component, attributes);
//					if ( properties == null )
						properties = GetAllPropertiesCustom(component, attributes);
				}
				else
				{
					properties = GetAllPropertiesCustom(component, attributes);
				}
			}

			if ( childElements != null && childElements.Count > 0 )
			{
				properties = MergeProperties(properties, childElements);
			}

			return properties;
		}

		public static PropertyDescriptorCollection GetStaticProperties(Type componentType)
		{
			PropertyInfo[] properties = componentType.GetProperties();
			ArrayList propDesc = new ArrayList();
			foreach (PropertyInfo prop in properties)
			{
				propDesc.Add(new StaticPropertyDescriptor(componentType, prop));
			}
			return GetProperties(propDesc);
		}

		public static PropertyDescriptorCollection GetInstanceEvents(object component)
		{
			Type componentType;
			if (component is Type)
			{
				componentType = component as Type;
			}
			else
			{
				componentType = component.GetType();
			}

			EventHandlerList eventHandlerList = null;

			PropertyInfo componentEvents = componentType.GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (componentEvents != null)
			{
				eventHandlerList = componentEvents.GetValue(component, new object[] {}) as EventHandlerList;
			}

			EventInfo[] eventsInfo = componentType.GetEvents();
			ArrayList propDesc = new ArrayList();
			foreach (EventInfo eventInfo in eventsInfo)
			{
				propDesc.Add(new EventPropertyDescriptor(component, eventInfo, eventHandlerList));
			}
			return GetProperties(propDesc);
		}

		public static PropertyDescriptorCollection RemapComponent(PropertyDescriptorCollection propertyDescriptors, object remappedComponent, object originalComponent, string displayNamePrefix, TypeConverter typeConverter)
		{
			PropertyDescriptor[] newProperties = new PropertyDescriptor[propertyDescriptors.Count];

			for (int i = 0; i < propertyDescriptors.Count; i++)
			{
				PropertyDescriptor originalDescriptor = propertyDescriptors[i];
				if (originalDescriptor.PropertyType.IsPrimitive || originalDescriptor.PropertyType.IsEnum || originalDescriptor.PropertyType == typeof (string)
					|| typeof (ICollection).IsAssignableFrom(originalDescriptor.PropertyType))
				{
					newProperties[i] = new RemapPropertyDescriptor(originalDescriptor, remappedComponent, originalComponent, displayNamePrefix, new ShowChildListConverter(originalDescriptor.Converter));
				}
				else
				{
					if (typeConverter == null)
					{
						newProperties[i] = new RemapPropertyDescriptor(originalDescriptor, remappedComponent, originalComponent, displayNamePrefix, new ShowChildListConverter(originalDescriptor.Converter));
					}
					else
					{
						newProperties[i] = new RemapPropertyDescriptor(originalDescriptor, remappedComponent, originalComponent, displayNamePrefix, new ShowChildListConverter(typeConverter));
					}
				}
			}

			PropertyDescriptorCollection retCollection = new PropertyDescriptorCollection(newProperties);
			return retCollection;
		}

		public static ArrayList GetProperties(PropertyDescriptorCollection collection)
		{
			ArrayList attributes = new ArrayList();
			foreach (PropertyDescriptor attr in collection)
			{
				attributes.Add(attr);
			}
			return attributes;
		}

		public static PropertyDescriptorCollection GetProperties(ArrayList properties)
		{
			PropertyDescriptorCollection collection = new PropertyDescriptorCollection((PropertyDescriptor[]) properties.ToArray(typeof (PropertyDescriptor)));
			return collection;
		}
		public static PropertyDescriptorCollection GetProperties(ICollection properties)
		{
			ArrayList newList = new ArrayList(properties);
			PropertyDescriptorCollection collection = new PropertyDescriptorCollection((PropertyDescriptor[]) newList.ToArray(typeof (PropertyDescriptor)));
			return collection;
		}

		public static PropertyDescriptorCollection MergeProperties(PropertyDescriptorCollection originalCollection, PropertyDescriptorCollection toMerge)
		{
			if( originalCollection == null )
				return toMerge;

			ArrayList originalList = GetProperties(originalCollection);
			ArrayList toMergeList = GetProperties(toMerge);
			originalList.AddRange(toMergeList);
			return GetProperties(originalList);
		}

		public static PropertyDescriptorCollection GetAllFields(ITypeDescriptorContext context, object component, Attribute[] attributes)
		{
			Type type;
			if (component is Type)
				type = component as Type;
			else
				type = component.GetType();
			
			ArrayList list = new ArrayList();

			int depth = 1;
			do
			{
				FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic|BindingFlags.DeclaredOnly);

				for (int i = 0; i < fieldInfos.Length; i++)
				{
					FieldInfo fieldInfo = fieldInfos[i];
					try
					{
						list.Add(new FieldPropertyDescriptor(component, fieldInfo, type, depth));
					}
					catch (Exception ex)
					{
						Trace.WriteLine("DescriptorUtils: GetAllFields:" + fieldInfo.Name + ":" + ex.ToString(), "Hawkeye");
					}
				}

				if ( type == typeof(object) )
					break;

				type = type.BaseType;
				depth ++;
			}
			while(true);

			if (component is Type)
				type = component as Type;
			else
				type = component.GetType();
			depth = 1;
			do
			{
				FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

				for (int i = 0; i < fieldInfos.Length; i++)
				{
					FieldInfo fieldInfo = fieldInfos[i];
					try
					{
						list.Add(new FieldPropertyDescriptor(null, fieldInfo, type, depth));
					}
					catch (Exception ex)
					{
						Trace.WriteLine("DescriptorUtils: GetAllFields:" + fieldInfo.Name + ":" + ex.ToString(), "Hawkeye");
					}
				}

				if (type == typeof(object))
					break;

				type = type.BaseType;
				depth++;
			}
			while (true);


			FieldPropertyDescriptor[] fieldDesc = (FieldPropertyDescriptor[]) list.ToArray(typeof (FieldPropertyDescriptor));
			return new PropertyDescriptorCollection(fieldDesc);
		}



		public static PropertyDescriptorCollection GetAllPropertiesCustom(object component, Attribute[] attributes)
		{
			if (component ==null || component.GetType().IsPrimitive || component is string)
				return GetProperties(new ArrayList());

			Type type = component.GetType();
			
			Hashtable fullList = new Hashtable();

			int depth = 1;
			do
			{
				PropertyInfo[] propInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
				AddPropertyDescriptors(propInfos, fullList, component, type, false);

				PropertyInfo[] propInfos1 = type.GetProperties(BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
				AddPropertyDescriptors(propInfos1, fullList, component, type, true);

				if ( type == typeof(object) )
					break;

				type = type.BaseType;
				depth ++;
			}
			while(true);

			return GetProperties(fullList.Values);
		}

		private static void AddPropertyDescriptors(PropertyInfo[] propInfos, Hashtable list, object component, Type type, bool areStatic)
		{
			for (int i = 0; i < propInfos.Length; i++)
			{
				try
				{
					PropertyInfo propInfo = propInfos[i];

					if ( list.ContainsKey(propInfo.Name))
						continue;

					if ( avoidableProperties.ContainsKey(propInfo.DeclaringType.FullName + "." + propInfo.Name) )
						continue; // avoid


					if( areStatic )
					{
						list.Add(propInfo.Name, new StaticPropertyDescriptor(type, propInfo));
					}
					else
					{
						list.Add(propInfo.Name,  new RealPropertyDescriptor(component, propInfo, type));
					}
				}
				catch (Exception ex)
				{
					Trace.WriteLine("DescriptorUtils: AddPropertyDescriptors:" + ex.ToString(), "Hawkeye");
				}
			}
		}
	}
}