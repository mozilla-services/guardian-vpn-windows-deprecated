@echo off
rem SPDX-License-Identifier: MIT
rem Copyright (C) 2019 WireGuard LLC. All Rights Reserved.
rem Copyright (C) 2019 Edge Security LLC. All Rights Reserved.

setlocal
set PATHEXT=.exe
set BUILDDIR=%~dp0
cd /d %BUILDDIR% || exit /b 1

set WIX_CANDLE_FLAGS=-nologo -ext WiXUtilExtension
set WIX_LIGHT_FLAGS=-nologo -spdb -ext WixUtilExtension

:build
	set WIX=%BUILDDIR%..\..\installer\.deps\wix\
	call :msi x64 || goto :error

:success
	echo [+] Success.
	exit /b 0

:msi
	if not exist "%~1" mkdir "%~1"
	echo [+] Compiling %1
	"%WIX%bin\candle" %WIX_CANDLE_FLAGS% -dPlatform=%1 -out "%~1\FirefoxPrivateNetworkMockVPN.wixobj" -arch %1 FirefoxPrivateNetworkMockVPN.wxs || exit /b %errorlevel%
	echo [+] Linking %1
	"%WIX%bin\light" %WIX_LIGHT_FLAGS% -out "%~1/FirefoxPrivateNetworkMockVPN.msi" "%~1\FirefoxPrivateNetworkMockVPN.wixobj" || exit /b %errorlevel%
	goto :eof

:error
	echo [-] Failed with error #%errorlevel%.
	cmd /c exit %errorlevel%
