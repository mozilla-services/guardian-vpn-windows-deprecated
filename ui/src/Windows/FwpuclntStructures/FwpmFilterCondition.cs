using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FirefoxPrivateNetwork.Windows.Fwpuclnt;

namespace FirefoxPrivateNetwork.Windows.FwpuclntStructures
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FwpmFilterCondition
    {
        /// GUID->_GUID
        //public GUID fieldKey;
        public Guid fieldKey;

        /// FWP_MATCH_TYPE->FWP_MATCH_TYPE_
        public FwpMatchType matchType;

        /// FWP_CONDITION_VALUE0_
        public FwpConditionValue conditionValue;
    }
}
