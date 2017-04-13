@echo off

set mypath=%~dp0

set arg=%1
set sourcefile="%mypath%\OF.EmailParser\\target\\*.jar"

set out_dir=Routput
if /i "%arg%"=="Debug" set out_dir=Output

if not exist "%mypath%\%out_dir%\java" (
    mkdir "%mypath%\%out_dir%\java"
)

rem ======= Copy file =======

xcopy "%mypath%\OF.EmailParser\target\*.jar" "%mypath%\%out_dir%\java"  /y
