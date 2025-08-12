using UnityEngine;
using Muco;
// using Unity.XR.PXR;

namespace Modo {
    public class HandTrackingConfidencePico : HandTrackingConfidenceProvider {
        // public Unity.XR.PXR.HandType handType;
        // private HandJointLocations handJointLocations = new HandJointLocations();

        public override bool GetConfidence() {
            // if (PXR_HandTracking.GetJointLocations(handType, ref handJointLocations)) {
            //     if (handJointLocations.isActive == 0)
            //         return false;
            //     return true;
            // }
            // return false;
            Debug.Log("Don't foget to fix tracking confidene");
            return true;
        }
    }
}
