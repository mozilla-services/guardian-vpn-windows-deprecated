$env:BUILD_NUMBER = [System.Environment]::GetEnvironmentVariable("BUILD_NUMBER","Machine") 
$env:S3_BUCKET = [System.Environment]::GetEnvironmentVariable("S3_BUCKET","Machine") 
New-Item -Path "~/" -Name "test" -ItemType "directory"
If ([System.IO.File]::Exists("C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe")) {
    Start-Process -FilePath "powershell" -ArgumentList "C:\'Program Files (x86)'\'Windows Application Driver'\WinAppDriver.exe"
    
    Start-Sleep -Seconds 15  
}     

$timeout = new-timespan -Minutes 10
$sw = [diagnostics.stopwatch]::StartNew()
while ($sw.elapsed -lt $timeout){

    Start-Process -FilePath "powershell" -Wait -ArgumentList "C:\'Program Files (x86)'\'Microsoft Visual Studio'\2019\BuildTools\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe C:\Users\Administrator\guardian-vpn-windows\test\smoke\FirefoxPrivateVPNUITest\FirefoxPrivateVPNUITest\bin\Release\FirefoxPrivateVPNUITest.dll | Out-File -FilePath ~/test/result.txt"

    $log = Get-Content -Path ~/test/result.txt

    If ($log -Match 'Total tests') {

        Write-S3Object -BucketName $env:S3_BUCKET -Key smoke/$env:BUILD_NUMBER/smoke_test_result.txt -File ~/test/result.txt

        Write-Host 'Complete'

        break
    }
    start-sleep -seconds 10
}