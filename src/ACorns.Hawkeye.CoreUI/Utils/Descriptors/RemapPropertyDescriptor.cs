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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;
using ACorns.Hawkeye.Tabs.Methods;
using ACorns.Hawkeye.Tools.Logging;
using System.Drawing.Design;
using ACorns.Hawkeye.Tools.Warning;
using ACorns.Hawkeye.Tools.Reflector;

namespace ACorns.Hawkeye.Utils
{
	/// <summary>
	/// Remaps a Propertie's Descriptor Component type
	/// </summary>
	internal class RemapPropertyDescriptor : PropertyDescriptor, IShowSourceCodeHandler
	{
		private PropertyDescriptor originalPropertyDescriptor;
		private object reamappedComponent;
		private object originalComponent;
		private string displayNamePrefix;
		private TypeConverter typeConverter;

		public RemapPropertyDescriptor(PropertyDescriptor originalPropertyDescriptor, object reamappedComponent, object originalComponent, string displayNamePrefix, TypeConverter typeConverter)
			: base(originalPropertyDescriptor.Name, null)
		{
			this.originalPropertyDescriptor = originalPropertyDescriptor;
			this.reamappedComponent = reamappedComponent;
			this.originalComponent = originalComponent;
			this.displayNamePrefix = displayNamePrefix;
			this.typeConverter = new ProxyTypeConverter(typeConverter);
		}

		#region Properties
		public object RemappedComponent
		{
			get { return this.reamappedComponent; }
		}

		public object OriginalComponent
		{
			get
			{
				return originalPropertyDescriptor.GetValue(this.originalComponent);
			}
		}
		#endregion

		protected override void FillAttributes(IList attributeList)
		{	// this method is not used - we overrride the Attributes property and create there a new set of attributes
			base.FillAttributes(attributeList);
			//originalPropertyDescriptor.Attributes;
			attributeList.Add(new EditorAttribute(typeof (UITypeEditor), typeof (UITypeEditor)));
			attributeList.Add(new RefreshPropertiesAttribute(RefreshProperties.Repaint));
			attributeList.Add(new DesignerAttribute(typeof (ComponentDesigner), typeof (IDesigner)));
		}

		protected override AttributeCollection CreateAttributeCollection()
		{
			return base.CreateAttributeCollection();
		}

		public override string Name
		{
			get { return originalPropertyDescriptor.Name; }
		}
		public override string Description
		{
			get { return originalPropertyDescriptor.Description; }
		}
		public override bool IsBrowsable
		{
			get { return originalPropertyDescriptor.IsBrowsable; }
		}
		public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
		{
			return originalPropertyDescriptor.GetChildProperties(originalComponent);
		}
		public override AttributeCollection Attributes
		{
			get
			{
				return originalPropertyDescriptor.Attributes;
//				ArrayList tempAttributes = new ArrayList();
//
//				AttributeCollection originalAttributes = originalPropertyDescriptor.Attributes;
//				//AttributeUtils.PrintAttributes(originalAttributes);
//				tempAttributes.AddRange( originalAttributes );
//				/*tempAttributes.AddRange( AttributeUtils.DeleteNonRelevatAttributes(originalAttributes) );
//
//				tempAttributes.Add(new EditorAttribute(typeof (UITypeEditor), typeof (UITypeEditor)));
//				tempAttributes.Add(new RefreshPropertiesAttribute(RefreshProperties.Repaint));
//				tempAttributes.Add(new DesignerAttribute(typeof (ComponentDesigner), typeof (IDesigner)));
//*/
//				return AttributeUtils.GetAttributes(tempAttributes);
			}
		}

		public override bool DesignTimeOnly
		{
			get { return false; }
		}

		public override object GetEditor(Type editorBaseType)
		{
			return originalPropertyDescriptor.GetEditor(editorBaseType);
		}
		public override bool CanResetValue(object component)
		{
			return originalPropertyDescriptor.CanResetValue(originalComponent);
		}
		public override void ResetValue(object component)
		{
			originalPropertyDescriptor.ResetValue(originalComponent);
		}
		public override void SetValue(object component, object value)
		{
			object realValue = value;
			if (value is IRealValueHolder)
			{
				realValue = (value as IRealValueHolder).RealValue;
			}
			if ( WarningsHelper.Instance.SetPropertyWarning(originalPropertyDescriptor.Name, realValue) )
			{
				originalPropertyDescriptor.SetValue(originalComponent, realValue);
			}
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			try
			{
				return originalPropertyDescriptor.GetValue(originalComponent);
			}
			catch (TargetInvocationException ex)
			{
				return "Ex:" + ex.InnerException.Message;
			}
			catch (Exception ex)
			{
				Trace.WriteLine("RemapPropertyDescriptor.GetValue:" + ex.ToString(), "Hawkeye");
				return "Ex:" + ex.Message;
			}
		}

		public override bool IsReadOnly
		{
			get { return originalPropertyDescriptor.IsReadOnly; }
		}

		public override Type PropertyType
		{
			get { return originalPropertyDescriptor.PropertyType; }
		}
		public override Type ComponentType
		{
			get { return this.reamappedComponent.GetType(); }
		}
		public override string DisplayName
		{
			get
			{
				if (displayNamePrefix != null)
				{
					return displayNamePrefix + ":" + originalPropertyDescriptor.DisplayName;
				}
				else
				{
					return originalPropertyDescriptor.DisplayName;
				}
			}
		}

		public override TypeConverter Converter
		{
			get
			{
				return this.typeConverter;
				//return originalPropertyDescriptor.Converter;
			}
		}

		#region IShowSourceCodeDescriptor Members

		public void ShowSourceCode()
		{
			ReflectorRouter.Instance.ShowSourceCode(this.originalPropertyDescriptor);
		}

		#endregion
	}
}