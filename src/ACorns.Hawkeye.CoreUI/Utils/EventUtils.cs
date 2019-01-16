using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace ACorns.Hawkeye.Utils
{
	internal sealed class EventUtils
	{
		private static Hashtable delegateParams = new Hashtable();

		static EventUtils()
		{
			AddDelegateParam("sender", typeof(object), new CreateObjectHandler(EventUtils.EventSender));
			AddDelegateParam("e", typeof(EventArgs), new CreateObjectHandler(EventUtils.EventArgsBuilder));
			AddDelegateParam(null, typeof(CancelEventArgs), new CreateObjectHandler(EventUtils.CancelEventArgsBuilder));
			AddDelegateParam(null, typeof(ListChangedEventHandler), new CreateObjectHandler(EventUtils.ListChangedEventHandlerBuilder));
			AddDelegateParam(null, typeof(PropertyChangedEventArgs), new CreateObjectHandler(EventUtils.PropertyChangedEventArgsBuilder));
			AddDelegateParam(null, typeof(RefreshEventArgs), new CreateObjectHandler(EventUtils.RefreshEventArgsBuilder));
		}

		private EventUtils()
		{
		}

		private static void AddDelegateParam(string paramName, System.Type paramType, CreateObjectHandler objectHandler)
		{
			if (paramName == null)
			{
				delegateParams.Add(paramType.FullName + ".", objectHandler);
			}
			else
			{
				delegateParams.Add(paramType.FullName + "::" + paramName, objectHandler);
			}
		}

		private static object CancelEventArgsBuilder(ParameterInfo paramter, object sender)
		{
			return new CancelEventArgs();
		}

		public static void DynamicInvoke(Delegate handler, object sender)
		{
			if (handler.Method != null)
			{
				Exception exception;
				ParameterInfo[] parameterInfos = handler.Method.GetParameters();
				object[] callParams = new object[parameterInfos.Length];
				try
				{
					FillInDefaultParams(parameterInfos, callParams, sender);
				}
				catch (Exception exception1)
				{
					exception = exception1;
					Trace.WriteLine(string.Concat(new object[] { "Could not fill in params required to invoke:", handler, ".", exception.ToString() }));
					MessageBox.Show(string.Concat(new object[] { "Could not fill in params required to invoke:", handler, ".", exception.Message }), HawkeyeAppUtils.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				try
				{
					handler.DynamicInvoke(callParams);
				}
				catch (Exception exception2)
				{
					exception = exception2;
					Trace.WriteLine(string.Concat(new object[] { "Exception invoking :", handler, ".", exception.ToString() }));
					MessageBox.Show(string.Concat(new object[] { "Could not fill in params required to invoke:", handler, ".", exception.Message }), HawkeyeAppUtils.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
		}

		private static object EventArgsBuilder(ParameterInfo paramter, object sender)
		{
			return EventArgs.Empty;
		}

		private static object EventSender(ParameterInfo paramter, object sender)
		{
			return sender;
		}

		private static void FillInDefaultParams(ParameterInfo[] parameterInfos, object[] callParams, object sender)
		{
			for (int i = 0; i < parameterInfos.Length; i++)
			{
				ParameterInfo paramter = parameterInfos[i];
				CreateObjectHandler delegateParamHandler = GetDelegateParamHandler(paramter.Name, paramter.ParameterType);
				object obj2 = null;
				if (delegateParamHandler == null)
				{
					try
					{
						obj2 = Activator.CreateInstance(paramter.ParameterType);
					}
					catch (Exception exception)
					{
						Trace.WriteLine(string.Concat(new object[] { "Could not create param:", i, " type:", paramter.ParameterType.FullName, ".", exception.ToString() }));
					}
				}
				else
				{
					obj2 = delegateParamHandler(paramter, sender);
				}
				callParams[i] = obj2;
			}
		}

		private static CreateObjectHandler GetDelegateParamHandler(string paramName, System.Type paramType)
		{
			CreateObjectHandler handler = delegateParams[paramType.FullName + "::" + paramName] as CreateObjectHandler;
			if (handler != null)
			{
				return handler;
			}
			return (delegateParams[paramType.FullName + "."] as CreateObjectHandler);
		}

		public static ParameterInfo[] GetEventParameters(EventInfo eventInfo)
		{
			return eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters();
		}

		private static object ListChangedEventHandlerBuilder(ParameterInfo paramter, object sender)
		{
			return new ListChangedEventArgs(ListChangedType.Reset, -1);
		}

		private static object PropertyChangedEventArgsBuilder(ParameterInfo paramter, object sender)
		{
			return new PropertyChangedEventArgs(null);
		}

		private static object RefreshEventArgsBuilder(ParameterInfo paramter, object sender)
		{
			return new RefreshEventArgs(sender);
		}

		private delegate object CreateObjectHandler(ParameterInfo paramter, object sender);
	}
}

