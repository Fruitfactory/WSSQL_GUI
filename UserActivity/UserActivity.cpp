// UserActivity.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

long GetIdleTime()
{
	LASTINPUTINFO pLastInputInfo = {0};
	pLastInputInfo.cbSize = sizeof(LASTINPUTINFO);
	GetLastInputInfo(&pLastInputInfo);
#ifdef X86
	long tickCount = GetTickCount();
	long result = tickCount - pLastInputInfo.dwTime;
	return result;
#else
	ULONGLONG tickCount = GetTickCount64();
	long result = tickCount - pLastInputInfo.dwTime;
	return result;

#endif
	
}