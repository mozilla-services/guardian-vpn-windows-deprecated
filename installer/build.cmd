@echo off
rem SPDX-License-Identifier: MIT
rem Copyright (C) 2019 WireGuard LLC. All Rights Reserved.
rem Copyright (C) 2019 Edge Security LLC. All Rights Reserved.

setlocal
set PATHEXT=.exe
SET PATH=%PATH%;%MOZ_FETCHES_DIR%

set WIX_CANDLE_FLAGS=-nologo -ext WiXUtilExtension
set WIX_LIGHT_FLAGS=-nologo -spdb -ext WixUtilExtension

if exist .deps\prepared goto :build
:installdeps
	rmdir /s /q .deps 2> NUL
	mkdir .deps || goto :error
	cd .deps || goto :error
	call :download wintun-x86-0.8.1.msm https://www.wintun.net/builds/wintun-x86-0.8.1.msm 5b47f83ffa9c361a360196d692f64755183e82c65f4753accc92087e6736af10 || goto :error
	call :download wintun-amd64-0.8.1.msm https://www.wintun.net/builds/wintun-amd64-0.8.1.msm af9644438a716f5a022052e3574ee0404c3e3309daff84889d656178fbc6b168 || goto :error
	cd .. || goto :error

:build
	call :msi x86 || goto :error
	call :msi x64 || goto :error

:success
	echo [+] Success.
	exit /b 0

:download
	echo [+] Downloading %1
	curl -#fLo %1 %2 || exit /b 1
	echo [+] Verifying %1
	for /f %%a in ('CertUtil -hashfile %1 SHA256 ^| findstr /r "^[0-9a-f]*$"') do if not "%%a"=="%~3" exit /b 1
	goto :eof

:msi
	if not exist "%~1" mkdir "%~1"
	echo [+] Compiling %1
	candle %WIX_CANDLE_FLAGS% -dPlatform=%1 -out "%~1\FirefoxPrivateNetworkVPN.wixobj" -arch %1 FirefoxPrivateNetworkVPN.wxs || exit /b %errorlevel%
	echo [+] Linking %1
	light %WIX_LIGHT_FLAGS% -out "%~1/FirefoxPrivateNetworkVPN.msi" "%~1\FirefoxPrivateNetworkVPN.wixobj" || exit /b %errorlevel%
	goto :eof

:error
	echo [-] Failed with error #%errorlevel%.
	cmd /c exit %errorlevel%
