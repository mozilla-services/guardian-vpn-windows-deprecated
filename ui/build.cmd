#@echo off

setlocal
set BUILDDIR=%~dp0
set PATHEXT=.exe
cd /d %BUILDDIR% || exit /b 1

:build
       cmd /c nuget.exe restore -SolutionDirectory .\ || goto :error
       "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\BuildTools\\MSBuild\\Current\\Bin\\amd64\\MSBuild.exe" /t:Rebuild /p:Configuration=Release /p:Platform="x64" || goto :error
	   cd "%BUILDDIR%\\bin" & zip MozillaVPN.zip -r *
	   exit /b 0

:error
	echo [-] Failed with error #%errorlevel%.
	cmd /c exit %errorlevel%
