# Mozilla VPN for Windows
[![CircleCI](https://circleci.com/gh/mozilla-services/guardian-vpn-windows.svg?style=svg&circle-token=d0d916754d2f18a3ec876dcdf2c79f6b45b334e0)](https://circleci.com/gh/mozilla-services/guardian-vpn-windows)

## Structure

| Folder        | Description           | Language  |
| ------------- |:-------------|:-----:|
| `ui`          | VPN client UI and the tunnel service interface | C# |
| `tunnel`      | Wrapper around [wireguard-windows](https://git.zx2c4.com/wireguard-windows/about/)'s [embeddable-dll-service](https://git.zx2c4.com/wireguard-windows/tree/embeddable-dll-service) for running a [WireGuard](https://www.wireguard.com/) process as a service. | Go |
| `installer`   | WiX installer scripts, for setting up and creating an MSI installer | XML |
| `test`        | Integration and end-to-end tests | Go |

## Environment Preparation
- Nuget CLI: Download Nuget.exe CLI from [nuget.org](https://dist.nuget.org/win-x86-commandline/latest/nuget.exe). [More information from Microsoft Doc](https://docs.microsoft.com/nuget/consume-packages/install-use-packages-nuget-cli)
- Add MSBuild.exe to System Environment Path. For Visual Studio 2019 user, the file is placed at `C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin`

## Building

- First, [build the embeddable-dll-service's tunnel.dll](tunnel/README.md).
- Next, [build the the UI](ui/README.md).
- Finally, [build the installer](installer/README.md).

Alternatively, this may all be done at once, like so:

```
C:\guardian-vpn> tunnel\build.cmd
C:\guardian-vpn> cd ui
C:\guardian-vpn\ui> nuget.exe restore -SolutionDirectory .\
C:\guardian-vpn\ui> MSBuild.exe /t:Rebuild /p:Configuration=Release /p:Platform="x86"
C:\guardian-vpn\ui> MSBuild.exe /t:Rebuild /p:Configuration=Release /p:Platform="x64"
C:\guardian-vpn\ui> cd ..
C:\guardian-vpn> installer\build.cmd
C:\guardian-vpn> msiexec /i installer\x64\MozillaVPN.msi
```
## Testing

See [the instructions](test/README.md) in the `test` folder.

## Code of Conduct

This repository is governed by Mozilla's [Community Participation Guidelines](CODE_OF_CONDUCT.md)
and [Developer Etiquette Guidelines][etiquette].

[etiquette]: https://bugzilla.mozilla.org/page.cgi?id=etiquette.html
