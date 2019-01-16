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

using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ACorns.Hawkeye.Core.Utils;
using ACorns.Hawkeye.Tools.Logging;

using ACorns.Hawkeye.Utils;
using System.Drawing.Design;
using ACorns.Hawkeye.Tools.Reflector;
using System.Diagnostics;

namespace ACorns.Hawkeye.Tabs.Methods
{
	/// <summary>
	/// Summary description for MethodPropertyDescriptor.
	/// </summary>
	internal class MethodPropertyDescriptor : PropertyDescriptor, IShowSourceCodeHandler
	{
		#region Help
		[TypeConverter(typeof (MethodEditingConverter))]
		internal class MethodPropertyValueHolder : IRealValueHolder
		{
			private readonly MethodPropertyDescriptor method;

			public MethodPropertyValueHolder(MethodPropertyDescriptor method)
			{
				this.method = method;
			}

			public MethodPropertyDescriptor Method
			{
				get { return method; }
			}

			public override string ToString()
			{
				if (method.valueOfLastRun != null)
					return method.valueOfLastRun.ToString();
				else
				{
					if (method.ParametersCount == 0)
						return "(select to invoke)";
					else
					{
						return "";
					}
				}
			}

			#region IRealValueHolder Members

			public object RealValue
			{
				get { return method.ValueOfLastRun; }
			}

			#endregion
		}

		#endregion

		private readonly MethodInfo methodInfo;
		private readonly Type ownerType;
		private readonly int depth;
		private readonly object monitoredObject;
		private object valueOfLastRun = null;
		private TypeConverter converter;
		private MethodPropertyValueHolder valueHolder;

		private ArrayList parameterDescriptors; // as ParameterPropertyDescriptor
		private PropertyDescriptorCollection propertyDescriptorCollection;
		private ReturnParameterDescriptor returnDescriptor;

		public MethodPropertyDescriptor(object monitoredObject, MethodInfo method, Type ownerType, int depth)
			: base((method.IsPublic ? "+ " : "- ") + method.Name, null)
		{
			this.monitoredObject = monitoredObject;
			this.methodInfo = method;
			this.ownerType = ownerType;
			this.depth = depth;
			valueHolder = new MethodPropertyValueHolder(this);
		}

		protected override void FillAttributes(IList attributeList)
		{
			base.FillAttributes(attributeList);
			attributeList.Add(new EditorAttribute(typeof (MethodEditor), typeof (UITypeEditor)));
			attributeList.Add(new RefreshPropertiesAttribute(RefreshProperties.Repaint));
			attributeList.Add(new CategoryAttribute( depth.ToString() + ". " + this.ownerType.Name + "( " + MethodUtils.GetMethodAccessShort(this.methodInfo) + ")" ));
			attributeList.Add(new DesignerAttribute(typeof (MethodDesigner), typeof (IDesigner)));
		}

		#region Override

		public override object GetValue(object component)
		{
			return valueHolder;
		}

		public override void SetValue(object component, object value)
		{
			if (ParametersCount == 0)
			{
				Invoke();
			}
		}

		public override Type ComponentType
		{
			get { return monitoredObject.GetType(); }
		}

		public override Type PropertyType
		{
			get { return valueHolder.GetType(); }
		}

		public override TypeConverter Converter
		{
			get
			{
				if (converter == null)
				{
					converter = new MethodEditingConverter(this);
				}
				return converter;
			}
		}

		public override string Description
		{
			get { return MethodUtils.GetMethodSignature(this.MethodInfo); }
		}

		#endregion

		#region Parameter Ops

		public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
		{
			if (propertyDescriptorCollection == null)
			{
				ResolveParameters();
				ArrayList list = parameterDescriptors.Clone() as ArrayList;
				list.Add(returnDescriptor);
				PropertyDescriptor[] paramDesc = (PropertyDescriptor[]) list.ToArray(typeof (PropertyDescriptor));
				propertyDescriptorCollection = new PropertyDescriptorCollection(paramDesc);
			}
			return propertyDescriptorCollection;
		}

		private void ResolveParameters()
		{
			if (parameterDescriptors != null)
				return;
			parameterDescriptors = MethodUtils.GetMethodParams(this);
			returnDescriptor = new ReturnParameterDescriptor(this);
		}

		#endregion

		#region Override

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override bool IsBrowsable
		{
			get { return true; }
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override void ResetValue(object component)
		{
		}

		public override bool CanResetValue(object component)
		{
			return true;
		}

		public override AttributeCollection Attributes
		{
			get { return base.Attributes; }
		}

		public override bool DesignTimeOnly
		{
			get { return false; }
		}

		#endregion

		#region Properties

		public MethodInfo MethodInfo
		{
			get { return methodInfo; }
		}

		public int ParametersCount
		{
			get { return methodInfo.GetParameters().Length; }
		}

		public bool IsVoidMethdod
		{
			get { return methodInfo.ReturnType == typeof (void); }
		}

		public object ValueOfLastRun
		{
			get { return valueOfLastRun; }
		}

		public void Invoke()
		{ // invoke the method
			if (parameterDescriptors == null)
				ResolveParameters();

			// Now, invoke
			object[] param = new object[parameterDescriptors.Count];
			for (int i = 0; i < parameterDescriptors.Count; i++)
			{
				ParameterPropertyDescriptor para = parameterDescriptors[i] as ParameterPropertyDescriptor;
				param[i] = para.GetValue(monitoredObject);
			}
			Invoke(param);
			returnDescriptor.SetValue(monitoredObject, valueOfLastRun);
		}

		private void Invoke(object[] param)
		{
			try
			{
				if ( monitoredObject != null )
				{	// instance
					StringBuilder builder = new StringBuilder();
					foreach ( object para in param )
					{
						builder.Append( CodeChangeLoggingSystem.Instance.PrepareValueToLog(para) );
						builder.Append(", ");
					}
					if ( builder.Length > 0 )
					{
						builder.Remove(builder.Length-2,2);
					}
					CodeChangeLoggingSystem.Instance.Log(HawkeyeUtils.GetControlName2(monitoredObject) + "." + methodInfo.Name + "( " + builder + " );");
				}

				if (monitoredObject is Form)
				{	// force an activate to make it easier for controls to relate to their parent form
					try
					{
						(monitoredObject as Form).Activate();
					}
					catch (Exception ex)
					{
						Trace.WriteLine(ex.ToString());
					}
				}

				valueOfLastRun = methodInfo.Invoke(monitoredObject, param);
				if (IsVoidMethdod)
					valueOfLastRun = "<void>";
			}
			catch (TargetInvocationException ex)
			{
				valueOfLastRun = ex.InnerException.ToString();
			}
			catch (Exception ex)
			{
				valueOfLastRun = ex.ToString();
			}
			return;
		}

		#endregion

		#region IShowSourceCodeDescriptor Members

		public void ShowSourceCode()
		{
			ReflectorRouter.Instance.ShowMethod(ownerType, methodInfo);
		}

		#endregion
	}
}