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
using ACorns.Hawkeye.Tools.Logging;
using System.Diagnostics;

namespace ACorns.Hawkeye.Utils
{
	/// <summary>
	/// StaticPropertyDescriptor 
	/// </summary>
	internal class StaticPropertyDescriptor : PropertyDescriptor
	{
		private Type objectType;
		private PropertyInfo propInfo;

		public StaticPropertyDescriptor(Type objectType, PropertyInfo propInfo)
			: base(propInfo.Name, null)
		{
			this.objectType = objectType;
			this.propInfo = propInfo;
		}
		protected override void FillAttributes(System.Collections.IList attributeList)
		{
			base.FillAttributes (attributeList);

			attributeList.Add(new CategoryAttribute( "(static: " + objectType.Name + ")" ));
			attributeList.Add(new RefreshPropertiesAttribute(RefreshProperties.Repaint));;
		}
		public override object GetValue(object component)
		{
			return propInfo.GetValue(null, new object[] {});
		}

		public override Type ComponentType
		{
			get { return objectType; }
		}

		public override bool IsReadOnly
		{
			get { return !propInfo.CanWrite; }
		}

		public override Type PropertyType
		{
			get { return propInfo.PropertyType; }
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override void ResetValue(object component)
		{
		}
		
		public override void SetValue(object component, object value)
		{
			Trace.Assert(false);
			//if ( WarningsHelper.Instance.SetPropertyWarning(propInfo.DeclaringType.Name, value) )
			//{
			//    CodeChangeLoggingSystem.Instance.LogSet(propInfo.DeclaringType.Name, propInfo.Name, value);
			//    propInfo.SetValue(null, value, new object[] {});
			//}
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}