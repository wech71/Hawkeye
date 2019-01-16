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

using ACorns.Hawkeye;
using ACorns.Hawkeye.Tools;
using ACorns.Hawkeye.Tools.Reflector;
using ACorns.Hawkeye.Utils;
using ACorns.Hawkeye.Utils.Generate;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using ACorns.Hawkeye.Core.Generate;
using ACorns.Hawkeye.Core.Utils.Accessors;

namespace ACorns.Hawkeye.Tabs.Events
{
    internal class EventPropertyDescriptor : AbstractPropertyDescriptor, IShowSourceCodeHandler, IDynamicInvoke
    {
        public EventPropertyDescriptor(object component, System.Reflection.EventInfo eventInfo, System.ComponentModel.EventHandlerList eventHandlerList) : base(eventInfo.Name)
        {
            this.component = component;
            this.eventInfo = eventInfo;
            this.eventHandlerList = eventHandlerList;
            this.converter = new EventInfoConverter(this);
        }

        private static void AddLoggingListener(ref EventController logController, object component, System.Reflection.EventInfo eventInfo, EventActionEnum action)
        {
            if (logController == null)
            {
                logController = ClassGenerator.Instance.GenerateHandler(eventInfo);
            }
            if (!logController.IsAttached)
            {
                logController.Attach(component, eventInfo, action);
            }
            ObjectEditor.Instance.ActiveEditor.ShowToolsWindow(ToolWindowEnum.EventLog);
        }

        protected override void FillAttributes(IList attributeList)
        {
            base.FillAttributes(attributeList);
            attributeList.Add(new RefreshPropertiesAttribute(RefreshProperties.Repaint));
            AttributeUtils.AddAllAttributes(attributeList, this.eventInfo, false);
        }

        public override object GetValue(object component)
        {
            return this.converter;
        }

        private void ReadListeners()
        {
            this.converter.ReadListeners();
        }

        private static void RemoveLoggingListener(EventController logController)
        {
            if (logController != null)
            {
                logController.Detach();
            }
        }

        public override void SetValue(object component, object value)
        {
            if (value is EnumHelperEnum)
            {
                switch (((EnumHelperEnum) value))
                {
                    case EnumHelperEnum.Refresh_Listeners:
                        this.ReadListeners();
                        return;

                    case EnumHelperEnum.Start_Logging:
                        EventPropertyDescriptor.AddLoggingListener(ref this.logController, component, this.eventInfo, EventActionEnum.CallLogging);
                        this.ReadListeners();
                        return;

                    case EnumHelperEnum.Remove_Logging:
                        EventPropertyDescriptor.RemoveLoggingListener(this.logController);
                        this.ReadListeners();
                        return;

                    case EnumHelperEnum.Break_Into_Debugger:
                        EventPropertyDescriptor.AddLoggingListener(ref this.breakIntoDebuggerController, component, this.eventInfo, EventActionEnum.BrakeIntoDebugger);
                        this.ReadListeners();
                        return;

                    case EnumHelperEnum.Remove_Break_Into_Debugger:
                        EventPropertyDescriptor.RemoveLoggingListener(this.breakIntoDebuggerController);
                        this.ReadListeners();
                        return;
                }
            }
        }

        public void ShowSourceCode()
        {
            ReflectorRouter.Instance.ShowEvent(this.eventInfo.DeclaringType, this.eventInfo);
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
				if (component == null)
					return typeof(Type);
				else
					return this.component.GetType();
            }
        }

        public override TypeConverter Converter
        {
            get
            {
                return this.converter;
            }
        }

        public System.ComponentModel.EventHandlerList EventHandlerList
        {
            get
            {
                return this.eventHandlerList;
            }
        }

        public System.Reflection.EventInfo EventInfo
        {
            get
            {
                return this.eventInfo;
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
                return typeof(EnumHelperEnum);
            }
        }


        private EventController breakIntoDebuggerController;
        private object component;
        private EventInfoConverter converter;
        private System.ComponentModel.EventHandlerList eventHandlerList;
        private System.Reflection.EventInfo eventInfo;
        private EventController logController;

		#region IDynamicInvoke Members

		public void DynamicInvoke()
		{
			Type target = (component is Type)?(component as Type):component.GetType();
			if (target != null)
			{
				MethodAccesor methodAcc = new MethodAccesor(target, "On" + eventInfo.Name);
				if (methodAcc.IsValid)
				{
					if (target is Type)
					{
						methodAcc.Invoke(null);
					}
					else
					{
						methodAcc.Invoke(component);
					}
				}
			}
		}

		#endregion
	}
}

