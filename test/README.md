# Testing and deploying

## Automation Testing
### Overview
![Overview](./assets/Overview.png)

**CircleCI** is the CI/CD tool that handles all automations. There are two workflows setup in CircleCI. One workflow will be triggered by any pull request. The other one will be triggered automatically every midnight.

### Workflows
Both PR workflow and Nightly workflow have exactly the same steps. The difference between them is the trigger.

#### Checkout
- Checkout the latest code

#### Build Tunnel
- Run the script to build the WireGuard tunnel
```
.\tunnel\build.cmd
```

#### Build The Application
- Restore all required packages using `nuget`
```
nuget restore -SolutionDirectory ./
```
- Build the app 
    - Release - 64 bit 
    - Release - 32 bit
    - Debug_QA - 64 bit (this is only for QA testing)
```
MSBuild /t:Rebuild /p:Configuration=Release /p:Platform="x64" 
MSBuild /t:Rebuild /p:Configuration=Release /p:Platform="x86"
MSBuild /t:Rebuild /p:Configuration=Debug_QA /p:Platform="x64"
```

#### Build the MSI
```
.\installer\build.cmd 
```

#### Install the MSI
```
msiexec /log install.log /qn /i installer\x64\FirefoxPrivateNetworkVPN.msi
```

#### Initial Check
- Check the Uninstallation and Installation Process
- Check whether the Application installed or not
- Verify that `tunnel.dll` exists in the application folder
- Check whether tunnel driver installed or not

```
cd test
go test github.com/mozilla-services/guardian-vpn-windows/test/initial -v
```

#### Run Unit Tests
- Use `nunit3-console.exe` to run all unit tests and save the results into XML format
```
cd ui\Guardian.Tests\bin\x64\Debug_QA
nunit3-console.exe /result:.\test\result\unittests\result.xml FirefoxPrivateNetwork.Tests.dll
```

#### Run App In Debug_QA mode
```
Start-Process -FilePath ".\ui\src\bin\x64\Debug_QA\FirefoxPrivateNetworkVPN.exe"
```

#### Run Integration Tests
```
cd test
go test github.com/mozilla-services/guardian-vpn-windows/test/integrations -v | tee test.out
```

#### Generate Test Report
- Use `extent.exe` to transform UnitTest reports from XML to HTML
- Use `OpenCover` to generate code coverage report
- Use `github.com/jstemmer/go-junit-report` to generate integration tests reports in XML
- Use `xunit-viewer` to transform integration test reports from XML to HTML
- All reports are saved into `test/result/`

#### Deploy
- Save the MSI file into `test/result`
```
Copy-Item ".\installer\x64\FirefoxPrivateNetworkVPN.msi" -Destination ".\test\result"
```
