using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.Windows.FwpuclntStructures
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]

    public struct UnionType2
    {
        /// GUID->_GUID
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public Guid filterType;

        /// GUID->_GUID
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public Guid calloutKey;

        /// UINT8->char
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public byte bitmapIndex;
    }
}
