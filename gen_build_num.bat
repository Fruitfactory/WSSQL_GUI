@echo off

set mypath=%~dp0

set generator="Toolkits\wsgen.exe"

%generator% %mypath%

exit /b 0