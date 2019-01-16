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
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ACorns.Hawkeye.Core.Utils.Visitor;

namespace ACorns.Hawkeye.Core.Utils
{
	/// <summary>
	/// Summary description for EventHandlerTypeVisitors.
	/// </summary>
	internal class EventHandlerTypeVisitors
	{
		#region Instance

		private static EventHandlerTypeVisitors instance = new EventHandlerTypeVisitors();

		/// <summary>
		/// Singleton.
		/// </summary>
		public static EventHandlerTypeVisitors Instance
		{
			get { return instance; }
		}

		#endregion

		private Hashtable handlerVisitors = new Hashtable();

		private ConvertObjectToString toStringConverter = new ConvertObjectToString(ToStringConvert);

		private EventHandlerTypeVisitors()
		{
			AddVisitor(typeof (EventArgs), new ConvertObjectToString(EventArgsConvert));
			//AddVisitor(typeof(MouseEventArgs), new ConvertObjectToString(MouseEventArgs));
		}

		private void AddVisitor(Type type, ConvertObjectToString converter)
		{
			handlerVisitors.Add(type, converter);
		}

		private ConvertObjectToString GetVisitor(Type type)
		{
			ConvertObjectToString convertor = handlerVisitors[type] as ConvertObjectToString;
			if (convertor == null)
			{
				return toStringConverter;
			}
			else
			{
				return convertor;
			}
		}

		#region Converters

		private static string MouseEventArgs(object value)
		{
			MouseEventArgs args = value as MouseEventArgs;
			return "MouseEventArgs( Button:" + HawkeyeUtils.EnumToString(args.Button) + ", Location:(" + args.X + "x" + args.Y + "), Clicks:" + args.Clicks + ", Delta:" + args.Delta + ")";
		}

		private static string EventArgsConvert(object value)
		{
			return (value as EventArgs).ToString();
		}

		private static string ToStringConvert(object value)
		{
			try
			{
				string defaultToString = value.ToString();
				if (defaultToString == value.GetType().FullName)
				{
					StringBuilder toStringBuild = new StringBuilder();

					toStringBuild.Append(value.GetType().Name + "{");

					// crappy to string - build a better one :)
					PropertyInfo[] allProps = value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
					bool first = true;
					foreach (PropertyInfo prop in allProps)
					{
						try
						{
							if (!first)
								toStringBuild.Append(", ");
							first = false;

							toStringBuild.Append(prop.Name + " = ");
							object propValue = prop.GetValue(value, null);
							if (propValue == null)
								toStringBuild.Append("<null>");
							else
								toStringBuild.Append(propValue);
						}
						catch (Exception ex)
						{
							toStringBuild.Append(prop.Name + " ex:" + ex.Message);
						}
					}

					toStringBuild.Append("}");

					return toStringBuild.ToString();
				}
				return value.ToString();
			}
			catch (Exception)
			{
				return value.ToString();
			}
		}

		#endregion

		public string Convert(object value)
		{
			if (value == null)
				return "<null>";
			ConvertObjectToString convertor = GetVisitor(value.GetType());
			return convertor(value);
		}
	}
}