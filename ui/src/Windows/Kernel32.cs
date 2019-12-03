// <copyright file="Kernel32.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace FirefoxPrivateNetwork.Windows
{
    /// <summary>
    /// Options for handle duplication.
    /// </summary>
    [Flags]
    public enum DuplicateOptions : uint
    {
        /// <summary>
        /// Closes the source handle. This occurs regardless of any error status returned.
        /// </summary>
        DUPLICATE_CLOSE_SOURCE = 0x00000001,

        /// <summary>
        /// Ignores the dwDesiredAccess parameter. The duplicate handle has the same access as the source handle.
        /// </summary>
        DUPLICATE_SAME_ACCESS = 0x00000002,
    }

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
        /// Duplicates an object handle.
        /// </summary>
        /// <param name="hSourceProcessHandle">A handle to the process with the handle to be duplicated.</param>
        /// <param name="hSourceHandle">The handle to be duplicated.</param>
        /// <param name="hTargetProcessHandle">A handle to the process that is to receive the duplicated handle.</param>
        /// <param name="lpTargetHandle">A pointer to a variable that receives the duplicate handle.</param>
        /// <param name="dwDesiredAccess">The access requested for the new handle.</param>
        /// <param name="bInheritHandle">A variable that indicates whether the handle is inheritable.</param>
        /// <param name="dwOptions">Optional duplication options.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr lpTargetHandle, uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwOptions);

        /// <summary>
        /// Creates an anonymous pipe, and returns handles to the read and write ends of the pipe.
        /// </summary>
        /// <param name="hReadPipe">A pointer to a variable that receives the read handle for the pipe.</param>
        /// <param name="hWritePipe">A pointer to a variable that receives the write handle for the pipe.</param>
        /// <param name="lpPipeAttributes">A pointer to a SecurityAttributes structure that determines whether the returned handle can be inherited by child processes.</param>
        /// <param name="nSize">The size of the buffer for the pipe, in bytes. If this parameter is zero, the system uses the default buffer size.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("kernel32.dll")]
        public static extern bool CreatePipe(out IntPtr hReadPipe, out IntPtr hWritePipe, SecurityAttributes lpPipeAttributes, uint nSize);

        /// <summary>
        /// Reads data from the specified file or input/output (I/O) device. Reads occur at the position specified by the file pointer if supported by the device.
        /// </summary>
        /// <param name="hFile">A handle to the device.</param>
        /// <param name="lpBuffer">A pointer to the buffer that receives the data read from a file or device.</param>
        /// <param name="nNumberOfBytesToRead">The maximum number of bytes to be read.</param>
        /// <param name="lpNumberOfBytesRead">A pointer to the variable that receives the number of bytes read when using a synchronous hFile parameter.</param>
        /// <param name="lpOverlapped">A pointer to an OVERLAPPED structure is required if the hFile parameter was opened with FILE_FLAG_OVERLAPPED, otherwise it can be NULL.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        /// <summary>
        /// Writes data to the specified file or input/output (I/O) device.
        /// </summary>
        /// <param name="hFile">A handle to the file or I/O device.</param>
        /// <param name="lpBuffer">A pointer to the buffer containing the data to be written to the file or device.</param>
        /// <param name="nNumberOfBytesToWrite">The number of bytes to be written to the file or device.</param>
        /// <param name="lpNumberOfBytesWritten">A pointer to the variable that receives the number of bytes written when using a synchronous hFile parameter.</param>
        /// <param name="lpOverlapped">A pointer to an OVERLAPPED structure is required if the hFile parameter was opened with FILE_FLAG_OVERLAPPED, otherwise this parameter can be NULL.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, [In] IntPtr lpOverlapped);

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="hObject">A valid handle to an open object.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

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
