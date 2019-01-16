using ACorns.Hawkeye.Core.Utils;
using ACorns.Hawkeye.Options;
using System;
using System.Collections;
using System.ComponentModel;

namespace ACorns.Hawkeye.Utils
{
	internal sealed class ConverterUtils
	{
		private ConverterUtils()
		{
		}

		public static PropertyDescriptorCollection GetEnumerableChildsAsProperties(ITypeDescriptorContext context, object owner, Attribute[] attributes)
		{
			if (owner == null)
			{
				return null;
			}
			if (owner is string)
			{
				return null;
			}
			IEnumerable enumerable = owner as IEnumerable;
			ArrayList properties = new ArrayList();
			int index = 0;
			foreach (object obj2 in enumerable)
			{
				if (obj2 is DictionaryEntry)
				{
					DictionaryEntry entry = (DictionaryEntry)obj2;
					string customName = HawkeyeUtils.ToStringAsNull(entry.Key);
					properties.Add(new SimpleChildDescriptor(owner, index, customName, obj2));
				}
				else
				{
					properties.Add(new SimpleChildDescriptor(owner, index, obj2));
				}
				index++;
				if (index >= ApplicationOptions.Instance.MaxIEnumerableChildsToShow)
				{
					properties.Add(new SimpleChildDescriptor("[more items where available]", "[max visible:" + ApplicationOptions.Instance.MaxIEnumerableChildsToShow + "]"));
					break;
				}
			}
			return DescriptorUtils.GetProperties(properties);
		}

		internal class SimpleChildDescriptor : AbstractPropertyDescriptor, IRealValueHolder
		{
			private readonly object childValue;
			private readonly int index;
			private readonly object owner;

			public SimpleChildDescriptor(string customName, object childValue)
				: base(customName)
			{
				this.owner = null;
				this.index = -1;
				this.childValue = childValue;
			}

			public SimpleChildDescriptor(object owner, int index, object childValue)
				: base("[Item:" + index.ToString() + "]")
			{
				this.owner = owner;
				this.index = index;
				this.childValue = childValue;
			}

			public SimpleChildDescriptor(object owner, int index, string customName, object childValue)
				: base(customName)
			{
				this.owner = owner;
				this.index = index;
				this.childValue = childValue;
			}

			public override object GetValue(object component)
			{
				return childValue;
			}

			public override string ToString()
			{
				if (this.childValue != null)
				{
					return this.childValue.ToString();
				}
				return null;
			}

			public override Type ComponentType
			{
				get
				{
					return this.owner.GetType();
				}
			}

			public override Type PropertyType
			{
				get
				{
					return base.GetType();
				}
			}

			public object RealValue
			{
				get
				{
					return this.childValue;
				}
			}
		}
	}
}

