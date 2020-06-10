# Installer
A WiX script for packaging up all binaries and supporting files in a convenient MSI file, suitable for further distribution.

## Building
A build script is included in this folder, which creates an installer for each platform and handles downloading all dependencies, such as [WiX](https://wixtoolset.org/) and [Wintun](https://www.wintun.net/). Don't attempt to run the scripts without building the [tunnel](../tunnel), followed by the [Firefox Private Network VPN client](../ui) for both x86 and x64 target platforms first.

If the build succeeds, `x86/FirefoxPrivateNetworkVPN.msi` and `x64/FirefoxPrivateNetworkVPN.msi` should appear.
