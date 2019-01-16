using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ACorns.Hawkeye.Core.Utils
{
	public static class ReflectionUtils
	{
		public static ParameterInfo[] GetEventParameters(EventInfo eventInfo)
		{
			MethodInfo invokeMethod = eventInfo.EventHandlerType.GetMethod("Invoke");
			ParameterInfo[] eventParams = invokeMethod.GetParameters();
			return eventParams;
		}
	}
}
