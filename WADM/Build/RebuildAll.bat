@echo off

rem DONT PANIC - THE FOLLOWING FOLDER CONTAINS ALSO .NET FRAMEWORK 4.5 - IT IS JUST A LITTLE WEIRD BUT 4.5 RESIDES IN THE 4.0 FOLDER

set MSBUILD=%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /target:Clean;Rebuild /verbosity:quiet /consoleloggerparameters:ErrorsOnly /nologo

echo.

echo Build ObservableModel
%MSBUILD% ..\Modules\MBODM.ObservableModel\MBODM.ObservableModel.sln
if errorlevel 1 goto end
echo.

echo Build Persister
%MSBUILD% ..\Modules\MBODM.WADM.Persister\MBODM.WADM.Persister.sln
if errorlevel 1 goto end
echo.

echo Build CurseParser
%MSBUILD% ..\Modules\MBODM.WOW.CurseParser\MBODM.WOW.CurseParser.sln
if errorlevel 1 goto end
echo.

echo Build DisplayNameExtension
%MSBUILD% ..\Modules\MBODM.WPF.DisplayNameExtension\MBODM.WPF.DisplayNameExtension.sln
if errorlevel 1 goto end
echo.

echo Build InvertBoolExtension
%MSBUILD% ..\Modules\MBODM.WPF.InvertBoolExtension\MBODM.WPF.InvertBoolExtension.sln
if errorlevel 1 goto end
echo.

echo Build RelayCommand
%MSBUILD% ..\Modules\MBODM.WPF.RelayCommand\MBODM.WPF.RelayCommand.sln
if errorlevel 1 goto end
echo.

echo Build StandardDialogs
%MSBUILD% ..\Modules\MBODM.WPF.StandardDialogs\MBODM.WPF.StandardDialogs.sln
if errorlevel 1 goto end
echo.

echo Build ZipExtractor
%MSBUILD% ..\Modules\MBODM.ZipExtractor\MBODM.ZipExtractor.sln
if errorlevel 1 goto end
echo.

echo Build AddonDownloadManager
%MSBUILD% ..\Modules\MBODM.WoW.AddonDownloadManager\MBODM.WoW.AddonDownloadManager.sln
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

pause
