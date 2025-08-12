using UnityEngine.XR.Hands;

namespace Muco {
    public class HandTrackingConfidenceOpenXr : HandTrackingConfidenceProvider {
        public XRHandMeshController meshController;
        public override bool GetConfidence() {
            return meshController.handIsTracked;
        }
    }
}
