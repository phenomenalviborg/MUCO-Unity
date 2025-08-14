using Muco;

namespace Muco {
    public class HandTrackingConfidenceStub : HandTrackingConfidenceProvider {
        public bool confidence;

        public override bool GetConfidence() {
            return confidence;
        }
    }
}
