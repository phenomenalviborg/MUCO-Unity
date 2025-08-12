using Muco;

namespace Modo {
    public class HandTrackingConfidenceStub : HandTrackingConfidenceProvider {
        public bool confidence;

        public override bool GetConfidence() {
            return confidence;
        }
    }
}
