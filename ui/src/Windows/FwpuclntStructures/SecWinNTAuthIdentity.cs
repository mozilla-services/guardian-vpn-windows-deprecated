using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.Windows.FwpuclntStructures
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct SecWinNTAuthIdentity
    {
        /// unsigned short*
        public System.IntPtr User;

        /// unsigned int
        public uint UserLength;

        /// unsigned short*
        public System.IntPtr Domain;

        /// unsigned int
        public uint DomainLength;

        /// unsigned short*
        public System.IntPtr Password;

        /// unsigned int
        public uint PasswordLength;

        /// unsigned int
        public uint Flags;
    }
}
