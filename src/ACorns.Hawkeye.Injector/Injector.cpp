// This is the main DLL file.

#include "stdafx.h"
#include "Injector.h"

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Reflection;
using namespace ACorns::Hawkeye::Injector;

String^ GetAssemblyPath()
{
	return Assembly::GetExecutingAssembly()->Location;	
}

HMODULE GetThisModuleHandle()
{
	pin_ptr<const wchar_t> wszModule = PtrToStringChars(GetAssemblyPath());
	return GetModuleHandle(wszModule);
}

const String^ GetFileMappingName(int processId = GetCurrentProcessId(), int threadId = GetCurrentThreadId())
{
	String^ fileMappingName = String::Format("Hawkeye_Hook_Comm{0}_{1}", processId, threadId);
	return fileMappingName;
}

const String^ GetEventName(int processId = GetCurrentProcessId(), int threadId = GetCurrentThreadId())
{
	String^  eventName = String::Format("Hawkeye_Hook_Event_{0}_{1}", processId, threadId);
	return eventName;
}

template<class T>
void _Win32X(T t)
{
	if (t == NULL)
		Marshal::ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
}

struct WFAVCommunicationData
{
	BOOL m_bErrorFlag;
	HANDLE m_hFileMap; //Stored to help in cleanup
	HANDLE m_hEvent; //To be set when assembly is injected
	DWORD m_dwAssemblyPathSize;
	DWORD m_dwTypeNameSize;
	DWORD m_dwDataSize; //Length of additional data
	BYTE ab[1];
	
	//Assembly name starts at offset 0
	LPWSTR _GetAssemblyName()
	{
		return reinterpret_cast<LPWSTR>(&ab[0]);
	}
	
	//Immediately after the assembly name
	LPWSTR _GetTypeName()
	{
		return reinterpret_cast<LPWSTR>(&ab[m_dwAssemblyPathSize]);
	}
	
	//Immediately after the type name
	BYTE* _GetData()
	{
		return (&ab[m_dwAssemblyPathSize + m_dwTypeNameSize]);
	}
	
	//managed variants of the above methods
	String^ GetAssemblyName()
	{
		return gcnew String(_GetAssemblyName());
	}
	
	String^ GetTypeName()
	{
		return gcnew String(_GetTypeName());
	}
	
	array<Byte>^ GetData()
	{
		array<Byte>^  data = gcnew array<Byte>(m_dwDataSize);
		Marshal::Copy(IntPtr(_GetData()), data, 0, m_dwDataSize);

		return data;
	}
	
	void Cleanup()
	{
		if (m_hEvent)
			CloseHandle(m_hEvent);

		if (m_hFileMap)
			CloseHandle(m_hFileMap);
	}
	
	static WFAVCommunicationData* Construct(int processID, int threadID, String^ assemblyPath, String^ typeName, array<Byte>^ additionalData)
	{
		DWORD dwAssemblyPathSize = (assemblyPath->Length + 1)*sizeof(WCHAR);
		DWORD dwTypeNameSize = (typeName->Length + 1)*sizeof(WCHAR);
		DWORD dwDataSize = additionalData->Length;
	
		DWORD cbTotalLen = 
				dwAssemblyPathSize + dwTypeNameSize + dwDataSize + //Length of data
				sizeof(DWORD)*3 + //Length of the data length holders
				sizeof(WFAVCommunicationData); //Size of structure itself
		
		WFAVCommunicationData* pwfcd = NULL;

		try
		{
			pin_ptr<const wchar_t> wszFileMappingName = PtrToStringChars(GetFileMappingName(processID, threadID));
			HANDLE hFileMapping = CreateFileMapping(NULL, NULL, PAGE_READWRITE, 0, cbTotalLen, wszFileMappingName);
			
			int err = GetLastError();

			//Could not be created
			_Win32X(hFileMapping);

			if (err == ERROR_ALREADY_EXISTS)
			{
				CloseHandle(hFileMapping);
				throw gcnew InvalidOperationException(); //Only one process can inject at a time
			}

			pwfcd = (WFAVCommunicationData*)MapViewOfFile(hFileMapping, FILE_MAP_ALL_ACCESS, 0, 0, 0);
			
			_Win32X(pwfcd);
			
			pwfcd->m_bErrorFlag = FALSE;
			pwfcd->m_hFileMap = hFileMapping;
			pwfcd->m_hEvent = NULL;
			pwfcd->m_dwAssemblyPathSize = dwAssemblyPathSize;
			pwfcd->m_dwDataSize = dwDataSize;
			pwfcd->m_dwTypeNameSize = dwTypeNameSize;
			
			pin_ptr<const wchar_t> wszEventName = PtrToStringChars(GetEventName(processID, threadID));
			pwfcd->m_hEvent = CreateEvent(NULL, FALSE, FALSE, wszEventName);
			
			if (pwfcd->m_hEvent == NULL)
				Marshal::ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));

			//Now copy the strings
			
			pin_ptr<const wchar_t> wszAssemblyPath = PtrToStringChars(assemblyPath);
			pin_ptr<const wchar_t> wszTypeName = PtrToStringChars(typeName);
			
			lstrcpy(pwfcd->_GetAssemblyName(), wszAssemblyPath);
			lstrcpy(pwfcd->_GetTypeName(), wszTypeName);
			Marshal::Copy(additionalData, 0, IntPtr(pwfcd->_GetData()), pwfcd->m_dwDataSize);
		
			/*HANDLE hTargetProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, processID);
			_Win32X(hTargetProcess);
			CloseHandle(hTargetProcess);*/
		}
		catch(Exception^ e)
		{
			if (pwfcd)
			{
				pwfcd->Cleanup();
			}

			throw e;
		}

		return pwfcd;
	}
};

//The hook proc
extern "C" LRESULT CALLBACK LocalHookProc(int code, DWORD wParam, LONG lParam)
{
	if (code < 0)
		return CallNextHookEx(NULL, code, wParam, lParam);

	LRESULT dwRet;
	
	HANDLE hMap = NULL;
	HANDLE hEvent = NULL;
	BOOL bSuccess = false;
	WFAVCommunicationData* pwfcd = NULL;

	try
	{
		pin_ptr<const wchar_t> wszFileMappingName = PtrToStringChars(GetFileMappingName());
		hMap = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, wszFileMappingName);
		
		_Win32X(hMap);
		
		pin_ptr<const wchar_t> wszEventName = PtrToStringChars(GetEventName());
		hEvent = OpenEvent(EVENT_ALL_ACCESS, FALSE, wszEventName);
		
		_Win32X(hEvent);
		
		pwfcd = (WFAVCommunicationData*)MapViewOfFile(hMap, FILE_MAP_ALL_ACCESS, 0, 0, 0);
		
		if (!pwfcd)
			Marshal::ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
		
		if (hEvent)
		{
			SetEvent(hEvent);
		}

		Assembly^ assem = Assembly::LoadFrom(pwfcd->GetAssemblyName());
		String^ typeName = pwfcd->GetTypeName();
		Type^ type = assem->GetType(typeName, true, true);

		Object^ newObj = Activator::CreateInstance(type);
#if NET2
		MethodInfo^ pInstallHook = type->GetMethod("OnInstallHook");
		array<Object^>^ params = gcnew array<Object^> { pwfcd->GetData() };
		pInstallHook->Invoke(newObj, params);
#else
		IHookInstall^ hookinstall = (IHookInstall^)(newObj);
		hookinstall->OnInstallHook( pwfcd->GetData() );
#endif

		bSuccess = true;
	}
	catch(Exception^ e)
	{
		System::Diagnostics::Trace::WriteLine(e->ToString());
		
		if (pwfcd)
			pwfcd->m_bErrorFlag = TRUE;

		if (hEvent)
		{
			SetEvent(hEvent);
		}
	}
	
	if (hMap)
		CloseHandle(hMap);

	if (hEvent)	
		CloseHandle(hEvent);

	dwRet = CallNextHookEx(NULL, code, wParam, lParam);

	return dwRet;
}

void HookHelper::InstallIdleHandler(int processID, int threadID, System::String^ assemblyLocation, System::String^ typeName, array<System::Byte >^ additionalData)
{
	WFAVCommunicationData* pwfcd = WFAVCommunicationData::Construct(processID, threadID, assemblyLocation, typeName, additionalData);

	HANDLE	hFile = CreateFile(L"C:\\Temp\\hawk.txt", GENERIC_WRITE, 0, NULL, CREATE_NEW, FILE_ATTRIBUTE_NORMAL, NULL);
	CloseHandle(hFile);

	__try
	{
		HMODULE hMod = GetThisModuleHandle();
		HOOKPROC proc = (HOOKPROC)GetProcAddress(hMod, "LocalHookProc");
		HHOOK hhook = SetWindowsHookEx(WH_GETMESSAGE, proc, hMod, threadID);

		if (hhook)
		{
			//Post a simple message
			PostThreadMessage(threadID, WM_NULL, 0, 0);

			bool bError = (WaitForSingleObject(pwfcd->m_hEvent, 2*1000000) != WAIT_OBJECT_0);
			
			UnhookWindowsHookEx(hhook);

			if (bError || pwfcd->m_bErrorFlag)
				throw gcnew ApplicationException("Error waiting for hook to be setup");
		}
		else
			Marshal::ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
	}
	__finally
	{
		pwfcd->Cleanup();
	}
}

