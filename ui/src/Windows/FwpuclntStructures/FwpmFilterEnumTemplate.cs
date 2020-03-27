using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FirefoxPrivateNetwork.Windows.Fwpuclnt;

namespace FirefoxPrivateNetwork.Windows.FwpuclntStructures
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]

    public struct FwpmFilterEnumTemplate
    {
        /// GUID*
        public System.IntPtr providerKey;

        /// GUID->_GUID
        public Guid layerKey;

        /// FWP_FILTER_ENUM_TYPE->FWP_FILTER_ENUM_TYPE_
        public FwpFilterEnumType enumType;

        /// UINT32->int
        public int flags;

        /// FWPM_PROVIDER_CONTEXT_ENUM_TEMPLATE0*
        public System.IntPtr providerContextTemplate;

        /// UINT32->int
        public int numFilterConditions;

        /// FWPM_FILTER_CONDITION0*
        public System.IntPtr filterCondition;

        /// UINT32->int
        public int actionMask;

        /// GUID*
        public System.IntPtr calloutKey;
    }
}
