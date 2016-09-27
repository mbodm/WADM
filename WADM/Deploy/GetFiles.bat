@echo off

cls

if exist .\WADM (rmdir /S /Q .\WADM)
mkdir .\WADM

echo.

echo Get WADM.UI
xcopy /Q /Y ..\Modules\MBODM.WADM.UI\MBODM.WADM.UI\bin\Debug\*.* .\WADM
rename .\WADM\MBODM.WADM.UI.exe WADM.exe
echo.

echo Get WADM.Console
xcopy /Q /Y ..\Modules\MBODM.WADM.Console\MBODM.WADM.Console\bin\Debug\MBODM.WADM.Console.exe .\WADM
rename .\WADM\MBODM.WADM.Console.exe WADMCMD.exe
echo.

del /Q .\WADM\*.pdb
del /Q .\WADM\*.config

pause
