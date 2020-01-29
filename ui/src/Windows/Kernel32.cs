// <copyright file="Kernel32.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace FirefoxPrivateNetwork.Windows
{
    /// <summary>
    /// Moves an existing file or directory, including its children, with various move options.
    /// </summary>
    [Flags]
    public enum MoveFileFlags
    {
        /// <summary>
        /// No flags are set.
        /// </summary>
        None = 0,

        /// <summary>
        /// If a file named lpNewFileName exists, the function replaces its contents with the contents of the lpExistingFileName file,
        /// provided that security requirements regarding access control lists (ACLs) are met.
        /// </summary>
        ReplaceExisting = 1,

        /// <summary>
        /// If the file is to be moved to a different volume, the function simulates the move by using the CopyFile and DeleteFile functions.
        /// If the file is successfully copied to a different volume and the original file is unable to be deleted, the function
        /// succeeds leaving the source file intact.
        /// </summary>
        CopyAllowed = 2,

        /// <summary>
        /// The system does not move the file until the operating system is restarted. The system moves the file immediately after AUTOCHK is executed,
        /// but before creating any paging files. Consequently, this parameter enables the function to delete paging files from previous startups.
        /// This value can be used only if the process is in the context of a user who belongs to the administrators group or the LocalSystem account.
        /// </summary>
        DelayUntilReboot = 4,

        /// <summary>
        /// The function does not return until the file is actually moved on the disk. Setting this value guarantees that a move performed
        /// as a copy and delete operation is flushed to disk before the function returns. The flush occurs at the end of the copy operation.
        /// </summary>
        WriteThrough = 8,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        CreateHardlink = 16,

        /// <summary>
        /// The function fails if the source file is a link source, but the file cannot be tracked after the move.
        /// This situation can occur if the destination is a volume formatted with the FAT file system.
        /// </summary>
        FailIfNotTrackable = 32,
    }

    /// <summary>
    /// Kernel32.dll pinvoke library.
    /// </summary>
    public class Kernel32
    {
        /// <summary>
        /// Sets the minimum and maximum working set sizes for the specified process.
        /// </summary>
        /// <param name="hProcess">A handle to the process whose working set sizes is to be set.</param>
        /// <param name="dwMinimumWorkingSetSize">The minimum working set size for the process, in bytes.</param>
        /// <param name="dwMaximumWorkingSetSize">The maximum working set size for the process, in bytes.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetProcessWorkingSetSize(IntPtr hProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        /// <summary>
        /// Retrieves a module handle for the specified module.
        /// </summary>
        /// <param name="lpModuleName">The name of the loaded module (either a .dll or .exe file). If the file name extension is omitted, the default library extension .dll is appended.</param>
        /// <returns>If the function succeeds, the return value is a handle to the specified module. If the function fails, the return value is NULL.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// Moves an existing file or directory, including its children, with various move options.
        /// </summary>
        /// <param name="lpExistingFileName">The current name of the file or directory on the local computer.</param>
        /// <param name="lpNewFileName">The new name of the file or directory on the local computer. If dwFlags
        /// specifies MOVEFILE_DELAY_UNTIL_REBOOT and lpNewFileName is NULL, MoveFileEx registers the lpExistingFileName
        /// file to be deleted when the system restarts.</param>
        /// <param name="dwFlags">MoveFileFlags parameters.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);
    }
}
