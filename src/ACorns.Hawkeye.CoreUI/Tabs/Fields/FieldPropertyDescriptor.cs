using ACorns.Hawkeye.Core.Utils;
using ACorns.Hawkeye.Tabs.Methods;
using ACorns.Hawkeye.Tools.Logging;
using ACorns.Hawkeye.Tools.Reflector;
using ACorns.Hawkeye.Utils;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;

namespace ACorns.Hawkeye.Tabs.Fields
{
	internal class FieldPropertyDescriptor : AbstractPropertyDescriptor, IShowSourceCodeHandler
	{
		private readonly object component;
		private readonly int depth;
		private readonly System.Reflection.FieldInfo field;
		private readonly Type ownerType;

		public FieldPropertyDescriptor(object component, System.Reflection.FieldInfo field, Type ownerType, int depth)
			: base(field.Name)
		{
			this.component = component;
			this.field = field;
			this.ownerType = ownerType;
			this.depth = depth;
		}

		protected override void FillAttributes(IList attributeList)
		{
			base.FillAttributes(attributeList);
			attributeList.Add(new CategoryAttribute(this.depth.ToString() + ". " + this.ownerType.Name));
			attributeList.Add(new RefreshPropertiesAttribute(RefreshProperties.Repaint));
			attributeList.Add(new DesignerAttribute(typeof(MethodDesigner), typeof(IDesigner)));
			attributeList.Add(new DesignTimeVisibleAttribute(false));
		}

		public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
		{
			return base.GetChildProperties(instance, filter);
		}

		public override object GetValue(object component)
		{
			object stringValue = this.field.GetValue(component);

#if LICENCEDBUILD
			stringValue = SecurityUtils.UnpackString(this.field.Name, stringValue);
#endif

			return stringValue;
		}

		public override void SetValue(object component, object value)
		{
			CodeChangeLoggingSystem.Instance.Log(string.Concat(new object[] { HawkeyeUtils.GetControlName2(component), this.field.Name, " = ", value, ";" }));
			this.field.SetValue(component, value);
		}

		public void ShowSourceCode()
		{
			ReflectorRouter.Instance.ShowField(this.ownerType, this.field);
		}

		public override AttributeCollection Attributes
		{
			get
			{
				return base.Attributes;
			}
		}

		public object Component
		{
			get
			{
				return this.component;
			}
		}

		public override Type ComponentType
		{
			get
			{
				return this.component.GetType();
			}
		}

		public override TypeConverter Converter
		{
			get
			{
				return base.Converter;
			}
		}

		public System.Reflection.FieldInfo FieldInfo
		{
			get
			{
				return this.field;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return this.field.FieldType;
			}
		}
	}
}

