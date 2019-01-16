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
	/// Abstract TypeConverter that delgates all calls back to the original converter
	/// </summary>
	internal abstract class AbstractDelegateTypeConverter : TypeConverter
	{
		protected readonly TypeConverter originalConverter;

		public AbstractDelegateTypeConverter(TypeConverter originalConverter)
		{
			this.originalConverter = originalConverter;
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return originalConverter.GetPropertiesSupported(context);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return originalConverter.GetProperties(context, value, attributes);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return originalConverter.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return originalConverter.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return originalConverter.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return originalConverter.ConvertTo(context, culture, value, destinationType);
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			return originalConverter.CreateInstance(context, propertyValues);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return originalConverter.GetCreateInstanceSupported(context);
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return originalConverter.GetStandardValues(context);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return originalConverter.GetStandardValuesExclusive(context);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return originalConverter.GetStandardValuesSupported(context);
		}
		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			return originalConverter.IsValid(context, value);
		}
	}
}