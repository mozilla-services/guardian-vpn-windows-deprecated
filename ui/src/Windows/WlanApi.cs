// <copyright file="WlanApi.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.Windows
{
    /// <summary>
    /// Wlanapi.dll pinvoke library.
    /// </summary>
    public class WlanApi
    {
        /// <summary>
        /// Defines a basic service set (BSS) network type.
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/nativewifi/dot11-bss-type">Further documentation</see>.
        /// </summary>
        public enum Dot11BssType
        {
            /// <summary>
            /// Specifies an infrastructure BSS network.
            /// </summary>
            Dot11BssTypeInfrastructure = 1,

            /// <summary>
            /// Specifies an independent BSS (IBSS) network.
            /// </summary>
            Dot11BssTypeIndependent = 2,

            /// <summary>
            /// Specifies either infrastructure or IBSS network.
            /// </summary>
            Dot11BssTypeAny = 3,
        }

        /// <summary>
        /// Defines an 802.11 PHY and media type.
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/nativewifi/dot11-phy-type">Further documentation</see>.
        /// </summary>
        public enum Dot11PhyType : uint
        {
            /// <summary>
            /// Specifies an unknown or uninitialized PHY type.
            /// </summary>
            Dot11PhyTypeUnknown = 0,

            /// <summary>
            /// Specifies any PHY type.
            /// </summary>
            Dot11PhyTypeAny = 0,

            /// <summary>
            /// Specifies a frequency-hopping spread-spectrum (FHSS) PHY. Bluetooth devices can use FHSS or an adaptation of FHSS.
            /// </summary>
            Dot11PhyTypeFhss = 1,

            /// <summary>
            /// Specifies a direct sequence spread spectrum (DSSS) PHY type.
            /// </summary>
            Dot11PhyTypeDsss = 2,

            /// <summary>
            /// Specifies an infrared (IR) baseband PHY type.
            /// </summary>
            Dot11PhyTypeIrbaseband = 3,

            /// <summary>
            /// Specifies an orthogonal frequency division multiplexing (OFDM) PHY type. 802.11a devices can use OFDM.
            /// </summary>
            Dot11PhyTypeOfdm = 4,

            /// <summary>
            /// Specifies a high-rate DSSS (HRDSSS) PHY type.
            /// </summary>
            Dot11PhyTypeHrdsss = 5,

            /// <summary>
            /// Specifies an extended rate PHY type (ERP). 802.11g devices can use ERP.
            /// </summary>
            Dot11PhyTypeErp = 6,

            /// <summary>
            /// Specifies the 802.11n PHY type.
            /// </summary>
            Dot11PhyTypeHt = 7,

            /// <summary>
            /// Specifies the 802.11ac PHY type. This is the very high throughput PHY type specified in IEEE 802.11ac. This value is supported on Windows 8.1, Windows Server 2012 R2, and later.
            /// </summary>
            Dot11PhyTypeVht = 8,

            /// <summary>
            /// Specifies the start of the range that is used to define PHY types that are developed by an independent hardware vendor (IHV).
            /// </summary>
            Dot11PhyTypeIhvStart = 0x80000000,

            /// <summary>
            /// Specifies the start of the range that is used to define PHY types that are developed by an independent hardware vendor (IHV).
            /// </summary>
            Dot11PhyTypeIhvEnd = 0xffffffff,
        }

        /// <summary>
        /// Indicates the state of an interface.
        /// <see href="https://msdn.microsoft.com/en-us/windows/ms706877(v=vs.71)">Further documentation</see>.
        /// </summary>
        public enum WlanInterfaceState
        {
            /// <summary>
            /// Interface is not ready.
            /// </summary>
            NotReady = 0,

            /// <summary>
            /// Interface is connected.
            /// </summary>
            Connected = 1,

            /// <summary>
            /// An ad-hoc network is formed.
            /// </summary>
            AdHocNetworkFormed = 2,

            /// <summary>
            /// Interface is disconnecting.
            /// </summary>
            Disconnecting = 3,

            /// <summary>
            /// Interface has disconnected.
            /// </summary>
            Disconnected = 4,

            /// <summary>
            /// Interface is associating.
            /// </summary>
            Associating = 5,

            /// <summary>
            /// Interface is discovering/scanning.
            /// </summary>
            Discovering = 6,

            /// <summary>
            /// Interface is trying to authenticate.
            /// </summary>
            Authenticating = 7,
        }

        /// <summary>
        /// Specifies the possible values of the NotificationCode member of the WlanNotificationData structure for Auto Configuration Module (ACM) notifications.
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/wlanapi/ne-wlanapi-wlan_notification_acm~r1">Further documentation</see>.
        /// </summary>
        public enum WlanNotificationAcm
        {
            #pragma warning disable SA1602 // EnumerationItemsMustBeDocumented
            Start = 0,
            AutoconfEnabled,
            AutoconfDisabled,
            BackgroundScanEnabled,
            BackgroundScanDisabled,
            BssTypeChange,
            PowerSettingChange,
            ScanComplete,
            ScanFail,
            ConnectionStart,
            ConnectionComplete,
            ConnectionAttemptFail,
            FilterListChange,
            InterfaceArrival,
            InterfaceRemoval,
            ProfileChange,
            ProfileNameChange,
            ProfilesExhausted,
            NetworkNotAvailable,
            NetworkAvailable,
            Disconnecting,
            Disconnected,
            AdhocNetworkStateChange,
            ProfileUnblocked,
            ScreenPowerChange,
            ProfileBlocked,
            ScanListRefresh,
            OperationalStateChange,
            End,
            #pragma warning restore SA1602 // EnumerationItemsMustBeDocumented
        }

        /// <summary>
        /// Specifies the possible values of the NotificationCode member of the WlanNotificationData structure for Media Specific Module (MSM) notifications.
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/wlanapi/ne-wlanapi-wlan_notification_msm~r1">Further documentation</see>.
        /// </summary>
        public enum WlanNotificationMsm
        {
            #pragma warning disable SA1602 // EnumerationItemsMustBeDocumented
            Start = 0,
            Associating,
            Associated,
            Authenticating,
            Connected,
            RoamingStart,
            RoamingEnd,
            RadioStateChange,
            SignalQualityChange,
            Disassociating,
            Disconnected,
            PeerJoin,
            PeerLeave,
            AdapterRemoval,
            AdapterOperationModeChange,
            LinkDegraded,
            LinkImproved,
            End,
            #pragma warning restore SA1602 // EnumerationItemsMustBeDocumented
        }

        /// <summary>
        /// Source of the Wlan change notification.
        /// <see href="https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms706902(v=vs.85)">Further documentation</see>.
        /// </summary>
        [Flags]
        public enum WlanNotificationSource : uint
        {
            /// <summary>
            /// No source.
            /// </summary>
            None = 0,

            /// <summary>
            /// All notifications, including those generated by the 802.1X module.
            /// </summary>
            All = 0X0000FFFF,

            /// <summary>
            /// Notifications generated by the auto configuration module.
            /// </summary>
            ACM = 0X00000008,

            /// <summary>
            /// Notifications generated by MSM.
            /// </summary>
            MSM = 0X00000010,

            /// <summary>
            /// Notifications generated by the security module.
            /// </summary>
            Security = 0X00000020,

            /// <summary>
            /// Notifications generated by independent hardware vendors (IHV).
            /// </summary>
            IHV = 0X00000040,
        }

        /// <summary>
        /// Various opcodes used to set and query parameters on a wireless interface.
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/wlanapi/ne-wlanapi-wlan_intf_opcode~r1">Further documentation</see>.
        /// </summary>
        public enum WlanIntfOpcode : uint
        {
            #pragma warning disable SA1602 // EnumerationItemsMustBeDocumented
            AutoconfStart = 0,
            AutoconfEnabled,
            BackgroundScanEnabled,
            MediaStreamingMode,
            RadioState,
            BssType,
            InterfaceState,
            CurrentConnection,
            ChannelNumber,
            SupportedInfrastructureAuthCipherPairs,
            SupportedAdhocAuthCipherPairs,
            SupportedCountryOrRegionStringList,
            CurrentOperationMode,
            SupportedSafeMode,
            CertifiedSafeMode,
            HostedNetworkCapable,
            ManagementFrameProtectionCapable,
            AutoconfEnd = 0x0fffffff,
            MsmStart = 0x10000100,
            Statistics,
            Rssi,
            MsmEnd = 0x1fffffff,
            SecurityStart = 0x20010000,
            SecurityEnd = 0x2fffffff,
            IhvStart = 0x30000000,
            IhvEnd = 0x3fffffff,
            #pragma warning restore SA1602 // EnumerationItemsMustBeDocumented

        }

        /// <summary>
        /// Wireless LAN authentication algorithm.
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/nativewifi/dot11-auth-algorithm">Further documentation</see>.
        /// </summary>
        public enum Dot11AuthAlgorithm : uint
        {
            /// <summary>
            /// Specifies an IEEE 802.11 Open System authentication algorithm.
            /// </summary>
            Open = 1,

            /// <summary>
            /// Specifies an 802.11 Shared Key authentication algorithm that requires the use of a pre-shared Wired Equivalent Privacy (WEP) key for the 802.11 authentication.
            /// </summary>
            SharedKey = 2,

            /// <summary>
            /// Specifies a Wi-Fi Protected Access (WPA) algorithm. IEEE 802.1X port authentication is performed by the supplicant, authenticator, and authentication server.
            /// </summary>
            Wpa = 3,

            /// <summary>
            /// Specifies a WPA algorithm that uses preshared keys (PSK).
            /// </summary>
            WpaPsk = 4,

            /// <summary>
            /// This value is not supported.
            /// </summary>
            WpaNone = 5,

            /// <summary>
            /// Specifies an 802.11i Robust Security Network Association (RSNA) algorithm.
            /// </summary>
            Rsna = 6,

            /// <summary>
            /// Specifies an 802.11i RSNA algorithm that uses PSK. IEEE 802.1X port authentication is performed by the supplicant and authenticator.
            /// </summary>
            RsnaPsk = 7,

            /// <summary>
            /// Indicates the start of the range that specifies proprietary authentication algorithms that are developed by an IHV.
            /// </summary>
            IhvStart = 0x80000000,

            /// <summary>
            /// Indicates the end of the range that specifies proprietary authentication algorithms that are developed by an IHV.
            /// </summary>
            IhvEnd = 0xffffffff,
        }

        /// <summary>
        /// Defines a cipher algorithm for data encryption and decryption.
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/nativewifi/dot11-cipher-algorithm">Further documentation</see>.
        /// </summary>
        public enum Dot11CipherAlgorithm : uint
        {
            /// <summary>
            /// Specifies that no cipher algorithm is enabled or supported.
            /// </summary>
            None = 0x00,

            /// <summary>
            /// Specifies a Wired Equivalent Privacy (WEP) algorithm, which is the RC4-based algorithm that is specified in the 802.11-1999 standard. This enumerator specifies the WEP cipher algorithm with a 40-bit cipher key.
            /// </summary>
            Wep40 = 0x01,

            /// <summary>
            /// Specifies a Temporal Key Integrity Protocol (TKIP) algorithm, which is the RC4-based cipher suite that is based on the algorithms that are defined in the WPA specification and IEEE 802.11i-2004 standard.
            /// </summary>
            Tkip = 0x02,

            /// <summary>
            /// Specifies an AES-CCMP algorithm, as specified in the IEEE 802.11i-2004 standard and RFC 3610. Advanced Encryption Standard (AES) is the encryption algorithm defined in FIPS PUB 197.
            /// </summary>
            Ccmp = 0x04,

            /// <summary>
            /// Specifies a WEP cipher algorithm with a 104-bit cipher key.
            /// </summary>
            Wep104 = 0x05,

            /// <summary>
            /// Specifies a Wi-Fi Protected Access (WPA) Use Group Key cipher suite. For more information about the Use Group Key cipher suite, refer to Clause 7.3.2.25.1 of the IEEE 802.11i-2004 standard.
            /// </summary>
            WpaUseGroup = 0x100,

            /// <summary>
            /// Specifies a Robust Security Network (RSN) Use Group Key cipher suite. For more information about the Use Group Key cipher suite, refer to Clause 7.3.2.25.1 of the IEEE 802.11i-2004 standard.
            /// </summary>
            RsnUseGroup = 0x100,

            /// <summary>
            /// Specifies a WEP cipher algorithm with a cipher key of any length.
            /// </summary>
            Wep = 0x101,

            /// <summary>
            /// Specifies the start of the range that is used to define proprietary cipher algorithms that are developed by an independent hardware vendor (IHV).
            /// </summary>
            IhvStart = 0x80000000,

            /// <summary>
            /// Specifies the end of the range that is used to define proprietary cipher algorithms that are developed by an IHV.
            /// </summary>
            IhvEnd = 0xffffffff,
        }

        /// <summary>
        /// Defines the mode of connection.
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/wlanapi/ne-wlanapi-wlan_connection_mode">Further documentation</see>.
        /// </summary>
        public enum WlanConnectionMode
        {
            /// <summary>
            /// A profile will be used to make the connection.
            /// </summary>
            Profile,

            /// <summary>
            /// A temporary profile will be used to make the connection.
            /// </summary>
            TemporaryProfile,

            /// <summary>
            /// Secure discovery will be used to make the connection.
            /// </summary>
            DiscoverySecure,

            /// <summary>
            /// Unsecure discovery will be used to make the connection.
            /// </summary>
            DiscoveryUnsecure,

            /// <summary>
            /// The connection is initiated by the wireless service automatically using a persistent profile.
            /// </summary>
            Auto,

            /// <summary>
            /// Not used.
            /// </summary>
            Invalid,
        }

        /// <summary>
        /// Opens a connection to the server.
        /// </summary>
        /// <param name="dwClientVersion">The highest version of the WLAN API that the client supports.</param>
        /// <param name="pReserved">Reserved for future use. Must be set to NULL.</param>
        /// <param name="pdwNegotiatedVersion">The version of the WLAN API that will be used in this session. This value is usually the highest version supported by both the client and server.</param>
        /// <param name="phClientHandle">A handle for the client to use in this session. This handle is used by other functions throughout the session.</param>
        /// <returns>If the function succeeds, the return value is ERROR_SUCCESS (0).</returns>
        [DllImport("Wlanapi.dll")]
        public static extern uint WlanOpenHandle(uint dwClientVersion, IntPtr pReserved, out uint pdwNegotiatedVersion, out IntPtr phClientHandle);

        /// <summary>
        /// The WlanCloseHandle function closes a connection to the server.
        /// </summary>
        /// <param name="hClientHandle">The client's session handle, which identifies the connection to be closed. This handle was obtained by a previous call to the WlanOpenHandle function.</param>
        /// <param name="pReserved">Reserved for future use. Set this parameter to NULL.</param>
        /// <returns>If the function succeeds, the return value is ERROR_SUCCESS (0).</returns>
        [DllImport("Wlanapi.dll")]
        public static extern uint WlanCloseHandle(IntPtr hClientHandle, IntPtr pReserved);

        /// <summary>
        /// Frees memory. Any memory returned from Native Wifi functions must be freed.
        /// </summary>
        /// <param name="pMemory">Pointer to the memory to be freed.</param>
        [DllImport("Wlanapi.dll")]
        public static extern void WlanFreeMemory(IntPtr pMemory);

        /// <summary>
        /// Queries various parameters of a specified interface.
        /// </summary>
        /// <param name="hClientHandle">The client's session handle, obtained by a previous call to the WlanOpenHandle function.</param>
        /// <param name="pInterfaceGuid">The GUID of the interface to be queried.</param>
        /// <param name="opCode">A WLAN_INTF_OPCODE value that specifies the parameter to be queried.</param>
        /// <param name="pReserved">Reserved for future use. Must be set to NULL.</param>
        /// <param name="pdwDataSize">The size of the ppData parameter, in bytes.</param>
        /// <param name="ppData">Pointer to the memory location that contains the queried value of the parameter specified by the OpCode parameter.</param>
        /// <param name="pWlanOpcodeValueType">If passed a non-NULL value, points to a WlanOpcodeValueType value that specifies the type of opcode returned. This parameter may be NULL.</param>
        /// <returns>If the function succeeds, the return value is ERROR_SUCCESS (0).</returns>
        [DllImport("Wlanapi.dll")]
        public static extern uint WlanQueryInterface(IntPtr hClientHandle, [MarshalAs(UnmanagedType.LPStruct)] Guid pInterfaceGuid, WlanIntfOpcode opCode, IntPtr pReserved, out uint pdwDataSize, ref IntPtr ppData, IntPtr pWlanOpcodeValueType);

        /// <summary>
        /// Contains information provided when registering for notifications.
        /// </summary>
        /// <remarks>
        /// Corresponds to the native <c>WlanNotificationData</c> type.
        /// </remarks>
        /// <see href="https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms706902(v=vs.85)" />
        [StructLayout(LayoutKind.Sequential)]
        public struct WlanNotificationData
        {
            /// <summary>
            /// Specifies where the notification comes from.
            /// </summary>
            /// <remarks>
            /// On Windows XP SP2, this field must be set to <see cref="WlanNotificationSource.None" />, <see cref="WlanNotificationSource.All" /> or <see cref="WlanNotificationSource.ACM" />.
            /// </remarks>
            public WlanNotificationSource NotificationSource;

            /// <summary>
            /// Indicates the type of notification. The value of this field indicates what type of associated data will be present in <see cref="DataPtr" />.
            /// </summary>
            public int NotificationCode;

            /// <summary>
            /// Indicates which interface the notification is for.
            /// </summary>
            public Guid InterfaceGuid;

            /// <summary>
            /// Specifies the size of <see cref="DataPtr" />, in bytes.
            /// </summary>
            public int DataSize;

            /// <summary>
            /// Pointer to additional data needed for the notification, as indicated by <see cref="NotificationCode" />.
            /// </summary>
            public IntPtr DataPtr;
        }

        /// <summary>
        /// Contains association attributes for a connection.
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/wlanapi/ns-wlanapi-wlan_association_attributes" />.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct WlanAssociationAttributes
        {
            /// <summary>
            /// A Dot11Ssid structure that contains the SSID of the association.
            /// </summary>
            public WlanApiStructures.WlanApiAdditionalStructures.Dot11Ssid Dot11Ssid;

            /// <summary>
            /// A Dot11BssType value that specifies whether the network is infrastructure or ad hoc.
            /// </summary>
            public Dot11BssType Dot11BssType;

            /// <summary>
            /// A Dot11MacAddress that contains the BSSID of the association.
            /// </summary>
            public WlanApiStructures.WlanApiAdditionalStructures.Dot11MacAddress Dot11Bssid;

            /// <summary>
            /// A Dot11PhyType value that indicates the physical type of the association.
            /// </summary>
            public Dot11PhyType Dot11PhyType;

            /// <summary>
            /// The position of the Dot11PhyIndex value in the structure containing the list of PHY types.
            /// </summary>
            public uint Dot11PhyIndex;

            /// <summary>
            /// A percentage value that represents the signal quality of the network.
            /// This member contains a value between 0 and 100. A value of 0 implies an actual RSSI signal strength of -100 dbm.
            /// A value of 100 implies an actual RSSI signal strength of -50 dbm.
            /// You can calculate the RSSI signal strength value for wlanSignalQuality values between 1 and 99 using linear interpolation.
            /// </summary>
            public uint WlanSignalQuality;

            /// <summary>
            /// Contains the receiving rate of the association.
            /// </summary>
            public uint RxRate;

            /// <summary>
            /// Contains the transmission rate of the association.
            /// </summary>
            public uint TxRate;
        }

        /// <summary>
        /// Defines the security attributes for a wireless connection.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/wlanapi/ns-wlanapi-wlan_security_attributes" />
        [StructLayout(LayoutKind.Sequential)]
        public struct WlanSecurityAttributes
        {
            /// <summary>
            /// Indicates whether security is enabled for this connection.
            /// </summary>
            [MarshalAs(UnmanagedType.Bool)]
            public bool SecurityEnabled;

            /// <summary>
            /// Indicates whether 802.1X is enabled for this connection.
            /// </summary>
            [MarshalAs(UnmanagedType.Bool)]
            public bool OneXEnabled;

            /// <summary>
            /// A Dot11AuthAlgorithm value that identifies the authentication algorithm.
            /// </summary>
            public Dot11AuthAlgorithm Dot11AuthAlgorithm;

            /// <summary>
            /// A Dot11CipherAlgorithm value that identifies the cipher algorithm.
            /// </summary>
            public Dot11CipherAlgorithm Dot11CipherAlgorithm;
        }

        /// <summary>
        /// Defines the attributes of a wireless connection.
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/wlanapi/ns-wlanapi-wlan_connection_attributes" />.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WlanConnectionAttributes
        {
            /// <summary>
            /// A WlanInterfaceState value that indicates the state of the interface.
            /// </summary>
            public WlanInterfaceState IsState;

            /// <summary>
            /// A WlanConnectionMode value that indicates the mode of the connection.
            /// </summary>
            public WlanConnectionMode WlanConnectionMode;

            /// <summary>
            /// The name of the profile used for the connection. Profile names are case-sensitive.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string ProfileName;

            /// <summary>
            /// A WlanAssociationAttributes structure that contains the attributes of the association.
            /// </summary>
            public WlanAssociationAttributes WlanAssociationAttributes;

            /// <summary>
            /// A WlanSecurityAttributes structure that contains the security attributes of the connection.
            /// </summary>
            public WlanSecurityAttributes WlanSecurityAttributes;
        }

        /// <summary>
        /// Contains information about a wireless LAN interface.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/wlanapi/ns-wlanapi-wlan_interface_info" />.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WlanInterfaceInfo
        {
            /// <summary>
            /// Contains the GUID of the interface.
            /// </summary>
            public Guid InterfaceGuid;

            /// <summary>
            /// Contains the description of the interface.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string StrInterfaceDescription;

            /// <summary>
            /// Contains a WlanInterfaceState value that indicates the current state of the interface.
            /// </summary>
            public WlanInterfaceState IsState;
        }
    }
}
