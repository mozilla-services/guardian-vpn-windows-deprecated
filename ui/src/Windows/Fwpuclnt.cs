using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Runtime.ExceptionServices;
using FirefoxPrivateNetwork.Windows.FwpuclntStructures;

namespace FirefoxPrivateNetwork.Windows
{
    public class Fwpuclnt
    {
        #region Native Constants

        /// FWPM_CALLOUT_FLAG_PERSISTENT -> (0x00010000)
        public const int FWPM_CALLOUT_FLAG_PERSISTENT = 65536;
        /// FWPM_CALLOUT_FLAG_USES_PROVIDER_CONTEXT -> (0x00020000)
        public const int FWPM_CALLOUT_FLAG_USES_PROVIDER_CONTEXT = 131072;
        /// FWPM_CALLOUT_FLAG_REGISTERED -> (0x00040000)
        public const int FWPM_CALLOUT_FLAG_REGISTERED = 262144;
        /// FWP_ACTION_FLAG_TERMINATING -> (0x00001000)
        public const int FWP_ACTION_FLAG_TERMINATING = 4096;
        /// FWP_ACTION_FLAG_NON_TERMINATING -> (0x00002000)
        public const int FWP_ACTION_FLAG_NON_TERMINATING = 8192;
        /// FWP_ACTION_FLAG_CALLOUT -> (0x00004000)
        public const int FWP_ACTION_FLAG_CALLOUT = 16384;
        /// FWP_ACTION_BLOCK -> (0x00000001 | FWP_ACTION_FLAG_TERMINATING)
        public const int FWP_ACTION_BLOCK = (1 | FWP_ACTION_FLAG_TERMINATING);
        /// FWP_ACTION_PERMIT -> (0x00000002 | FWP_ACTION_FLAG_TERMINATING)
        public const int FWP_ACTION_PERMIT = (2 | FWP_ACTION_FLAG_TERMINATING);
        /// FWP_ACTION_CALLOUT_TERMINATING -> (0x00000003 | FWP_ACTION_FLAG_CALLOUT | FWP_ACTION_FLAG_TERMINATING)
        public const int FWP_ACTION_CALLOUT_TERMINATING = (3 | (FWP_ACTION_FLAG_CALLOUT | FWP_ACTION_FLAG_TERMINATING));
        /// FWP_ACTION_CALLOUT_INSPECTION -> (0x00000004 | FWP_ACTION_FLAG_CALLOUT | FWP_ACTION_FLAG_NON_TERMINATING)
        public const int FWP_ACTION_CALLOUT_INSPECTION = (4 | (FWP_ACTION_FLAG_CALLOUT | FWP_ACTION_FLAG_NON_TERMINATING));
        /// FWP_ACTION_CALLOUT_UNKNOWN -> (0x00000005 | FWP_ACTION_FLAG_CALLOUT)
        public const int FWP_ACTION_CALLOUT_UNKNOWN = (5 | FWP_ACTION_FLAG_CALLOUT);
        /// FWP_ACTION_CONTINUE -> (0x00000006 | FWP_ACTION_FLAG_NON_TERMINATING)
        public const int FWP_ACTION_CONTINUE = (6 | FWP_ACTION_FLAG_NON_TERMINATING);
        /// FWP_ACTION_NONE -> (0x00000007)
        public const int FWP_ACTION_NONE = 7;
        /// FWP_ACTION_NONE_NO_MATCH -> (0x00000008)
        public const int FWP_ACTION_NONE_NO_MATCH = 8;
        /// FWPM_FILTER_FLAG_NONE -> (0x00000000)
        public const int FWPM_FILTER_FLAG_NONE = 0;
        /// FWPM_FILTER_FLAG_PERSISTENT -> (0x00000001)
        public const int FWPM_FILTER_FLAG_PERSISTENT = 1;
        /// FWPM_FILTER_FLAG_BOOTTIME -> (0x00000002)
        public const int FWPM_FILTER_FLAG_BOOTTIME = 2;
        /// FWPM_FILTER_FLAG_HAS_PROVIDER_CONTEXT -> (0x00000004)
        public const int FWPM_FILTER_FLAG_HAS_PROVIDER_CONTEXT = 4;
        /// FWPM_FILTER_FLAG_CLEAR_ACTION_RIGHT -> (0x00000008)
        public const int FWPM_FILTER_FLAG_CLEAR_ACTION_RIGHT = 8;
        /// FWPM_FILTER_FLAG_PERMIT_IF_CALLOUT_UNREGISTERED -> (0x00000010)
        public const int FWPM_FILTER_FLAG_PERMIT_IF_CALLOUT_UNREGISTERED = 16;
        /// FWPM_FILTER_FLAG_DISABLED -> (0x00000020)
        public const int FWPM_FILTER_FLAG_DISABLED = 32;
        /// FWPM_FILTER_FLAG_INDEXED -> (0x00000040)
        public const int FWPM_FILTER_FLAG_INDEXED = 64;
        /// FWPM_SESSION_FLAG_DYNAMIC -> (0x00000001)
        public const int FWPM_SESSION_FLAG_DYNAMIC = 1;

        #endregion

        #region Authentication Service Constants

        const uint RPC_C_AUTHN_NONE = 0;
        const uint RPC_C_AUTHN_DCE_PRIVATE = 1;
        const uint RPC_C_AUTHN_DCE_PUBLIC = 2;
        const uint RPC_C_AUTHN_DEC_PUBLIC = 4;
        const uint RPC_C_AUTHN_GSS_NEGOTIATE = 9;
        public const uint RPC_C_AUTHN_WINNT = 10;
        const uint RPC_C_AUTHN_GSS_SCHANNEL = 14;
        const uint RPC_C_AUTHN_GSS_KERBEROS = 16;
        const uint RPC_C_AUTHN_DPA = 17;
        const uint RPC_C_AUTHN_MSN = 18;
        const uint RPC_C_AUTHN_DIGEST = 21;
        const uint RPC_C_AUTHN_NEGO_EXTENDER = 30;
        const uint RPC_C_AUTHN_MQ = 100;
        const uint RPC_C_AUTHN_DEFAULT = 0xffffffff;

        #endregion

        #region Filter Keys

        private static Guid FWPM_CONDITION_FLAGS = new Guid("{632ce23b-5167-435c-86d7-e903684aa80c}");
        private static Guid FWPM_CONDITION_INTERFACE_INDEX = new Guid("{667fd755-d695-434a-8af5-d3835a1259bc}");
        private static Guid FWPM_CONDITION_INTERFACE_TYPE = new Guid("{daf8cd14-e09e-4c93-a5ae-c5c13b73ffca}");
        private static Guid FWPM_CONDITION_IP_LOCAL_ADDRESS = new Guid("{d9ee00de-c1ef-4617-bfe3-ffd8f5a08957}");
        private static Guid FWPM_CONDITION_IP_LOCAL_ADDRESS_TYPE = new Guid("{6ec7f6c4-376b-45d7-9e9c-d337cedcd237}");
        private static Guid FWPM_CONDITION_IP_LOCAL_INTERFACE = new Guid("{4cd62a49-59c3-4969-b7f3-bda5d32890a4}");
        private static Guid FWPM_CONDITION_IP_LOCAL_PORT = new Guid("{0c1ba1af-5765-453f-af22-a8f791ac775b}");
        private static Guid FWPM_CONDITION_IP_PROTOCOL = new Guid("{3971ef2b-623e-4f9a-8cb1-6e79b806b9a7}");
        private static Guid FWPM_CONDITION_IP_REMOTE_ADDRESS = new Guid("{b235ae9a-1d64-49b8-a44c-5ff3d9095045}");
        private static Guid FWPM_CONDITION_IP_REMOTE_PORT = new Guid("{c35a604d-d22b-4e1a-91b4-68f674ee674b}");
        private static Guid FWPM_CONDITION_SUB_INTERFACE_INDEX = new Guid("{0cd42473-d621-4be3-ae8c-72a348d283e1}");
        private static Guid FWPM_CONDITION_TUNNEL_TYPE = new Guid("{77a40437-8779-4868-a261-f5a902f1c0cd}");
        public static Guid FWPM_CONDITION_ALE_APP_ID = new Guid("{d78e1e87-8644-4ea5-9437-d809ecefc971}");

        #endregion

        #region Layer Keys

        private static Guid FWPM_LAYER_INBOUND_IPPACKET_V4 = new Guid("{c86fd1bf-21cd-497e-a0bb-17425c885c58}");
        private static Guid FWPM_LAYER_OUTBOUND_IPPACKET_V4 = new Guid("{1e5c9fae-8a84-4135-a331-950b54229ecd}");
        private static Guid FWPM_LAYER_INBOUND_TRANSPORT_V4 = new Guid("{5926dfc8-e3cf-4426-a283-dc393f5d0f9d}");
        private static Guid FWPM_LAYER_ALE_AUTH_CONNECT_V4 = new Guid("{c38d57d1-05a7-4c33-904f-7fbceee60e82}");
        private static Guid FWPM_LAYER_ALE_FLOW_ESTABLISHED_V4 = new Guid("{af80470a-5596-4c13-9992-539e6fe57967}");
        private static Guid FWPM_LAYER_ALE_AUTH_LISTEN_V4 = new Guid("{88bb5dad-76d7-4227-9c71-df0a3ed7be7e}");
        private static Guid FWPM_LAYER_ALE_AUTH_RECV_ACCEPT_V4 = new Guid("{e1cd9fe7-f4b5-4273-96c0-592e487b8650}");
        private static Guid FWPM_LAYER_ALE_CONNECT_REDIRECT_V4 = new Guid("{c6e63c8c-b784-4562-aa7d-0a67cfcaf9a3}");
        public static Guid FWPM_LAYER_ALE_BIND_REDIRECT_V4 = new Guid("{66978cad-c704-42ac-86ac-7c1a231bd253}");
        private static Guid FWPM_LAYER_IPFORWARD_V4 = new Guid("{a82acc24-4ee1-4ee1-b465-fd1d25cb10a4}");

        #endregion

        public enum FwpFilterEnumType
        {

            FWP_FILTER_ENUM_FULLY_CONTAINED,

            FWP_FILTER_ENUM_OVERLAPPING,

            FWP_FILTER_ENUM_TYPE_MAX,
        }

        public enum FwpDataType
        {
            /// FWP_EMPTY -> 0
            FWP_EMPTY = 0,
            FWP_UINT8 = (FWP_EMPTY + 1),
            FWP_UINT16 = (FWP_UINT8 + 1),
            FWP_UINT32 = (FWP_UINT16 + 1),
            FWP_UINT64 = (FWP_UINT32 + 1),
            FWP_INT8 = (FWP_UINT64 + 1),
            FWP_INT16 = (FWP_INT8 + 1),
            FWP_INT32 = (FWP_INT16 + 1),
            FWP_INT64 = (FWP_INT32 + 1),
            FWP_FLOAT = (FWP_INT64 + 1),
            FWP_DOUBLE = (FWP_FLOAT + 1),
            FWP_BYTE_ARRAY16_TYPE = (FWP_DOUBLE + 1),
            FWP_BYTE_BLOB_TYPE = (FWP_BYTE_ARRAY16_TYPE + 1),
            FWP_SID = (FWP_BYTE_BLOB_TYPE + 1),
            FWP_SECURITY_DESCRIPTOR_TYPE = (FWP_SID + 1),
            FWP_TOKEN_INFORMATION_TYPE = (FWP_SECURITY_DESCRIPTOR_TYPE + 1),
            FWP_TOKEN_ACCESS_INFORMATION_TYPE = (FWP_TOKEN_INFORMATION_TYPE + 1),
            FWP_UNICODE_STRING_TYPE = (FWP_TOKEN_ACCESS_INFORMATION_TYPE + 1),
            FWP_BYTE_ARRAY6_TYPE = (FWP_UNICODE_STRING_TYPE + 1),
            FWP_SINGLE_DATA_TYPE_MAX = 0xff,
            FWP_V4_ADDR_MASK = (FWP_SINGLE_DATA_TYPE_MAX + 1),
            FWP_V6_ADDR_MASK = (FWP_V4_ADDR_MASK + 1),
            FWP_RANGE_TYPE = (FWP_V6_ADDR_MASK + 1),
            FWP_DATA_TYPE_MAX = (FWP_RANGE_TYPE + 1)
        }

        public enum FwpMatchType
        {
            FWP_MATCH_EQUAL = 0,
            FWP_MATCH_GREATER = (FWP_MATCH_EQUAL + 1),
            FWP_MATCH_LESS = (FWP_MATCH_GREATER + 1),
            FWP_MATCH_GREATER_OR_EQUAL = (FWP_MATCH_LESS + 1),
            FWP_MATCH_LESS_OR_EQUAL = (FWP_MATCH_GREATER_OR_EQUAL + 1),
            FWP_MATCH_RANGE = (FWP_MATCH_LESS_OR_EQUAL + 1),
            FWP_MATCH_FLAGS_ALL_SET = (FWP_MATCH_RANGE + 1),
            FWP_MATCH_FLAGS_ANY_SET = (FWP_MATCH_FLAGS_ALL_SET + 1),
            FWP_MATCH_FLAGS_NONE_SET = (FWP_MATCH_FLAGS_ANY_SET + 1),
            FWP_MATCH_EQUAL_CASE_INSENSITIVE = (FWP_MATCH_FLAGS_NONE_SET + 1),
            FWP_MATCH_NOT_EQUAL = (FWP_MATCH_EQUAL_CASE_INSENSITIVE + 1),
            FWP_MATCH_TYPE_MAX = (FWP_MATCH_NOT_EQUAL + 1)
        }

        /// Return Type: DWORD->unsigned int
        ///serverName: wchar_t*
        ///authnService: UINT32->unsigned int
        ///authIdentity: SEC_WINNT_AUTH_IDENTITY_W*
        ///session: FWPM_SESSION0*
        ///engineHandle: HANDLE*
        [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmEngineOpen0")]
        public static extern uint FwpmEngineOpen0([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string serverName, uint authnService, [System.Runtime.InteropServices.InAttribute()] System.IntPtr authIdentity, [System.Runtime.InteropServices.InAttribute()] System.IntPtr session, ref System.IntPtr engineHandle);

        /// Return Type: DWORD->int
        ///serverName: wchar_t*
        ///authnService: UINT32->int
        ///authIdentity: SEC_WINNT_AUTH_IDENTITY_W*
        ///session: FWPM_SESSION0*
        ///engineHandle: HANDLE*
        //[System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmEngineOpen0")]
        //public static extern int FwpmEngineOpen0([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string serverName, int authnService, ref SecWinNTAuthIdentity authIdentity, ref FwpmSession session, ref System.IntPtr engineHandle);

        /// Return Type: DWORD->int
        ///engineHandle: HANDLE->void*
        [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmEngineClose0")]
        public static extern int FwpmEngineClose0(System.IntPtr engineHandle);

        /// Return Type: DWORD->int
        ///engineHandle: HANDLE->void*
        ///filter: FWPM_FILTER0*
        ///sd: PSECURITY_DESCRIPTOR->PVOID->void*
        ///id: UINT64*
        [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmFilterAdd0")]
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public static extern int FwpmFilterAdd0(System.IntPtr engineHandle, ref FwpmFilter filter, System.IntPtr sd, ref long id);

        /// Return Type: DWORD->int
        ///engineHandle: HANDLE->void*
        ///id: UINT64->__int64
        [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmFilterDeleteById0")]
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public static extern int FwpmFilterDeleteById0(System.IntPtr engineHandle, long id);

        /// Return Type: DWORD->int
        ///engineHandle: HANDLE->void*
        ///key: GUID*
        [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmFilterDeleteByKey0")]
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public static extern int FwpmFilterDeleteByKey0(System.IntPtr engineHandle, ref Guid key);

        /// Return Type: DWORD->int
        ///engineHandle: HANDLE->void*
        ///subLayer: FWPM_SUBLAYER0*
        ///sd: PSECURITY_DESCRIPTOR->PVOID->void*
        [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmSubLayerAdd0")]
        public static extern int FwpmSubLayerAdd0(System.IntPtr engineHandle, ref FwpmSublayer subLayer, System.IntPtr sd);

        /// Return Type: DWORD->int
        ///engineHandle: HANDLE->void*
        ///key: GUID*
        [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmSubLayerDeleteByKey0")]
        public static extern int FwpmSubLayerDeleteByKey0(System.IntPtr engineHandle, ref Guid key);

        /// Return Type: DWORD->int
        ///fileName: PCWSTR->WCHAR*
        ///appId: FWP_BYTE_BLOB**
        [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmGetAppIdFromFileName0")]
        public static extern int FwpmGetAppIdFromFileName0([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string fileName, ref System.IntPtr appId);

        /// Return Type: DWORD->int
        ///engineHandle: HANDLE->void*
        ///enumHandle: HANDLE->void*
        ///numEntriesRequested: UINT32->int
        ///entries: FWPM_FILTER0***
        ///numEntriesReturned: UINT32*
        [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmFilterEnum0")]
        public static extern int FwpmFilterEnum0(System.IntPtr engineHandle, System.IntPtr enumHandle, int numEntriesRequested, ref System.IntPtr entries, ref int numEntriesReturned);

        /// Return Type: DWORD->int
        ///engineHandle: HANDLE->void*
        ///enumTemplate: FWPM_FILTER_ENUM_TEMPLATE0*
        ///enumHandle: HANDLE*
        [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmFilterCreateEnumHandle0")]
        public static extern int FwpmFilterCreateEnumHandle0(System.IntPtr engineHandle, ref FwpmFilterEnumTemplate enumTemplate, ref System.IntPtr enumHandle);

        /// Return Type: void
        ///p: void**
        [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmFreeMemory0")]
        public static extern void FwpmFreeMemory0(ref System.IntPtr p);

        /// Return Type: DWORD->int
        ///engineHandle: HANDLE->void*
        ///id: UINT64->__int64
        ///filter: FWPM_FILTER0**
        [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmFilterGetById0")]
        public static extern int FwpmFilterGetById0(System.IntPtr engineHandle, long id, ref System.IntPtr filter);

        /// Return Type: DWORD->int
        ///engineHandle: HANDLE->void*
        ///key: GUID*
        ///callout: FWPM_CALLOUT0**
        [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmCalloutGetByKey0")]
        public static extern int FwpmCalloutGetByKey0(System.IntPtr engineHandle, ref Guid key, ref System.IntPtr callout);
    }
}
