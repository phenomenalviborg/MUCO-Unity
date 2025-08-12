using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Muco {
    public class HandPoseDetection : MonoBehaviour {
        public HandPoseType handPoseType = HandPoseType.Open;
        public void Detect(Span<FingerProvider.Finger> fingers) {
            List<bool> fingersCurled = new List<bool>();
            foreach(var finger in fingers) {
                fingersCurled.Add(finger.isCurled);
            }
            // bool[] fingersCurled = fingers.Select(x => x.isCurled).ToArray();

            string debugString = "";
            foreach (var x in fingersCurled) {
                if (x)
                    debugString += "1";
                else
                    debugString += "0";
            }
            var name = gameObject.name;
            VrDebug.SetValue("HandPose", name, debugString);

            int curlCount = fingersCurled.Count(b => b);
            int bitmask = FingerArrayToBitmask(fingersCurled);
            if (poseMap.TryGetValue(bitmask, out HandPoseType matchedHandPoseType))
            {
                handPoseType = matchedHandPoseType;
            }
            else
            {
                handPoseType = HandPoseType.Open;
            }

            VrDebug.SetValue("HandPose", "Type" + name, "" + handPoseType);
        }
        public enum HandPoseType {
            Open,
            Grabbing,
            Pointing,
            GunLoaded,
            GunFired,
            Peace,
            ThumbsUp,
            Rude,
            Loser
        }

        private readonly Dictionary<int, HandPoseType> poseMap = new Dictionary<int, HandPoseType>
        {
            [0b00000] = HandPoseType.Open,
            [0b11111] = HandPoseType.Grabbing,
            [0b10111] = HandPoseType.Pointing,
            [0b10011] = HandPoseType.Peace,
            [0b01111] = HandPoseType.ThumbsUp,
            [0b11011] = HandPoseType.Rude,
            [0b00111] = HandPoseType.Loser
        };

        private int FingerArrayToBitmask(List<bool> fingers)
        {
            int mask = 0;
            for (int i = 0; i < 5; i++)
            {
                if (fingers[i])
                {
                    mask |= (1 << (4 - i));
                }
            }
            return mask;
        }
    }

}