@echo off
IF dummy==dummy%2 (
nant-0.92\bin\NAnt.exe -buildfile:wsui.build %1 -D:project.build.type=Debug
) ELSE (
nant-0.92\bin\NAnt.exe -buildfile:wsui.build %1 -D:project.build.type=%2
)
date /t && time /t
pause