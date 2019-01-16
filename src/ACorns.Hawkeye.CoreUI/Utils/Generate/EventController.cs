using ACorns.Hawkeye.Tools.Logging;
using ACorns.Hawkeye.Utils;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace ACorns.Hawkeye.Utils.Generate
{
/*	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class EventController
	{
		protected EventActionEnum eventAction;
		protected EventGeneratedHandler eventHandlerDelegate;
		protected EventInfo eventInfo;
		protected string eventName;
		protected Delegate handler;
		protected bool isAttached = false;
		protected string[] parameterNames;
		protected object target;

		public void Attach(object target, EventInfo eventInfo, EventActionEnum actionType)
		{
			try
			{
				this.target = target;
				this.eventInfo = eventInfo;
				this.eventAction = actionType;
				this.handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, "HandleEvent");
				ParameterInfo[] eventParameters = EventUtils.GetEventParameters(eventInfo);
				this.parameterNames = new string[eventParameters.Length];
				for (int i = 0; i < eventParameters.Length; i++)
				{
					this.parameterNames[i] = eventParameters[i].Name;
				}
				EventControllers.Instance.Add(this);
				eventInfo.AddEventHandler(target, this.handler);
				this.isAttached = true;
			}
			finally
			{
			}
		}

		public void Detach()
		{
			this.isAttached = false;
			EventControllers.Instance.Remove(this);
			this.eventInfo.RemoveEventHandler(this.target, this.handler);
		}

		public void DynamicAttach(EventInfo eventInfo, string fieldName)
		{
			try
			{
				this.eventInfo = eventInfo;
				this.handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, "HandleEvent");
				ParameterInfo[] eventParameters = EventUtils.GetEventParameters(eventInfo);
				this.parameterNames = new string[eventParameters.Length];
				for (int i = 0; i < eventParameters.Length; i++)
				{
					this.parameterNames[i] = eventParameters[i].Name;
				}
				EventControllers.Instance.Add(this);
				FieldAccesor accesor = new FieldAccesor(eventInfo.DeclaringType, fieldName);
				accesor.Get();
				Delegate newValue = Delegate.Combine(accesor.Value as Delegate, this.handler);
				accesor.Set(newValue);
				this.isAttached = true;
			}
			finally
			{
			}
		}

		public void GenericEventHandler()
		{
			this.Handle(null);
		}

		public void GenericEventHandler(object param1)
		{
			this.Handle(new object[] { param1 });
		}

		public void GenericEventHandler(object param1, object param2)
		{
			this.Handle(new object[] { param1, param2 });
		}

		public void GenericEventHandler(object param1, object param2, object param3)
		{
			this.Handle(new object[] { param1, param2, param3 });
		}

		public void GenericEventHandler(object param1, object param2, object param3, object param4)
		{
			this.Handle(new object[] { param1, param2, param3, param4 });
		}

		private void Handle(object[] parameters)
		{
			switch (this.eventAction)
			{
				case EventActionEnum.CallLogging:
					EventLoggingSystem.Instance.Log(this.target, this.eventInfo.Name, parameters, this.parameterNames);
					break;

				case EventActionEnum.BrakeIntoDebugger:
					Debugger.Break();
					break;

				case EventActionEnum.DelegateCall:
					this.eventHandlerDelegate(this, parameters);
					break;
			}
		}

		public void HandleEvent(string text1, IntPtr ptr1, int num1)
		{
		}

		public override string ToString()
		{
			return ("[Hawkeye " + this.eventAction.ToString() + " listener]");
		}

		public EventActionEnum EventAction
		{
			get
			{
				return this.eventAction;
			}
			set
			{
				this.eventAction = value;
			}
		}

		public EventGeneratedHandler EventHandlerDelegate
		{
			get
			{
				return this.eventHandlerDelegate;
			}
			set
			{
				this.eventHandlerDelegate = value;
				this.eventAction = EventActionEnum.DelegateCall;
			}
		}

		public string EventName
		{
			get
			{
				return this.eventName;
			}
			set
			{
				this.eventName = value;
			}
		}

		public bool IsAttached
		{
			get
			{
				return this.isAttached;
			}
		}

		public object Target
		{
			get
			{
				return this.target;
			}
		}
	}*/
}

