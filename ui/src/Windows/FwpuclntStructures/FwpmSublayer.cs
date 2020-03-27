using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.Windows.FwpuclntStructures
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FwpmSublayer
    {
        /// GUID->_GUID
        //public GUID subLayerKey;
        public Guid subLayerKey;

        /// FWPM_DISPLAY_DATA0->FWPM_DISPLAY_DATA0_
        public FwpmDisplayData displayData;

        /// UINT16->unsigned short
        public ushort flags;

        /// GUID*
        public System.IntPtr providerKey;

        /// FWP_BYTE_BLOB->FWP_BYTE_BLOB_
        public FwpByteBlob providerData;

        /// UINT16->unsigned short
        public ushort weight;
    }
}
