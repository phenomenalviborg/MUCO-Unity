using System.Collections;
using UnityEngine;
using UnityEngine.XR;

namespace Muco
{
    public class CustomAltTrackingXr : AltTrackingXr {
        [Tooltip("Automatically recreate the tracking alignment once valid Alt + HMD data are available, removing the need for a manual pause/unpause cycle.")]
        public bool autoRecalibrate = true;

        public enum PlacementPreset {
            FromService,
            Identity,
            Quest2,
            Quest2Printed,
            Pico4TopPrinted,
            Pico4BottomPrinted,
        }

        public PlacementPreset placementPreset;

        private bool _autoRecalibrated;

        protected override void Awake() {
            base.Awake();
            _autoRecalibrated = false;
            if (autoRecalibrate) {
                StartCoroutine(AutoRecalibrateRoutine());
            }
        }

        private IEnumerator AutoRecalibrateRoutine() {
            var wait = new WaitForSecondsRealtime(0.3f);
            while (!_autoRecalibrated) {
                if (ShouldAutoRecalibrate()) {
                    OnFocusChanged(false);
                    OnFocusChanged(true);
                    _autoRecalibrated = true;
                    yield break;
                }
                yield return wait;
            }
        }

        private bool ShouldAutoRecalibrate() {
            if (!TryGetTrackingConfidence(out float confidence) || confidence < MinimalAQualityToAlign) {
                return false;
            }

            var centerEye = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
            if (!centerEye.isValid) {
                return false;
            }

            if (!centerEye.TryGetFeatureValue(CommonUsages.userPresence, out bool userPresence) || !userPresence) {
                return false;
            }

            if (!centerEye.TryGetFeatureValue(CommonUsages.trackingState, out InputTrackingState state)) {
                return false;
            }

            return state.HasFlag(InputTrackingState.Rotation);
        }

        public void ForceRecalibrate() {
            OnFocusChanged(false);
            OnFocusChanged(true);
            _autoRecalibrated = true;
        }

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

        public bool TryGetTrackingConfidence(out float confidence)
        {
            confidence = 0;
            Antilatency.Alt.Tracking.State state;
            if (GetRawTrackingState(out state))
            {
                confidence = state.stability.value;
                return true;
            }
            return false;
        }
    }
}
