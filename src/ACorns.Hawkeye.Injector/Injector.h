// Injector.h

#pragma once

namespace ACorns
{
	namespace Hawkeye
	{
		namespace Injector
		{
			public interface class IHookInstall
			{
			public:
				virtual void OnInstallHook(array<System::Byte>^ data) = NULL;
			};

			public ref class HookHelper
			{
			private:

			public:
				static void InstallIdleHandler(int processID, int threadID, System::String^ assemblyLocation, System::String^ typeName, array<System::Byte >^ additionalData);
			};
		}
	}
}