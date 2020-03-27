using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.Windows.FwpuclntStructures
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public struct UnionType
    {
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public byte uint8;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public ushort uint16;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public uint uint32;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr uint64;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public byte int8;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public short int16;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public int int32;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr int64;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public float float32;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr double64;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr byteArray16;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr byteBlob;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr sid;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr sd;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr tokenInformation;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr tokenAccessInformation;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr unicodeString;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr byteArray6;
    }
}
