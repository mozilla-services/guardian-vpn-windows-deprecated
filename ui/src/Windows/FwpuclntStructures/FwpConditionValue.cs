using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FirefoxPrivateNetwork.Windows.Fwpuclnt;

namespace FirefoxPrivateNetwork.Windows.FwpuclntStructures
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]

    public struct FwpConditionValue
    {
        /// FWP_DATA_TYPE->FWP_DATA_TYPE_
        public FwpDataType type;

        public UnionType Union1;
    }
}
