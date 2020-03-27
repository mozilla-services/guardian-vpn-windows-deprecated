using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.Windows.FwpuclntStructures
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]

    public struct FwpByteBlob
    {
        /// UINT32->unsigned int
        public uint size;

        /// UINT8*
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)]
        public string data;
    }
}
