using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FirefoxPrivateNetwork.Windows.FwpuclntStructures;
using static FirefoxPrivateNetwork.Windows.Fwpuclnt;

namespace FirefoxPrivateNetwork.Network
{
    class SplitTunnel
    {
        public void FilterAddPerAppCallout()
        {
            var sublayerKey = new Guid("{0104fd7e-c825-414e-94c9-f0d525bbc169}");
            var calloutKey = new Guid("{b16b0a6e-2b2a-41a3-8b39-bd3ffc855ff8}");

            // Add temporary filters which don't survive reboots or crashes
            FwpmSession session = new FwpmSession();
            //session.flags = FWPM_SESSION_FLAG_DYNAMIC;
            session.flags = 0;
            IntPtr sessionPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(FwpmSession)));
            Marshal.StructureToPtr(session, sessionPtr, false);

            // Open a session to the filter engine
            IntPtr engineHandle = IntPtr.Zero;
            if (FwpmEngineOpen0(null, RPC_C_AUTHN_WINNT, IntPtr.Zero, sessionPtr, ref engineHandle) != 0)
            {
                MessageBox.Show("Unable to open filter engine");
                return;
            }

            // Create a sublayer
            FwpmSublayer sublayer = new FwpmSublayer
            {
                subLayerKey = sublayerKey,
                displayData = new FwpmDisplayData
                {
                    name = "Firefox Private Network Sublayer",
                    description = "Sublayer for split tunneling callouts",
                },
                weight = 0,
                flags = 0,
            };

            try
            {
                var sublayerAddError = FwpmSubLayerAdd0(engineHandle, ref sublayer, IntPtr.Zero);
                if (sublayerAddError != 0)
                {
                    MessageBox.Show("Could not add the sublayer. " + sublayerAddError.ToString());
                }
                else
                {
                    MessageBox.Show("Managed to add the sublayer. " + sublayerAddError.ToString());
                }

                Marshal.ThrowExceptionForHR((int)sublayerAddError);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

            // Get the callout by key
            IntPtr calloutPtr = IntPtr.Zero;
            try
            {
                var getCalloutError = FwpmCalloutGetByKey0(engineHandle, ref calloutKey, ref calloutPtr);
                if (getCalloutError != 0)
                {
                    MessageBox.Show("Could not get callout. " + getCalloutError.ToString());
                }
                else
                {
                    MessageBox.Show("Managed to get the callout. " + getCalloutError.ToString());
                }

                Marshal.ThrowExceptionForHR((int)getCalloutError);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

            FwpmCallout callout = (FwpmCallout)Marshal.PtrToStructure(calloutPtr, typeof(FwpmCallout));
            MessageBox.Show("Callout was found.  Key: " + callout.calloutKey);
            MessageBox.Show("Callout was found.  Name: " + callout.displayData.name + " Description: " + callout.displayData.description);

            // Get application Id from filename
            var chrome_86 = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";

            IntPtr appId = IntPtr.Zero;
            if (FwpmGetAppIdFromFileName0(chrome_86, ref appId) != 0)
            {
                MessageBox.Show("Failed get app id from filename");
                return;
            }

            // Define filter conditions
            var filterCondition1 = new FwpmFilterCondition
            {
                fieldKey = FWPM_CONDITION_ALE_APP_ID,
                matchType = FwpMatchType.FWP_MATCH_EQUAL,
                conditionValue = new FwpConditionValue
                {
                    type = FwpDataType.FWP_BYTE_BLOB_TYPE,
                    Union1 = new UnionType
                    {
                        byteBlob = appId,
                    },
                },
            };

            // Create array of filter conditions
            FwpmFilterCondition[] filterConditions = new FwpmFilterCondition[1];
            filterConditions[0] = filterCondition1;

            // Marshal array of filter conditions to pointer
            int arrayLen = filterConditions.Length;
            int structSize = Marshal.SizeOf(typeof(FwpmFilterCondition));
            IntPtr filterConditionsPtr = Marshal.AllocCoTaskMem(filterConditions.Length * Marshal.SizeOf(typeof(FwpmFilterCondition)));
            for (int i = 0; i < arrayLen; i++)
            {
                Marshal.StructureToPtr(filterConditions[i], (IntPtr)(filterConditionsPtr.ToInt64() + i * structSize), false);
            }

            // Define the filter
            var filter = new FwpmFilter
            {
                layerKey = FWPM_LAYER_ALE_BIND_REDIRECT_V4,
                subLayerKey = sublayerKey,
                displayData = new FwpmDisplayData
                {
                    name = "Firefox Private Network",
                    description = "Bind Redirect Filter",
                },
                action = new FwpmAction
                {
                    type = FWP_ACTION_CALLOUT_UNKNOWN,
                    Union1 = new UnionType2
                    {
                        calloutKey = callout.calloutKey,
                    },
                },
                weight = new FwpValue
                {
                    type = FwpDataType.FWP_EMPTY,
                },
                filterCondition = filterConditionsPtr,
                numFilterConditions = (uint)filterConditions.Length,
                rawContext = 0,
            };

            // Add the filter
            try
            {
                var error = FwpmFilterAdd0(engineHandle, ref filter, IntPtr.Zero, ref filter.filterId);
                if (error != 0)
                {
                    MessageBox.Show("Filter did not get added. Boo. " + error.ToString());
                }
                else
                {
                    MessageBox.Show("Filter seemingly added successfully. " + error.ToString());
                }

                Marshal.ThrowExceptionForHR((int)error);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

            // Close the session to the filter engine
            if (FwpmEngineClose0(engineHandle) != 0)
            {
                MessageBox.Show("Can't close");
                return;
            }
        }
    }
}
