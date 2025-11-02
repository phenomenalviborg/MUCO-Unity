using System;
using UnityEngine;

namespace Muco {
    public class FingerProviderTransforms : FingerProvider {
        public Transform[] transforms;
        public float pinchDist;
        public override int FingerCount() {
            return transforms.Length;
        }
        [Tooltip("Pico: palm")]
        public Transform curlTargetFinger;
        [Tooltip("Pico: index_intermediate")]
        public Transform curlTargetThumb;
        [Tooltip("Normalized to 1-0")]
        public float curlThresholdFinger = .75f;
        public float[] distancesToWristUncurled =  new float[5];
        public HandPoseDetection handPoseDetection;
        public Transform smoothHandReference;
        public override void UpdateFingers(Span<Finger> fingers) {
            var thumbPos = transforms[0].position;
            for (int i = 0; i < transforms.Length; i++)
            {
                bool isPinching = false;
                var finger = fingers[i];
                var pos = transforms[i].position;

                finger.lastPosition = finger.currentPosition;
                finger.currentPosition = pos;

                float distToTarget = Vector3.Distance(i == 0 ? curlTargetThumb.position : curlTargetFinger.position, pos);
                bool isCurled = distToTarget < (distancesToWristUncurled[i] * curlThresholdFinger);

                if (i != 0)
                {
                    var distToThumb = Vector3.Distance(thumbPos, pos);
                    if (distToThumb < pinchDist)
                    {
                        isPinching = true;
                    }
                }

                finger.wasPinching = finger.isPinching;
                finger.isPinching = isPinching;
                finger.confidence = true;
                finger.hand = curlTargetFinger;
                finger.isCurled = isCurled;
                fingers[i] = finger;
            }
            handPoseDetection?.Detect(fingers);
        }
    }
}
