// <copyright file="DebugDump.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace FirefoxPrivateNetwork.ErrorHandling
{
    /// <summary>
    /// Debug helper class. Used to package up system diagnostic info and log files.
    /// </summary>
    public class DebugDump
    {
        /// <summary>
        /// Creates a debug dump ZIP file with log files and system diagnostic info.
        /// </summary>
        /// <param name="outputZip">Output ZIP file location.</param>
        public static void CreateDump(string outputZip)
        {
            var tempFolderName = string.Concat(ProductConstants.ProductName, "Debug", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            var tempPath = Path.Combine(Path.GetTempPath(), tempFolderName);

            // Create directory first
            if (Directory.Exists(tempPath))
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(tempPath);

                // Driver checkup
                RunAndSaveOutput("pnputil.exe", Path.Combine(tempPath, "drivers.txt"), "/enum-drivers");

                // Running processes
                RunAndSaveOutputCsv("wmic", Path.Combine(tempPath, "processes.csv"), "path win32_process get Caption,Processid,Commandline,CreationDate,ExecutablePath,WorkingSetSize /format:csv");

                // Network
                RunAndSaveOutput("ipconfig.exe", Path.Combine(tempPath, "network.txt"), "/all");

                // System info
                RunAndSaveOutput("systeminfo.exe", Path.Combine(tempPath, "system.txt"));

                // AppData
                if (File.Exists(Path.Combine(ProductConstants.UserAppDataFolder, "servers.json")))
                {
                    File.Copy(Path.Combine(ProductConstants.UserAppDataFolder, "servers.json"), Path.Combine(tempPath, "servers.json"), true);
                }

                // GUI log and WireGuard log, combined
                var logTxt = File.CreateText(Path.Combine(tempPath, "log.txt"));
                ErrorHandler.Ringlogger.WriteTo(logTxt);
                logTxt.Close();

                // AppData status
                var appDataFiles = Directory.GetFiles(ProductConstants.UserAppDataFolder);
                System.IO.File.WriteAllText(Path.Combine(tempPath, "appdatacontents.txt"), string.Join("\n", appDataFiles));

                File.Delete(Path.Combine(tempPath, outputZip));
                ZipFile.CreateFromDirectory(tempPath, outputZip);

                // Cleanup
                Directory.Delete(tempPath, true);
            }
            catch (Exception)
            {
                // Cleanup
                Directory.Delete(tempPath, true);
                ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-debug-export-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowError, ErrorHandling.LogLevel.Error);
            }
        }

        /// <summary>
        /// Runs a process and grabs the stdout output.
        /// </summary>
        /// <param name="cmdLine">Command line to run.</param>
        /// <param name="arguments">Arguments to run the command line with.</param>
        /// <returns>Stdout output from the launched application.</returns>
        private static string RunProcess(string cmdLine, string arguments = "")
        {
            var proc = new Process();
            proc.StartInfo.FileName = cmdLine;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            if (arguments != string.Empty)
            {
                proc.StartInfo.Arguments = arguments;
            }

            proc.Start();

            var reader = proc.StandardOutput;
            string taskOutput = reader.ReadToEnd();
            proc.WaitForExit();
            proc.Dispose();

            return taskOutput;
        }

        /// <summary>
        /// Wrapper function which runs a command and redirects the standard output to a file.
        /// </summary>
        /// <param name="cmdLine">Command line to run.</param>
        /// <param name="outputFile">Output file where the stdout output will be saved.</param>
        /// <param name="arguments">Arguments to run the command line with.</param>
        private static void RunAndSaveOutput(string cmdLine, string outputFile, string arguments = "")
        {
            System.IO.File.WriteAllText(outputFile, RunProcess(cmdLine, arguments));
        }

        /// <summary>
        /// Wrapper function which runs a command and redirects the output to a CSV file with a CSV delimiter.
        /// </summary>
        /// <param name="cmdLine">Command line to run.</param>
        /// <param name="outputFile">Output file where the stdout CSV output will be saved.</param>
        /// <param name="arguments">Arguments to run the command line with.</param>
        private static void RunAndSaveOutputCsv(string cmdLine, string outputFile, string arguments = "")
        {
            var csvData = RunProcess(cmdLine, arguments);
            csvData = string.Concat("sep=,", csvData);

            System.IO.File.WriteAllText(outputFile, csvData.Replace("\r\r", "\r"));
        }
    }
}
