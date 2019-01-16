// Injector.h

#pragma once

namespace ACorns
{
namespace Hawkeye
{
namespace Injector
{
	public __gc __interface IHookInstall
	{
	public:
		void OnInstallHook(System::Byte data[]) = 0;
	};
	
	public __gc class HookHelper
	{
	private:

	public:
		static void InstallIdleHandler(int processID, int threadID, System::String* assemblyLocation, System::String* typeName, System::Byte additionalData[]);
	};
}
}
}