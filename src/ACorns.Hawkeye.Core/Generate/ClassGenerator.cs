using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using ACorns.Hawkeye.Core.Utils;
using ACorns.Hawkeye.Core.Options;

namespace ACorns.Hawkeye.Core.Generate
{
	public class ClassGenerator
	{
		private AssemblyBuilder assemblyBuilder;
		private AssemblyName assemblyName;
		private static readonly Type BASE_CLASS_TYPE = typeof(EventController);
		private Hashtable generatedTypes = new Hashtable();
		private static ClassGenerator instance = new ClassGenerator();
		private ModuleBuilder moduleBuilder;
		private bool saveGeneratedAssembly = CoreApplicationOptions.Instance.SaveGeneratedAssembly;

		private void GenerateAssembly()
		{
			if (this.assemblyBuilder == null)
			{
				this.assemblyName = new AssemblyName();
				this.assemblyName.Name = "Hawkeye.Event.Listener";
				this.assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(this.assemblyName, this.saveGeneratedAssembly ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run);
				if (this.saveGeneratedAssembly)
				{
					this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule("HawkeyeEvents", "HawkeyeEvents.dll");
				}
				else
				{
					this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule("HawkeyeEvents");
				}
			}
		}

		private Type GenerateEventConsumerType(EventInfo eventInfo, Type baseClassType, string method)
		{
			int index;
			string name = "EventListener" + eventInfo.EventHandlerType.Name;
			this.GenerateAssembly();
			TypeBuilder builder = this.moduleBuilder.DefineType(name, TypeAttributes.Public, baseClassType);
			ParameterInfo[] eventParameters = ReflectionUtils.GetEventParameters(eventInfo);
			Type[] parameterTypes = new Type[eventParameters.Length];
			for (index = 0; index < parameterTypes.Length; index++)
			{
				parameterTypes[index] = eventParameters[index].ParameterType;
			}
			Type returnType = typeof(void);
			ILGenerator iLGenerator = builder.DefineMethod("HandleEvent", MethodAttributes.HideBySig | MethodAttributes.Public, returnType, parameterTypes).GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldarg_0);
			for (index = 0; index < parameterTypes.Length; index++)
			{
				iLGenerator.Emit(OpCodes.Ldarg, (int)(((short)index) + 1));
			}
			MethodInfo meth = baseClassType.GetMethod(method, parameterTypes);
			iLGenerator.Emit(OpCodes.Call, meth);
			iLGenerator.Emit(OpCodes.Ret);
			Type type2 = builder.CreateType();
			if (this.saveGeneratedAssembly)
			{
				this.assemblyBuilder.Save("Hawkeye.Generated.dll");
			}
			return type2;
		}

		public EventController GenerateHandler(EventInfo eventInfo)
		{
			return this.GenerateHandler(eventInfo, BASE_CLASS_TYPE, "GenericEventHandler");
		}

		public EventController GenerateHandler(EventInfo eventInfo, Type baseClassType, string method)
		{
			Type type = null;
			type = this.generatedTypes[eventInfo.EventHandlerType] as Type;
			if (type == null)
			{
				type = this.GenerateEventConsumerType(eventInfo, baseClassType, method);
				this.generatedTypes[eventInfo.EventHandlerType] = type;
			}
			EventController controller = Activator.CreateInstance(type) as EventController;
			controller.EventName = eventInfo.Name;
			return controller;
		}

		public static ClassGenerator Instance
		{
			get
			{
				return instance;
			}
		}
	}
}

