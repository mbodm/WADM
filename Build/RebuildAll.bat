@echo off

rem DONT PANIC - THE FOLLOWING FOLDER CONTAINS ALSO .NET FRAMEWORK 4.5 - IT IS JUST A LITTLE WEIRD BUT 4.5 RESIDES IN THE 4.0 FOLDER

set MSBUILD=%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /target:Clean;Rebuild /verbosity:quiet /consoleloggerparameters:ErrorsOnly /nologo

echo.

echo Build CurseParser
%MSBUILD% ..\Modules\MBODM.WoW.CurseParser\MBODM.WoW.CurseParser.sln
if errorlevel 1 goto end
echo.

echo Build AddonDownloadManager
%MSBUILD% ..\Modules\MBODM.WoW.AddonDownloadManager\MBODM.WoW.AddonDownloadManager.sln
if errorlevel 1 goto end
echo.

echo Build Persister
%MSBUILD% ..\Modules\MBODM.WADM.Persister\MBODM.WADM.Persister.sln
if errorlevel 1 goto end
echo.

echo Build Console
%MSBUILD% ..\Modules\MBODM.WADM.Console\MBODM.WADM.Console.sln
if errorlevel 1 goto end
echo.

echo Build UI
%MSBUILD% ..\Modules\MBODM.WADM.UI\MBODM.WADM.UI.sln
if errorlevel 1 goto end
echo.

:end

echo.
echo.

pause
