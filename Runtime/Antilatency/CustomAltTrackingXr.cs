using UnityEngine;

namespace Muco
{
    public class CustomAltTrackingXr : AltTrackingXr {
        public enum PlacementPreset {
            FromService,
            Identity,
            Quest2,
            Quest2Printed,
            Pico4TopPrinted,
            Pico4BottomPrinted,
        }

        public PlacementPreset placementPreset;

        protected override Pose GetPlacement() {
            switch (placementPreset) {
                case PlacementPreset.FromService: {
                    return base.GetPlacement();
                }
                case PlacementPreset.Identity: {
                    return Pose.identity;
                }
                case PlacementPreset.Quest2: {
                    return new Pose {
                        position = new Vector3(0f, -3.65f, 8.07f) * 0.01f,
                        rotation = Quaternion.Euler(9.5f, 0f, 90f),
                    };
                }
                case PlacementPreset.Quest2Printed: {
                    return new Pose {
                        position = new Vector3(0f, 0f, 8.7f) * 0.01f,
                        rotation = Quaternion.Euler(0f, 0f, 90f),
                    };
                }
                case PlacementPreset.Pico4TopPrinted: {
                    return new Pose {
                        position = new Vector3(0.5f, 4.55f, 3.2f) * 0.01f,
                        rotation = Quaternion.Euler(0f, 0f, -70f),
                    };
                }
                case PlacementPreset.Pico4BottomPrinted: {
                    return new Pose {
                        position = new Vector3(0.5f, -4.27f, 5.5f) * 0.01f,
                        rotation = Quaternion.Euler(45f, 0f, -70f),
                    };
                }
            }
            return Pose.identity;
        }

        void Update()
        {
            GetTrackingConfidence();
        }

        public float GetTrackingConfidence() {
            float confidence = 0f;
            Antilatency.Alt.Tracking.State state;
            if (GetRawTrackingState(out state)) {
                confidence = state.stability.value;
            }
            VrDebug.SetValue("Stats", "TrackingConfidence", "" + confidence);
            return confidence;
        }
    }
}
