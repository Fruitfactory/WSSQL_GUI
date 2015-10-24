@echo off

set DEVENV="%VS100COMNTOOLS%..\IDE\devenv.com"

set CONFIG=%1
if "%CONFIG%" == "" set CONFIG=Debug

set OUT_DIR=ROutput
if /i "%CONFIG%" == "Debug" set OUT_DIR=Output

rem ============================= Clean ==================================

if /i "%2"=="Rebuild" (

  call gen_build_num.bat	
  call build-plugin.bat		
  %DEVENV% Build.sln /Clean "%CONFIG%|Mixed Platforms"
  if ERRORLEVEL 1 goto FAILED

  del /q "%OUT_DIR%\*.ilk"
)

if ERRORLEVEL 1 goto FAILED

rem ============================= Build ==================================

del /q "%OUT_DIR%\*.log" >nul 2>&1

call gen_build_num.bat
call build-plugin.bat
%DEVENV% Build.sln /Build "%CONFIG%|Mixed Platforms" /Out "%OUT_DIR%\_Build_OutlookFinder.log" 
if ERRORLEVEL 1 goto FAILED

:EXIT
exit /b 0

rem ======================================================================

:FAILED
echo "Build failed"
pause
exit 1
