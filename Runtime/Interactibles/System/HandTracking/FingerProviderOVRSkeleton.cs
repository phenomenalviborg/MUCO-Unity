using System;
using UnityEngine;

namespace Muco {
    public class FingerProviderOVRSkeleton : FingerProvider {
        // public OVRSkeleton handSkeleton;

        public HandPoseDetection handPoseDetection;

        public override int FingerCount() {
            return 5;
        }

        public override void UpdateFingers(Span<Finger> fingers) {
            // if (handSkeleton.Bones.Count == 0)
            //     return;

            // var skeletonType = handSkeleton.GetSkeletonType();
            // var hand = skeletonType == OVRSkeleton.SkeletonType.HandLeft ? OVRPlugin.Hand.HandLeft : OVRPlugin.Hand.HandRight;

            // OVRPlugin.HandState handState = new OVRPlugin.HandState();
            // OVRPlugin.GetHandState(OVRPlugin.Step.Physics, hand, ref handState);

            // if (handState.FingerConfidences == null)
            //     return;

            // for (int i = 0; i < 5; i++) {
            //     var fingerConfidence = handState.FingerConfidences[i];
            //     var confident = fingerConfidence == OVRPlugin.TrackingConfidence.High;

            //     var t = handSkeleton.Bones[i + 19].Transform;

            //     var finger = fingers[fingerIndex];
            //     finger.lastPosition = finger.currentPosition;
            //     finger.currentPosition = t.position;
            //     finger.confidence = confident;

            //     var pinches = (int)handState.Pinches & 0b11110;
            //     var isPinched = ((1 << i) & pinches) != 0;

            //     if (!confident)
            //         isPinched = false;

            //     finger.wasPinching = finger.isPinching;
            //     finger.isPinching = isPinched;
            //     finger.hand = transform;
            //     fingers[fingerIndex] = finger;
            //     fingerIndex++;
            // }
            // handPoseDetection?.Detect(fingers);
            Debug.Log("Don't foget to fix Finger provider");
        }
    }
}
