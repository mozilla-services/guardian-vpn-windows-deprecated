// <copyright file="Fwpuclnt.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.Windows
{
    /// <summary>
    /// Access to Windows Filtering Platform to allow/block network traffic.
    /// </summary>
    internal class Fwpuclnt
    {
        #region Layer Keys
        public static Guid FWPM_LAYER_ALE_AUTH_CONNECT_V4 = new Guid("{c38d57d1-05a7-4c33-904f-7fbceee60e82}");
        public static Guid FWPM_LAYER_ALE_AUTH_CONNECT_V6 = new Guid("{4a72393b-319f-44bc-84c3-ba54dcb3b6b4}");
        #endregion

        /// FWP_ACTION_FLAG_TERMINATING -> (0x00001000)
        public const int FWP_ACTION_FLAG_TERMINATING = 4096;
        /// FWP_ACTION_FLAG_NON_TERMINATING -> (0x00002000)
        public const int FWP_ACTION_FLAG_NON_TERMINATING = 8192;
        /// FWP_ACTION_FLAG_CALLOUT -> (0x00004000)
        public const int FWP_ACTION_FLAG_CALLOUT = 16384;
        /// FWP_ACTION_BLOCK -> (0x00000001 | FWP_ACTION_FLAG_TERMINATING)
        public const int FWP_ACTION_BLOCK = 1 | FWP_ACTION_FLAG_TERMINATING;
        /// FWP_ACTION_PERMIT -> (0x00000002 | FWP_ACTION_FLAG_TERMINATING)
        public const int FWP_ACTION_PERMIT = 2 | FWP_ACTION_FLAG_TERMINATING;
        /// FWP_ACTION_CALLOUT_TERMINATING -> (0x00000003 | FWP_ACTION_FLAG_CALLOUT | FWP_ACTION_FLAG_TERMINATING)
        public const int FWP_ACTION_CALLOUT_TERMINATING = 3 | (FWP_ACTION_FLAG_CALLOUT | FWP_ACTION_FLAG_TERMINATING);
        /// FWP_ACTION_CALLOUT_INSPECTION -> (0x00000004 | FWP_ACTION_FLAG_CALLOUT | FWP_ACTION_FLAG_NON_TERMINATING)
        public const int FWP_ACTION_CALLOUT_INSPECTION = 4 | (FWP_ACTION_FLAG_CALLOUT | FWP_ACTION_FLAG_NON_TERMINATING);
        /// FWP_ACTION_CALLOUT_UNKNOWN -> (0x00000005 | FWP_ACTION_FLAG_CALLOUT)
        public const int FWP_ACTION_CALLOUT_UNKNOWN = 5 | FWP_ACTION_FLAG_CALLOUT;
        /// FWP_ACTION_CONTINUE -> (0x00000006 | FWP_ACTION_FLAG_NON_TERMINATING)
        public const int FWP_ACTION_CONTINUE = 6 | FWP_ACTION_FLAG_NON_TERMINATING;
        /// FWP_ACTION_NONE -> (0x00000007)
        public const int FWP_ACTION_NONE = 7;
        /// FWP_ACTION_NONE_NO_MATCH -> (0x00000008)
        public const int FWP_ACTION_NONE_NO_MATCH = 8;

        public Guid sublayerKey = Guid.NewGuid();

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FWPM_SESSION0_
        {
            /// GUID->_GUID
            public Guid sessionKey;

            /// FWPM_DISPLAY_DATA0->FWPM_DISPLAY_DATA0_
            public FWPM_DISPLAY_DATA0_ displayData;

            /// UINT32->int
            public int flags;

            /// UINT32->int
            public int txnWaitTimeoutInMSec;

            /// DWORD->int
            public int processId;

            /// SID*
            public System.IntPtr sid;

            /// wchar_t*
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string username;

            /// BOOL->int
            public int kernelMode;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FWPM_DISPLAY_DATA0_
        {
            /// wchar_t*
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string name;

            /// wchar_t*
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string description;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct _SEC_WINNT_AUTH_IDENTITY_W
        {
            /// unsigned short*
            public System.IntPtr User;

            /// unsigned int
            public uint UserLength;

            /// unsigned short*
            public System.IntPtr Domain;

            /// unsigned int
            public uint DomainLength;

            /// unsigned short*
            public System.IntPtr Password;

            /// unsigned int
            public uint PasswordLength;

            /// unsigned int
            public uint Flags;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FWPM_SUBLAYER0_
        {
            /// GUID->_GUID
            public Guid subLayerKey;

            /// FWPM_DISPLAY_DATA0->FWPM_DISPLAY_DATA0_
            public FWPM_DISPLAY_DATA0_ displayData;

            /// UINT32->int
            public int flags;

            /// GUID*
            public System.IntPtr providerKey;

            /// FWP_BYTE_BLOB->FWP_BYTE_BLOB_
            public FWP_BYTE_BLOB_ providerData;

            /// UINT16->short
            public short weight;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FWP_BYTE_BLOB_
        {
            /// UINT32->int
            public int size;

            /// UINT8*
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)]
            public string data;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
        public struct FWPM_FILTER_UNION
        {
            /// UINT64->__int64
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public long rawContext;

            /// GUID->_GUID
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public Guid providerContextKey;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FWPM_FILTER0_
        {
            /// GUID->_GUID
            public Guid filterKey;

            /// FWPM_DISPLAY_DATA0->FWPM_DISPLAY_DATA0_
            public FWPM_DISPLAY_DATA0_ displayData;

            /// UINT32->int
            public int flags;

            /// GUID*
            public System.IntPtr providerKey;

            /// FWP_BYTE_BLOB->FWP_BYTE_BLOB_
            public FWP_BYTE_BLOB_ providerData;

            /// GUID->_GUID
            public Guid layerKey;

            /// GUID->_GUID
            public Guid subLayerKey;

            /// FWP_VALUE0->FWP_VALUE0_
            public FWP_VALUE0_ weight;

            /// UINT32->int
            public int numFilterConditions;

            /// FWPM_FILTER_CONDITION0*
            public System.IntPtr filterCondition;

            /// FWPM_ACTION0->FWPM_ACTION0_
            public FWPM_ACTION0_ action;

            /// Anonymous_e8d4a944_8ad7_4ea8_9dc1_d4a3dbf8989f
            public FWPM_FILTER_UNION Union1;

            /// GUID*
            public System.IntPtr reserved;

            /// UINT64->__int64
            public long filterId;

            /// FWP_VALUE0->FWP_VALUE0_
            public FWP_VALUE0_ effectiveWeight;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
        public struct FWP_VALUE_UNION
        {
            /// UINT8->char
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public byte uint8;

            /// UINT16->short
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public short uint16;

            /// UINT32->int
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public int uint32;

            /// UINT64*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr uint64;

            /// INT8->char
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public byte int8;

            /// INT16->short
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public short int16;

            /// INT32->int
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public int int32;

            /// INT64*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr int64;

            /// float
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public float float32;

            /// double*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr double64;

            /// FWP_BYTE_ARRAY16*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr byteArray16;

            /// FWP_BYTE_BLOB*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr byteBlob;

            /// SID*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr sid;

            /// FWP_BYTE_BLOB*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr sd;

            /// FWP_TOKEN_INFORMATION*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr tokenInformation;

            /// FWP_BYTE_BLOB*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr tokenAccessInformation;

            /// LPWSTR->WCHAR*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr unicodeString;

            /// FWP_BYTE_ARRAY6*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr byteArray6;

            /// FWP_BITMAP_ARRAY64*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr bitmapArray64;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FWP_VALUE0_
        {
            /// FWP_DATA_TYPE->FWP_DATA_TYPE_
            public FWP_DATA_TYPE_ type;

            /// FWP_VALUE_UNION
            public FWP_VALUE_UNION Union1;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
        public struct FWPM_ACTION_UNION
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

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FWPM_ACTION0_
        {
            /// FWP_ACTION_TYPE->UINT32->int
            public int type;

            /// Anonymous_5978e21f_64b6_4288_ae7d_0222f642549d
            public FWPM_ACTION_UNION Union1;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FWPM_FILTER_CONDITION0_
        {
            /// GUID->_GUID
            public Guid fieldKey;

            /// FWP_MATCH_TYPE->FWP_MATCH_TYPE_
            public FWP_MATCH_TYPE_ matchType;

            /// FWP_CONDITION_VALUE0->FWP_CONDITION_VALUE0_
            public FWP_CONDITION_VALUE0_ conditionValue;
        }

        public enum FWP_MATCH_TYPE_
        {
            FWP_MATCH_EQUAL,
            FWP_MATCH_GREATER,
            FWP_MATCH_LESS,
            FWP_MATCH_GREATER_OR_EQUAL,
            FWP_MATCH_LESS_OR_EQUAL,
            FWP_MATCH_RANGE,
            FWP_MATCH_FLAGS_ALL_SET,
            FWP_MATCH_FLAGS_ANY_SET,
            FWP_MATCH_FLAGS_NONE_SET,
            FWP_MATCH_EQUAL_CASE_INSENSITIVE,
            FWP_MATCH_NOT_EQUAL,
            FWP_MATCH_PREFIX,
            FWP_MATCH_NOT_PREFIX,
            FWP_MATCH_TYPE_MAX,
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
        public struct FWP_CONDITION_VALUE_UNION
        {
            /// UINT8->char
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public byte uint8;

            /// UINT16->short
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public short uint16;

            /// UINT32->int
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public int uint32;

            /// UINT64*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr uint64;

            /// INT8->char
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public byte int8;

            /// INT16->short
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public short int16;

            /// INT32->int
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public int int32;

            /// INT64*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr int64;

            /// float
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public float float32;

            /// double*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr double64;

            /// FWP_BYTE_ARRAY16*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr byteArray16;

            /// FWP_BYTE_BLOB*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr byteBlob;

            /// SID*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr sid;

            /// FWP_BYTE_BLOB*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr sd;

            /// FWP_TOKEN_INFORMATION*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr tokenInformation;

            /// FWP_BYTE_BLOB*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr tokenAccessInformation;

            /// LPWSTR->WCHAR*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr unicodeString;

            /// FWP_BYTE_ARRAY6*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr byteArray6;

            /// FWP_BITMAP_ARRAY64*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr bitmapArray64;

            /// FWP_V4_ADDR_AND_MASK*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr v4AddrMask;

            /// FWP_V6_ADDR_AND_MASK*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr v6AddrMask;

            /// FWP_RANGE0*
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public System.IntPtr rangeValue;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FWP_CONDITION_VALUE0_
        {

            /// FWP_DATA_TYPE->FWP_DATA_TYPE_
            public FWP_DATA_TYPE_ type;

            /// Anonymous_7453761e_fb92_4d1e_b03a_ade3b67fb9cf
            public FWP_CONDITION_VALUE_UNION Union1;
        }

        public enum FWP_DATA_TYPE_
        {
            FWP_EMPTY,
            FWP_UINT8,
            FWP_UINT16,
            FWP_UINT32,
            FWP_UINT64,
            FWP_INT8,
            FWP_INT16,
            FWP_INT32,
            FWP_INT64,
            FWP_FLOAT,
            FWP_DOUBLE,
            FWP_BYTE_ARRAY16_TYPE,
            FWP_BYTE_BLOB_TYPE,
            FWP_SID,
            FWP_SECURITY_DESCRIPTOR_TYPE,
            FWP_TOKEN_INFORMATION_TYPE,
            FWP_TOKEN_ACCESS_INFORMATION_TYPE,
            FWP_UNICODE_STRING_TYPE,
            FWP_BYTE_ARRAY6_TYPE,
            FWP_BITMAP_INDEX_TYPE,
            FWP_BITMAP_ARRAY64_TYPE,
            FWP_SINGLE_DATA_TYPE_MAX,
            FWP_V4_ADDR_MASK,
            FWP_V6_ADDR_MASK,
            FWP_RANGE_TYPE,
            FWP_DATA_TYPE_MAX,
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct FWP_BYTE_ARRAY16_
        {

            /// UINT8[16]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 16)]
            public string byteArray16;
        }

        /// <summary>
        /// Stores an array ofexactly 6 bytes.
        /// </summary>
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct FWP_BYTE_ARRAY6_
        {
            /// UINT8[6]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 6)]
            public string byteArray6;
        }

        /// <summary>
        /// FWP Bitmap array.
        /// </summary>
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct FWP_BITMAP_ARRAY64_
        {
            /// UINT8[8]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 8)]
            public string bitmapArray64;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FWP_TOKEN_INFORMATION_
        {
            /// ULONG->int
            public int sidCount;

            /// PSID_AND_ATTRIBUTES->_SID_AND_ATTRIBUTES*
            public System.IntPtr sids;

            /// ULONG->int
            public int restrictedSidCount;

            /// PSID_AND_ATTRIBUTES->_SID_AND_ATTRIBUTES*
            public System.IntPtr restrictedSids;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FWP_RANGE0_
        {
            /// FWP_VALUE0->FWP_VALUE0_
            public FWP_VALUE0_ valueLow;

            /// FWP_VALUE0->FWP_VALUE0_
            public FWP_VALUE0_ valueHigh;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FWP_V4_ADDR_AND_MASK_
        {
            /// UINT32->int
            public int addr;

            /// UINT32->int
            public int mask;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct FWP_V6_ADDR_AND_MASK_
        {
            /// UINT8[16]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 16)]
            public string addr;

            /// UINT8->char
            public byte prefixLength;
        }

        /// <summary>
        /// Operations to control network access.
        /// </summary>
        public partial class NativeMethods
        {
            /// <summary>
            /// Opens filter engine.
            /// </summary>
            /// <param name="serverName">This value must be NULL..</param>
            /// <param name="authnService">Specifies the authentication service to use.</param>
            /// <param name="authIdentity">The authentication and authorization credentials for accessing the filter engine.</param>
            /// <param name="session">Session-specific parameters for the session being opened.</param>
            /// <param name="engineHandle">Handle for the open session to the filter engine.</param>
            /// <returns>DWORD.</returns>
            [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmEngineOpen0")]
            [HandleProcessCorruptedStateExceptions]
            [SecurityCritical]
            public static extern int FwpmEngineOpen0([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string serverName, uint authnService, [System.Runtime.InteropServices.InAttribute()] System.IntPtr authIdentity, [System.Runtime.InteropServices.InAttribute()] System.IntPtr session, ref System.IntPtr engineHandle);

            /// <summary>
            /// Closes a session to a filter engine.
            /// </summary>
            /// <param name="engineHandle">Handle for an open session to the filter engine.</param>
            /// <returns>DWORD.</returns>
            [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmEngineClose0")]
            [HandleProcessCorruptedStateExceptions]
            [SecurityCritical]
            public static extern int FwpmEngineClose0(System.IntPtr engineHandle);

            /// <summary>
            /// Adds a new sublayer to the system.
            /// </summary>
            /// <param name="engineHandle">Handle for an open session to the filter engine.</param>
            /// <param name="subLayer">The sublayer to be added.</param>
            /// <param name="sd">Security information for the sublayer object.</param>
            /// <returns>DWORD.</returns>
            [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmSubLayerAdd0")]
            [HandleProcessCorruptedStateExceptions]
            [SecurityCritical]
            public static extern int FwpmSubLayerAdd0(System.IntPtr engineHandle, ref FWPM_SUBLAYER0_ subLayer, System.IntPtr sd);

            /// <summary>
            /// Adds a new filter object to the system.
            /// </summary>
            /// <param name="engineHandle">Handle for an open session to the filter engine.</param>
            /// <param name="filter">The filter object to be added.</param>
            /// <param name="sd">Security information about the filter object.</param>
            /// <param name="id">The runtime identifier for this filter.</param>
            /// <returns>DWORD.</returns>
            [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmFilterAdd0")]
            public static extern int FwpmFilterAdd0(System.IntPtr engineHandle, ref FWPM_FILTER0_ filter, System.IntPtr sd, ref long id);

            /// <summary>
            /// Deletes a sublayer from the system by its key.
            /// </summary>
            /// <param name="engineHandle">Handle for an open session to the filter engine.</param>
            /// <param name="key">Unique identifier of the sublayer to be removed from the system.</param>
            /// <returns>DWORD.</returns>
            [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmSubLayerDeleteByKey0")]
            public static extern int FwpmSubLayerDeleteByKey0(System.IntPtr engineHandle, ref Guid key);

            /// <summary>
            /// Removes a filter object from the system.
            /// </summary>
            /// <param name="engineHandle">Handle for an open session to the filter engine.</param>
            /// <param name="key">Unique identifier of the object being removed from the system. </param>
            /// <returns>DWORD.</returns>
            [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmFilterDeleteByKey0")]
            public static extern int FwpmFilterDeleteByKey0(System.IntPtr engineHandle, ref Guid key);
        }
    }
}
