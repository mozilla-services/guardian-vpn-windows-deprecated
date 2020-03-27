using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.Windows.FwpuclntStructures
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]

    public struct FwpmSession
    {
        /// GUID->_GUID
        public Guid sessionKey;

        /// FWPM_DISPLAY_DATA0->FWPM_DISPLAY_DATA0_
        public FwpmDisplayData displayData;

        /// UINT32->unsigned int
        public uint flags;

        /// UINT32->unsigned int
        public uint txnWaitTimeoutInMSec;

        /// DWORD->unsigned int
        public uint processId;

        /// SID*
        public System.IntPtr sid;

        /// wchar_t*
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        public string username;

        /// BOOL->int
        public int kernelMode;
    }
}
