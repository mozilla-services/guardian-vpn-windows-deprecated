using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.Windows.FwpuclntStructures
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FwpmCallout
    {
        /// GUID->_GUID
        public Guid calloutKey;

        /// FWPM_DISPLAY_DATA0->FWPM_DISPLAY_DATA0_
        public FwpmDisplayData displayData;

        /// UINT32->int
        public int flags;

        /// GUID*
        public System.IntPtr providerKey;

        /// FWP_BYTE_BLOB->FWP_BYTE_BLOB_
        public FwpByteBlob providerData;

        /// GUID->_GUID
        public Guid applicableLayer;

        /// UINT32->int
        public int calloutId;
    }
}
