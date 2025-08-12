using System;
using System.Collections.Generic;
using UnityEngine;

namespace Muco {
    public class FingerProvider : MonoBehaviour {
        [System.Serializable]
        public enum FingerState {
            Default,
            Dragging,
            Movable,
        }

        [System.Serializable]
        public struct Finger {
            public Vector3 lastPosition;
            public Vector3 currentPosition;
            public List<Interactible> currentInteractibles;
            public Interactible activeInteractible;
            public Vector3 pickUpLocalPosition;
            public Quaternion pickUpLocalRotation;
            public Transform hand;
            public bool isPinching;
            public bool wasPinching;
            public FingerState fingerState;
            public bool confidence;
            public bool isCurled;
            public void Init() {
                currentInteractibles = new List<Interactible>();
            }
        }

        public virtual int FingerCount() { return 0; }

        public virtual void UpdateFingers(Span<Finger> fingers) {}
    }
}
