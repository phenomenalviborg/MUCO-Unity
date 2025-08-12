using UnityEngine;

namespace Muco {
    public class HandTrackingConfidenceProvider : MonoBehaviour {
        public Renderer meshRenderer;
        public virtual bool GetConfidence() { return true;}

        void Update() {
            meshRenderer.enabled = GetConfidence();
        }
    }
}
