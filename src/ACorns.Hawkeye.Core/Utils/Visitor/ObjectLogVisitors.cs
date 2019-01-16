/* ****************************************************************************
 *  Hawkeye - The .Net Runtime Object Editor - Loader
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
using System.Windows.Forms;
using ACorns.Hawkeye.Core.Utils;
using ACorns.Hawkeye.Utils;
using System.Drawing;

namespace ACorns.Hawkeye.Core.Utils.Visitor
{
	internal delegate string ConvertObjectToString(object value);

	/// <summary>
	/// Summary description for ObjectLogVisitors.
	/// </summary>
	public class ObjectLogVisitors
	{
		#region Instance
		private static ObjectLogVisitors instance = new ObjectLogVisitors();
		/// <summary>
		/// Singleton.
		/// </summary>
		public static ObjectLogVisitors Instance
		{
			get { return instance; }
		}
		#endregion

		private ArrayList convertors = new ArrayList();

		private ObjectLogVisitors()
		{
			convertors.Add(new ConvertObjectToString(StringToCode));
			convertors.Add(new ConvertObjectToString(EnumToCode));
			convertors.Add(new ConvertObjectToString(PrimitiveToString));
			convertors.Add(new ConvertObjectToString(StructToString));

			convertors.Add(new ConvertObjectToString(CursorToString));
			convertors.Add(new ConvertObjectToString(ColorToString));
			convertors.Add(new ConvertObjectToString(StructToString));

			// Keep this line the last one
			convertors.Add(new ConvertObjectToString(JustToString));
		}

		private string CursorToString(object value)
		{
			if ( value is Cursor )
			{
				Cursor cursor = (Cursor)value;
				return "Cursors." + cursor.ToString().Replace("[Cursor: ","").Trim(new char[]{'[',']'});
			}
			return null;
		}
		private string ColorToString(object value)
		{
			if ( value is Color )
			{
				Color color = (Color)value;
				if ( color.IsSystemColor )
				{
					return "SystemColors." + color.Name;
				}
				else if ( color.IsKnownColor || color.IsNamedColor )
				{
					return "Color." + color.Name;
				}
				else
				{
					return "new Color(" + color.A + ", " + color.R + ", " + color.G + ", " + color.B + ")";
				}
			}
			return null;
		}
		private static string PrimitiveToString(object value)
		{
			Type valueType = value.GetType();
			if ( valueType.IsPrimitive )
				return value.ToString();
			return null;
		}
		private static string StructToString(object value)
		{
			Type valueType = value.GetType();
			if ( valueType.IsValueType && !valueType.IsPrimitive )
			{
				// this is a struct!
				// Parse the toString
				// {X=32,Y=0,Width=292,Height=22}
				string toString = value.ToString().Trim( new char[] { '{', '}' } );	// structs have by 
				// X=32,Y=0,Width=292,Height=22
				string[] allParams = toString.Split(',');
				for ( int i = 0; i < allParams.Length; i++ )
				{
					string param = allParams[i];
				
					int equalIndex = param.IndexOf('=');
					if ( equalIndex > -1 )
						allParams[i] = param.Substring(equalIndex+1);
				}

				return " new " + valueType.Name + "(" + String.Join(", ", allParams) + ")";
			}
			return null;
		}
		private static string EnumToCode(object value)
		{
			if ( value is Enum )
				return HawkeyeUtils.EnumToString((Enum)value);
			return null;
		}
		private static string StringToCode(object value)
		{
			if ( value == null )
				return String.Empty;

			string strValue = ( value as string );
			if ( strValue != null )
				return "\"" + strValue + "\"";
			return null;
		}
		private static string JustToString(object value)
		{
			if (value!=null)
				return value.ToString();
			return String.Empty;
		}

		public string ConvertValue(object value)
		{
			foreach( ConvertObjectToString codeConvert in convertors )
			{
				string convertedValue = codeConvert(value);
				if ( convertedValue != null )
					return convertedValue;
			}
			return String.Empty;
		}
	}
}