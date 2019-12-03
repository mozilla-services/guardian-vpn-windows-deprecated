@echo on
REM ** Be sure to install these nugets:
REM ** NUnit.ConsoleRunner
REM ** OpenCover
REM ** ReportGenerator
REM **
REM ** All paths should be entered without quotes

REM ** SET TestResultsFileProjectName=CalculatorResults
SET TestResultsFileProjectName=GuardianResults

REM ** SET DLLToTestRelativePath=Calculator\bin\Debug\MyCalc.dll
SET DLLToTestRelativePath=Guardian.Tests\bin\x64\Debug_QA\FirefoxPrivateNetwork.Tests.dll

REM ** Filters Wiki https://github.com/opencover/opencover/wiki/Usage
REM ** SET Filters=+[Calculator]* -[Calculator]CalculatorTests.*
SET Filters=+[*]* -[nunit*]* -[FirefoxPrivateNetwork.Tests]*

SET OpenCoverFolderName=OpenCover.4.7.922
SET NUnitConsoleRunnerFolderName=NUnit.ConsoleRunner.3.10.0
SET ReportGeneratorFolderName=ReportGenerator.4.2.19

REM *****************************************************************

REM Create a 'GeneratedReports' folder if it does not exist
if not exist "%~dp0GeneratedReports" mkdir "%~dp0GeneratedReports"

REM Remove any previous test execution files to prevent issues overwriting
IF EXIST "%~dp0%TestResultsFileProjectName%.trx" del "%~dp0%TestResultsFileProjectName%.trx%"

REM Remove any previously created test output directories
CD %~dp0
FOR /D /R %%X IN (%USERNAME%*) DO RD /S /Q "%%X"

CD %~dp0Guardian.Tests\bin\x64\Debug_QA

REM Run the tests against the targeted output
call :RunOpenCoverUnitTestMetrics

CD %~dp0
REM Generate the report output based on the test results
if %errorlevel% equ 0 (
 call :RunReportGeneratorOutput
)

REM Launch the report
if %errorlevel% equ 0 (
 call :RunLaunchReport
)
exit /b %errorlevel%

:RunOpenCoverUnitTestMetrics
"%~dp0packages\%OpenCoverFolderName%\tools\OpenCover.Console.exe" ^
-register:administrator ^
-target:"%~dp0packages\%NUnitConsoleRunnerFolderName%\tools\nunit3-console.exe" ^
-targetargs:"--noheader \"%~dp0%DLLToTestRelativePath%\"" ^
-filter:"%Filters%" ^
-mergebyhash ^
-skipautoprops ^
-excludebyattribute:"System.CodeDom.Compiler.GeneratedCodeAttribute" ^
-output:"%~dp0GeneratedReports\%TestResultsFileProjectName%.xml"
exit /b %errorlevel%

:RunReportGeneratorOutput
"%~dp0packages\%ReportGeneratorFolderName%\tools\net47\ReportGenerator.exe" ^
-reports:"%~dp0GeneratedReports\%TestResultsFileProjectName%.xml" ^
-targetdir:"..\test\result\unittests\codecoverage"
exit /b %errorlevel%

:RunLaunchReport
start "report" "..\test\result\unittests\codecoverage\index.htm"
exit /b %errorlevel%