/*++

Copyright (c) Microsoft Corporation. All rights reserved

Abstract:

   This file implements the classifyFn, notifiFn, and flowDeleteFn callout
   functions for the flow-established and datagram-data callouts. In addition
   the system worker thread that performs the actual packet modifications
   is also implemented here along with the eventing mechanisms shared between
   the classify function and the worker thread.

   Packet modification is done out-of-band by a system worker thread using 
   the reference-drop-clone-modify-reinject mechanism. Therefore the sample 
   can serve as a base in scenarios where filtering/modification decision 
   cannot be made within the classifyFn() callout and instead must be made, 
   for example, by an user-mode application.

Environment:

    Kernel mode

--*/

#include <ntddk.h>
#include <ip2string.h>
#pragma warning(push)
#pragma warning(disable:4201)       // unnamed struct/union
#include <fwpsk.h>
#include <mstcpip.h>
#pragma warning(pop)
#include <fwpmk.h>
#include "DD_proxy.h"

// b16b0a6e-2b2a-41a3-8b39-bd3ffc855ff8
DEFINE_GUID(
    DD_PROXY_CALLOUT_V4,
    0xb16b0a6e,
    0x2b2a,
    0x41a3,
    0x8b, 0x39, 0xbd, 0x3f, 0xfc, 0x85, 0x5f, 0xf8
);

typedef struct BIND_REDIRECT_DATA
{
    IN_ADDR localAddress;
} BIND_REDIRECT_DATA;

void
PrintAddressV4(
    UINT32 IpAddress
)
{
    do {
        DbgPrintEx(DPFLTR_IHVDRIVER_ID, 0, \
            "%u.%u.%u.%u", \
            (UINT32)((IpAddress) >> 24), \
            (UINT32)(((IpAddress) >> 16) & 0xFF), \
            (UINT32)(((IpAddress) >> 8) & 0xFF), \
            (UINT32)((IpAddress) & 0xFF));
    } while (0);
}

void
DDProxyFlowEstablishedClassify(
   _In_ const FWPS_INCOMING_VALUES0* inFixedValues,
   _In_ const FWPS_INCOMING_METADATA_VALUES0* inMetaValues,
   _Inout_opt_ void* layerData,
   _In_opt_ const void* classifyContext,
   _In_ const FWPS_FILTER* filter,
   _In_ UINT64 flowContext,
   _Inout_ FWPS_CLASSIFY_OUT0* classifyOut
   )

    /* ++

       This is the classifyFn function for the ALE connect (v4 and v6) callout.
       For an initial classify (where the FWP_CONDITION_FLAG_IS_REAUTHORIZE flag
       is not set), it is queued to the connection list for inspection by the
       worker thread. For re-auth, we first check if it is triggered by an ealier
       FwpsCompleteOperation0 call by looking for an pended connect that has been
       inspected. If found, we remove it from the connect list and return the
       inspection result; otherwise we can conclude that the re-auth is triggered
       by policy change so we queue it to the packet queue to be process by the
       worker thread like any other regular packets.

    -- */
{

    DbgPrint("[DEBUG] Callout function, DDProxyFlowEstablishedClassify, is invoked\n");
    UNREFERENCED_PARAMETER(inFixedValues);
    UNREFERENCED_PARAMETER(inMetaValues);
    UNREFERENCED_PARAMETER(flowContext);
    UNREFERENCED_PARAMETER(layerData);

    if (inFixedValues == NULL) {
        DbgPrint("[DEBUG] inFixedValues is NULL\n");
        return;
    }

    if (classifyContext == NULL)
    {
        DbgPrint("[DEBUG] classifyContext is NULL\n");
        return;
    }

    if (filter == NULL)
    {
        DbgPrint("[DEBUG] filter is NULL\n");
        return;
    }

    /*
    if (filter->providerContext == NULL)
    {
        DbgPrint("[DEBUG] providerContext is NULL\n");
        return;
    }

    if (filter->providerContext->type != FWPM_GENERAL_CONTEXT)
    {
        DbgPrint("[DEBUG] Provider context type is not FWPM_GENERAL_CONTEXT\n");
        return;
    }

    if (filter->providerContext->dataBuffer == NULL)
    {
        DbgPrint("[DEBUG] Provider context data buffer is NULL\n");
        return;
    }

    
    auto size = filter->providerContext->dataBuffer->size;
    if (size != sizeof(BIND_REDIRECT_DATA))
    {
        DbgPrint("[DEBUG] Provider context data size is %i instead of expected %i\n", size, sizeof(BIND_REDIRECT_DATA));
        return;
    }

    if (filter->providerContext->dataBuffer->data == NULL)
    {
        DbgPrint("[DEBUG] Provider context data not specified\n");
        return;
    }
    */

    UINT64 ClassifyHandle = 0;
    NTSTATUS Status = STATUS_SUCCESS;
    FWPS_BIND_REQUEST0* pModifiedLayerData = NULL;
    HANDLE hRedirectHandle = NULL;
    //UINT16 ui16NewLocalPort = 27015;
    //DWORD dwProcessID = 1072;
    UNICODE_STRING IPNumber;
    PWSTR terminator;

    if (!(classifyOut->rights & FWPS_RIGHT_ACTION_WRITE)) {
        DbgPrint("[DEBUG] Insufficient permissions to modify classify out\n");
        return;
    }

    DbgPrint("[DEBUG] Doing FwpsAcquireClassifyHandle0\n");

    Status = FwpsAcquireClassifyHandle0((void*)classifyContext, (UINT32)0, &ClassifyHandle);
    if (!NT_SUCCESS(Status))
    {
        DbgPrint("[DEBUG] Failed at FwpsAcquireClassifyHandle0\n");
        return;
    }
    
    Status = FwpsRedirectHandleCreate0(&DD_PROXY_CALLOUT_V4, 0, &hRedirectHandle);
    if (!NT_SUCCESS(Status))
    {
        DbgPrint("[DEBUG] Failed at FwpsRedirectHandleCreate0\n");
        return;
    }

    FWPS_CONNECTION_REDIRECT_STATE redirectState = FwpsQueryConnectionRedirectState0(
        inMetaValues->redirectRecords,
        hRedirectHandle,
        NULL
    );
    

    if (redirectState != FWPS_CONNECTION_NOT_REDIRECTED) {
        DbgPrint("[DEBUG] Connection was already redirected (presumably by us). Ignoring it.\n");
        return;
    }


    DbgPrint("[DEBUG] Doing FwpsAcquireWritableLayerDataPointer0\n");
    Status = FwpsAcquireWritableLayerDataPointer0(ClassifyHandle, filter->filterId, (UINT32)0, (PVOID*)(&pModifiedLayerData), classifyOut);
    if (!NT_SUCCESS(Status))
    {
        DbgPrint("[DEBUG] Failed at FwpsAcquireWritableLayerDataPointer0\n");
    }

    for (FWPS_BIND_REQUEST0* bindRequest = pModifiedLayerData->previousVersion;
        bindRequest != NULL;
        bindRequest = bindRequest->previousVersion)
    {
        if (bindRequest->modifierFilterId == filter->filterId)

        {
            // Don't redirect the same socket more than once
            DbgPrint("[DEBUG] Already redirected!  Returning...\n");
            return;
        }
    }

    // Current local address & port
    SOCKADDR_IN* localAddr = (SOCKADDR_IN*)&pModifiedLayerData->localAddressAndPort;
    UINT32 localAddrIp = localAddr->sin_addr.S_un.S_addr;
    UINT16 localAddrPort = localAddr->sin_port;

    // New local address & port
    IN_ADDR myNewLocalAddressV4;
    RtlInitUnicodeString(&IPNumber, L"10.0.2.15");
    Status = RtlIpv4StringToAddressW((PCWSTR)(IPNumber.Buffer), TRUE, &terminator, &myNewLocalAddressV4);

    DbgPrint("[DEBUG] In ALEConnectRedirectClassifyFn, intercepting connection: From ");
    PrintAddressV4(localAddrIp);
    DbgPrint(":%d\n", localAddrPort);

    DbgPrint("[DEBUG] In ALEConnectRedirectClassifyFn, redirecting connection: To "); 
    PrintAddressV4(myNewLocalAddressV4.S_un.S_addr);
    DbgPrint(":%d\n", localAddrPort);

    DbgPrint("[DEBUG] INETADDR_SET_ADDRESS: %d\n", myNewLocalAddressV4.S_un.S_un_b);
    BIND_REDIRECT_DATA tempData;
    tempData.localAddress = myNewLocalAddressV4;
    BIND_REDIRECT_DATA* redirectData = &tempData;
    INETADDR_SET_ADDRESS((PSOCKADDR)&(pModifiedLayerData->localAddressAndPort), &(redirectData->localAddress.S_un.S_un_b.s_b1));
    
    pModifiedLayerData->modifierFilterId = filter->filterId;
    classifyOut->actionType = FWP_ACTION_PERMIT;
    classifyOut->rights ^= FWPS_RIGHT_ACTION_WRITE;

    if (pModifiedLayerData) {
        DbgPrint("[%d] : %s\n", PsGetCurrentProcessId(), "[DEBUG] Doing FwpsApplyModifiedLayerData0\n");
        FwpsApplyModifiedLayerData0(ClassifyHandle, (PVOID*)(&pModifiedLayerData), 0);
    }
    else {
        DbgPrint("[%d] : %s\n", PsGetCurrentProcessId(), "[DEBUG] Unable to do FwpsApplyModifiedLayerData0\n");
    }
   
    if (ClassifyHandle) {
        DbgPrint("[DEBUG] Doing FwpsReleaseClassifyHandle0\n");
        FwpsReleaseClassifyHandle0(ClassifyHandle);
    }
    else {
        DbgPrint("[DEBUG] Unable to do FwpsReleaseClassifyHandle0\n");
    }

    return;
}

NTSTATUS
DDProxyFlowEstablishedNotify(
   _In_ FWPS_CALLOUT_NOTIFY_TYPE notifyType,
   _In_ const GUID* filterKey,
   _Inout_ const FWPS_FILTER* filter
   )
{
   UNREFERENCED_PARAMETER(notifyType);
   UNREFERENCED_PARAMETER(filterKey);
   UNREFERENCED_PARAMETER(filter);

   return STATUS_SUCCESS;
}