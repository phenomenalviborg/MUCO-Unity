using UnityEngine;

namespace Muco {
    public class HandTrackingConfidenceQuest : HandTrackingConfidenceProvider {
        // public OVRSkeleton handSkeleton;

        public override bool GetConfidence() {
            // return handSkeleton.IsDataHighConfidence;
            Debug.Log("Don't foget to fix tracking confidence");
            return true;
        }
    }
}
