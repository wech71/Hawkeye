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
using System.Drawing;
using System.Windows.Forms.Design;
using ACorns.Hawkeye.Utils;

namespace ACorns.Hawkeye.Tabs.Fields
{
	/// <summary>
	/// Tab that shows ALL the properties of the currently selected item.
	/// </summary>
	internal class AllFieldsTab : PropertyTab
	{
		public AllFieldsTab()
		{
		}

		public override Bitmap Bitmap
		{
			get { return SystemUtils.LoadBitmap( "Tabs.Fields.bmp"); }
		}

		public override string TabName
		{
			get { return "All Fields"; }
		}

		public override bool CanExtend(object extendee)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
		{
			return GetProperties(null, component, attributes);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
		{
			PropertyDescriptorCollection propsCollection = DescriptorUtils.GetAllFields(context, component, attributes);
			return DescriptorUtils.RemapComponent(propsCollection, component, component, null, null);
		}
	}
}