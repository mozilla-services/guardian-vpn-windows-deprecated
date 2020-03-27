using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.Windows.FwpuclntStructures
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FwpmFilter
    {
        /// GUID->_GUID
        //public GUID filterKey;
        public Guid filterKey;

        /// FWPM_DISPLAY_DATA0->FWPM_DISPLAY_DATA0_
        public FwpmDisplayData displayData;

        /// UINT32->unsigned int
        public uint flags;

        /// GUID*
        public System.IntPtr providerKey;

        /// FWP_BYTE_BLOB->FWP_BYTE_BLOB_
        public FwpByteBlob providerData;

        /// GUID->_GUID
        //public GUID layerKey;
        public Guid layerKey;

        /// GUID->_GUID
        //public GUID subLayerKey;
        public Guid subLayerKey;

        /// FWP_VALUE0->FWP_VALUE0_
        public FwpValue weight;

        /// UINT32->unsigned int
        public uint numFilterConditions;

        /// FWPM_FILTER_CONDITION0*
        public System.IntPtr filterCondition;

        /// FWPM_ACTION0->FWPM_ACTION0_
        public FwpmAction action;

        /// Anonymous_9e7af134_1c75_45df_ba40_12e91eb0542c
        /// UINT64->unsigned __int64
        public ulong rawContext;

        /// GUID->_GUID
        public Guid providerContextKey;

        // CARLOS: rawContext & providerContextKey may need to be in a Union bro!!!


        /// GUID*
        public System.IntPtr reserved;

        /// UINT64->unsigned __int64
        public long filterId;
        //public IntPtr filterId;

        /// FWP_VALUE0->FWP_VALUE0_
        public FwpValue effectiveWeight;
    }
}
