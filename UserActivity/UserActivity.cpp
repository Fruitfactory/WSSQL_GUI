// UserActivity.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

int GetIndldeTime()
{
	PLASTINPUTINFO plAstInputInfo = new LASTINPUTINFO();
	if(GetLastInputInfo(plAstInputInfo))
	{
		int tickCount = GetTickCount();
		int result = tickCount - plAstInputInfo->dwTime;
		delete plAstInputInfo;
		plAstInputInfo = nullptr;
		return result;
	}
	return -1;
}