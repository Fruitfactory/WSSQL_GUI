@echo off
nant-0.92\bin\NAnt.exe -buildfile:deployGIT.build deploy
date /t && time /t
pause