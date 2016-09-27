@echo off

rem DONT PANIC - THE FOLLOWING FOLDER CONTAINS ALSO .NET FRAMEWORK 4.5 - IT IS JUST A LITTLE WEIRD BUT 4.5 RESIDES IN THE 4.0 FOLDER

set MSBUILD=%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /target:Clean;Rebuild /verbosity:quiet /consoleloggerparameters:ErrorsOnly /nologo

echo.

echo Clean ObservableModel
rmdir /Q /S ..\Modules\MBODM.ObservableModel\MBODM.ObservableModel\bin
rmdir /Q /S ..\Modules\MBODM.ObservableModel\MBODM.ObservableModel\obj
echo.

echo Clean Persister
rmdir /Q /S ..\Modules\MBODM.WADM.Persister\MBODM.WADM.Persister\bin
rmdir /Q /S ..\Modules\MBODM.WADM.Persister\MBODM.WADM.Persister\obj
echo.

echo Clean CurseParser
rmdir /Q /S ..\Modules\MBODM.WOW.CurseParser\MBODM.WOW.CurseParser\bin
rmdir /Q /S ..\Modules\MBODM.WOW.CurseParser\MBODM.WOW.CurseParser\obj
echo.

echo Clean DisplayNameExtension
rmdir /Q /S ..\Modules\MBODM.WPF.DisplayNameExtension\MBODM.WPF.DisplayNameExtension\bin
rmdir /Q /S ..\Modules\MBODM.WPF.DisplayNameExtension\MBODM.WPF.DisplayNameExtension\obj
echo.

echo Clean InvertBoolExtension
rmdir /Q /S ..\Modules\MBODM.WPF.InvertBoolExtension\MBODM.WPF.InvertBoolExtension\bin
rmdir /Q /S ..\Modules\MBODM.WPF.InvertBoolExtension\MBODM.WPF.InvertBoolExtension\obj
echo.

echo Clean RelayCommand
rmdir /Q /S ..\Modules\MBODM.WPF.RelayCommand\MBODM.WPF.RelayCommand\bin
rmdir /Q /S ..\Modules\MBODM.WPF.RelayCommand\MBODM.WPF.RelayCommand\obj
echo.

echo Clean StandardDialogs
rmdir /Q /S ..\Modules\MBODM.WPF.StandardDialogs\MBODM.WPF.StandardDialogs\bin
rmdir /Q /S ..\Modules\MBODM.WPF.StandardDialogs\MBODM.WPF.StandardDialogs\obj
echo.

echo Clean ZipExtractor
rmdir /Q /S ..\Modules\MBODM.ZipExtractor\MBODM.ZipExtractor\bin
rmdir /Q /S ..\Modules\MBODM.ZipExtractor\MBODM.ZipExtractor\obj
echo.

echo Clean AddonDownloadManager
rmdir /Q /S ..\Modules\MBODM.WoW.AddonDownloadManager\MBODM.WoW.AddonDownloadManager\bin
rmdir /Q /S ..\Modules\MBODM.WoW.AddonDownloadManager\MBODM.WoW.AddonDownloadManager\obj
echo.

echo Clean Console
rmdir /Q /S ..\Modules\MBODM.WADM.Console\MBODM.WADM.Console\bin
rmdir /Q /S ..\Modules\MBODM.WADM.Console\MBODM.WADM.Console\obj
echo.

echo Clean UI
rmdir /Q /S ..\Modules\MBODM.WADM.UI\MBODM.WADM.UI\bin
rmdir /Q /S ..\Modules\MBODM.WADM.UI\MBODM.WADM.UI\obj
echo.

pause
