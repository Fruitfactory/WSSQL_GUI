@echo off
nant-0.92\bin\NAnt.exe -buildfile:wsui.build %1
date /t && time /t
pause