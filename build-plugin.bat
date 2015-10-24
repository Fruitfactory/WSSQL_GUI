@echo off

set mypath=%~dp0

SETLOCAL

set maven="%mypath%\Toolkits\\maven\\maven\\bin\\mvn.bat"
set netspy="%mypath%\Toolkits\\maven\\maven-nblib\\netbeans-eventspy.jar"

cd PSTRiverPlugin\

%maven% -Dmaven.ext.class.path=%netspy% -Dfile.encoding=UTF-8 install

ENDLOCAL

exit /b 0
