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
using System.Reflection;

namespace ACorns.Hawkeye.Core.Utils.Accessors
{
	/// <summary>
	/// Summary description for PropertyAccesor.
	/// </summary>
	public class PropertyAccesor
	{
		private readonly string propertyName;
		private readonly Type targetType;

		private object target;
		private PropertyInfo propertyInfo;

		private object value;

		public PropertyAccesor(object target, string propertyName)
			: this(target.GetType(), target, propertyName)
		{
		}

		public PropertyAccesor(Type targetType, string propertyName)
			: this(targetType, null, propertyName)
		{
		}

		public PropertyAccesor(Type targetType, object target, string propertyName)
		{
			this.target = target;
			this.targetType = targetType;
			this.propertyName = propertyName;

			do
			{
				TryReadProp(BindingFlags.Default);
				TryReadProp(BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				TryReadProp(BindingFlags.Static | BindingFlags.FlattenHierarchy);

				TryReadProp(BindingFlags.NonPublic | BindingFlags.Instance);

				TryReadProp(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				TryReadProp(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);
				TryReadProp(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);

				TryReadProp(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty | BindingFlags.GetProperty | BindingFlags.IgnoreCase | BindingFlags.IgnoreReturn | BindingFlags.Instance | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.SetProperty | BindingFlags.Static);

				TryReadProp(BindingFlags.NonPublic | BindingFlags.Static);
				TryReadProp(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);
				TryReadProp(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty);

				if (propertyInfo == null)
				{
					this.targetType = this.targetType.BaseType;
					if (this.targetType == typeof (object))
					{ // no chance
						break;
					}
				}

			} while (propertyInfo == null);
		}

		private void SearchForProperty(BindingFlags bindingFlags)
		{
			if (propertyInfo != null)
				return;

			PropertyInfo[] all = targetType.GetProperties(bindingFlags);
			foreach (PropertyInfo prop in all)
			{
				if (prop.Name == this.propertyName)
				{
					this.propertyInfo = prop;
					return;
				}
			}
		}


		private void TryReadProp(BindingFlags bindingFlags)
		{
			if (propertyInfo != null)
				return;
			propertyInfo = targetType.GetProperty(propertyName, bindingFlags);
			if (propertyInfo == null)
			{
				SearchForProperty(bindingFlags);
			}
		}

		public PropertyAccesor Save()
		{
			value = propertyInfo.GetValue(target, null);
			return this;
		}

		public void Clear()
		{
			propertyInfo.SetValue(target, null, null);
		}

		public void Restore()
		{
			propertyInfo.SetValue(target, value, null);
		}

		public void Restore(object newValue)
		{
			propertyInfo.SetValue(target, newValue, null);
			this.value = newValue;
		}

		public object Target
		{
			get { return target; }
			set { this.target = value; }
		}

		public bool IsValid
		{
			get { return this.propertyInfo != null; }
		}

		public object Value
		{
			get { return this.value; }
		}
	}
}