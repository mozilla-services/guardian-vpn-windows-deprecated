using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.Windows.FwpuclntStructures
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]

    public struct FwpmAction
    {
        /// FWP_ACTION_TYPE
        public uint type;

        public UnionType2 Union1;

        /// Anonymous_2997c82f_b552_43d7_a71b_526008c4825c
        //public Guid filterType;

        /// GUID->_GUID
        //public Guid calloutKey;
    }
}
