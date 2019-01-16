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

namespace ACorns.Hawkeye.Utils
{
	/// <summary>
	/// Summary description for ShowChildListConverter.
	/// </summary>
	internal class ShowChildListConverter : AbstractDelegateTypeConverter
	{
		public ShowChildListConverter(TypeConverter originalConverter) : base(originalConverter)
		{
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			// try to convert to IList
			object val = context.PropertyDescriptor.GetValue(context.Instance);
			if (context != null && (val is IEnumerable) && !(val is string))
			{
				return true;
			}
			else
			{
				return originalConverter.GetPropertiesSupported(context);
			}
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (value is IEnumerable)
			{
				return ConverterUtils.GetEnumerableChildsAsProperties(context, value, attributes);
			}
			else
			{
				return originalConverter.GetProperties(context, value, attributes);
			}
		}
	}
}