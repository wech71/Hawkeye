using System.ComponentModel;
using System.Globalization;

namespace ACorns.Hawkeye.Utils
{
	internal class ProxyConvertedObjectHolder : IRealValueHolder, IStringValueHolder
	{
		private readonly object realValue;
		private readonly string originalString;

		public ProxyConvertedObjectHolder(object realValue, string originalString)
		{
			this.realValue = realValue;
			this.originalString = originalString;
		}
		#region IRealValueHolder Members

		public object RealValue
		{
			get
			{
				return realValue;
			}
		}

		#endregion

		public string OriginalString
		{
			get { return originalString; }
		}

		public override string ToString()
		{
			return originalString;
		}
	}


	/// <summary>
	/// Summary description for ProxyTypeConverter.
	/// </summary>
	internal class ProxyTypeConverter : AbstractDelegateTypeConverter
	{
		public ProxyTypeConverter(TypeConverter originalConverter)
			: base(originalConverter)
		{
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			object expected = base.ConvertFrom(context, culture, value);
			if ( value is string )
			{
				return new ProxyConvertedObjectHolder(expected, value as string);
			}
			else
			{
				return expected;
			}
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
		{
			if ( value is IRealValueHolder )
				value = (value as IRealValueHolder).RealValue;

			return base.ConvertTo (context, culture, value, destinationType);
		}

	}
}
