using System;
using System.Text;
using System.Windows.Forms;

namespace ACorns.Hawkeye.Core.Utils
{
	public static class HawkeyeUtils
	{
		public static string EnumToString(Enum value)
		{
			string enumType = value.GetType().Name;

			Attribute[] attributes = (Attribute[])value.GetType().GetCustomAttributes(typeof(FlagsAttribute), false);
			if (attributes != null && attributes.Length > 0)
			{
				long longValue = Convert.ToInt64(value);
				// try to build a flags out of this enum
				StringBuilder builder = new StringBuilder();
				string flagZero = "";
				foreach (Enum enumValue in Enum.GetValues(value.GetType()))
				{
					long flagValue = Convert.ToInt64(enumValue);
					if (flagValue != 0)
					{
						if ((longValue & flagValue) == flagValue)
						{
							builder.Append(EnumValueToString(enumType, enumValue));
							builder.Append(" | ");
						}
					}
					else
					{
						flagZero = EnumValueToString(enumType, enumValue);
					}
				}
				if (builder.Length > 3)
					builder.Remove(builder.Length - 3, 3);
				else
					builder.Append(flagZero);
				return builder.ToString();
			}
			else
			{
				return EnumValueToString(enumType, value);
			}
		}

		private static string EnumValueToString(string enumType, Enum enumValue)
		{
			return enumType + "." + enumValue;
		}

		public static string ToEmptyString(object value)
		{
			if (value == null)
				return String.Empty;
			else
				return value.ToString();
		}
		public static string ToStringAsNull(object value)
		{
			if (value == null)
				return "<null>";
			else
				return value.ToString();
		}

		public static string GetControlName(object control)
		{
			Control ctl = control as Control;
			if (ctl != null && ctl.Name != null)
			{
				return ctl.Name;
			}
			else
				return String.Empty;
		}

		public static string GetControlName2(object control)
		{
			string controlName = GetControlName(control);
			if (controlName.Length > 0)
			{
				return controlName + ".";
			}
			else
			{
				return String.Empty;
			}
		}

		public static string GetControlName2OrToString(object control)
		{
			string controlName = GetControlName(control);
			if (controlName.Length > 0)
			{
				return controlName + ".";
			}
			else
			{
				return control.ToString() + ".";
			}
		}
	}
}
