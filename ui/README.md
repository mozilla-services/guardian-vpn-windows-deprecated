# Mozilla VPN UI

The main application housing the UI, the elevated service manager and the WireGuard tunnel interface.

## Stack

C#, .NET, WPF

## Requirements

- x64 or x86 build of Windows 10
- Any CPU capable of running Windows 10 (dual core recommended)
- 1GB RAM minimum (2GB recommended)

## Building

First ensure that the tunnel library is built, and then build the UI using MSBuild:

```
C:\guardian-vpn\ui> ..\tunnel\build.cmd
C:\guardian-vpn\ui> nuget.exe restore -SolutionDirectory .\
C:\guardian-vpn\ui> MSBuild.exe /t:Rebuild /p:Configuration=Release /p:Platform="x64"
C:\guardian-vpn\ui> MSBuild.exe /t:Rebuild /p:Configuration=Release /p:Platform="x86"
```
