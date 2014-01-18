@echo off
nant-0.92\bin\NAnt.exe -buildfile:wsuiTrial.build %1
date /t && time /t
pause