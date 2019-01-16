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
using System.Reflection;
using System.Security;
using ACorns.Hawkeye.Tools.Logging;
using System.Runtime.InteropServices;
using ACorns.Hawkeye.Core.Utils;
using ACorns.Hawkeye.Tools.Warning;
using ACorns.Hawkeye.Utils.TypeConverters;

namespace ACorns.Hawkeye.Utils
{
	/// <summary>
	/// RealPropertyDescriptor 
	/// </summary>
	internal class RealPropertyDescriptor : AbstractPropertyDescriptor
	{
		private PropertyInfo propInfo;
		private readonly Type ownerType;
		private readonly object component;
		private string criticalException = null;

		private TypeConverter converter;

		public RealPropertyDescriptor(object component, PropertyInfo propInfo, Type ownerType)
			: base(propInfo.Name)
		{
			this.propInfo = propInfo;
			this.ownerType = ownerType;
			this.component = component;
		}

		protected override void FillAttributes(System.Collections.IList attributeList)
		{
			base.FillAttributes (attributeList);
			
			AttributeUtils.AddAllAttributes(attributeList, propInfo, true);

			AttributeUtils.DeleteNonRelevatAttributes(attributeList);
		}

		public override object GetValue(object component)
		{
			if ( criticalException != null )
				return criticalException;

			try
			{
				object returnValue = propInfo.GetValue(component, new object[] {});

#if LICENCEDBUILD
				returnValue = SecurityUtils.UnpackString(propInfo.Name, returnValue);
#endif
				
				return returnValue;
			}
			catch(SecurityException ex)
			{
				criticalException = ex.Message;
			}
			catch(TargetParameterCountException ex)
			{
				criticalException = ex.Message;
			}
			catch(TargetInvocationException ex)
			{
				criticalException = ex.InnerException.Message;
			}
			return criticalException;
		}

		public override Type ComponentType
		{
			get { return propInfo.PropertyType; }
		}

		public override bool IsReadOnly
		{
			get { return !propInfo.CanWrite; }
		}

		public override Type PropertyType
		{
			get 
			{
				if ( propInfo.PropertyType == typeof(object) )
				{
					return typeof(string);
				}
				else
				{
					return propInfo.PropertyType; 
				}
			}
		}

		public override void SetValue(object component, object value)
		{
			if ( value is IRealValueHolder )
				value = (value as IRealValueHolder).RealValue;

			if ( WarningsHelper.Instance.SetPropertyWarning(propInfo.DeclaringType.Name, value) )
			{
				//LoggingSystem.Instance.LogSet(propInfo.DeclaringType.Name, propInfo.Name, value);
				CodeChangeLoggingSystem.Instance.LogSet(HawkeyeUtils.GetControlName(component), propInfo.Name, value);
				propInfo.SetValue(component, value, new object[] {});
			}
		}

		public override TypeConverter Converter
		{
			get
			{
				converter = base.Converter;
				if (propInfo.DeclaringType.IsPrimitive || propInfo.DeclaringType == typeof(string))
				{
					// leave the origial converter
				}
				else
				{
					if (converter == null || !(converter is ExposePublicPropertiesTypeConverter))
					{
						converter = new ExposePublicPropertiesTypeConverter(converter);
					}
				}
				return converter;
			}
		}
	}
}