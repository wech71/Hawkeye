using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace ACorns.Hawkeye.Utils.TypeConverters
{
	internal class ExposePublicPropertiesTypeConverter : AbstractDelegateTypeConverter
	{
		public ExposePublicPropertiesTypeConverter(TypeConverter originalConverter) : base(originalConverter)
		{
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			// try to convert to IList
			if (context != null && context.Instance != null && context.PropertyDescriptor!=null)
			{
				return DescriptorUtils.GetAllPropertiesCustom(context.PropertyDescriptor.GetValue(context.Instance), null).Count > 0;
			}
			else
			{
				return originalConverter.GetPropertiesSupported(context);
			}
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection col = DescriptorUtils.GetAllPropertiesCustom(context.PropertyDescriptor.GetValue(value), attributes);
			if ( col.Count > 0 )
			{
				return col;
			}
			else
			{
				return originalConverter.GetProperties(context, value, attributes);
			}
		}
	}
}
