@echo off

set SRC=.\MBODM.WADM
set DST=.\Publish

cls

if exist %DST% (rmdir /S /Q %DST%)
mkdir %DST%

echo.

echo Get WADM.UI
xcopy /Q /Y %SRC%\MBODM.WADM.UI\bin\Debug\*.* %DST%
rename %DST%\MBODM.WADM.UI.exe WADM.exe
echo.

echo Get WADM.Console
xcopy /Q /Y %SRC%\MBODM.WADM.Console\bin\Debug\MBODM.WADM.Console.exe %DST%
rename %DST%\MBODM.WADM.Console.exe WADMCMD.exe
echo.

del /Q %DST%\*.pdb
del /Q %DST%\*.config

pause
