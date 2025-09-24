using Antilatency.SDK;
using UnityEngine;

namespace Muco {
    public class SetupAntilatency : MonoBehaviour {
        public Player player;
        public GameObject vrInput;

        public Camera mainCamera;
        public UnityEngine.SpatialTracking.TrackedPoseDriver trackedPose;
        public CustomAltTrackingXr.PlacementPreset placementPreset;

        public PlatformCriteria platformCriteria;
        public bool debugTrackingConfidence;

        CustomAltTrackingXr altTracking;

        void Start() {
            if(PlatformDetection.PlatformCriteriaSatisfied(platformCriteria))
                Setup();
        }

        void Update()
        {
            if (debugTrackingConfidence && altTracking != null)
            {
                if (altTracking.TryGetTrackingConfidence(out float confidence))
                    VrDebug.SetValue("Stats", "TrackingConfidence", "" + confidence);
                else
                    VrDebug.SetValue("Stats", "TrackingConfidence", "None");
            }
        }

        void Setup() {
            player.gameObject.SetActive(false);
            vrInput.SetActive(false);

            // Device Network
            var network = player.gameObject.AddComponent<DeviceNetwork>();

            // Environment Code Config
            var environment = player.gameObject.AddComponent<EnvironmentCodeConfig>();
            environment.fallbackCode = "AntilatencyAltEnvironmentHorizontalGrid~AgACBLhTiT_cRqA-r45jvZqZmT4AAAAAAAAAAACamRk_AQEAAgM";

            // Camera Offset
            // vrInput.AddComponent<CameraOffset>();

            // Custom Alt Traking Xr
            altTracking = vrInput.AddComponent<CustomAltTrackingXr>();
            altTracking.Network = network;
            altTracking.Environment = environment;
            altTracking.XrCamera = mainCamera;
            altTracking.HmdPoseDriver = trackedPose;
            altTracking.placementPreset = placementPreset;
            altTracking.MinimalAQualityToAlign = 0;
            altTracking.ExtrapolationTime = 0.03f;

            // Alt Environment Markers DRawer
            var markerDrawer = player.gameObject.AddComponent<AltEnvironmentMarkersDrawer>();
            markerDrawer.Environment = environment;
            markerDrawer.enabled = DeveloperMode.TheDeveloperMode.isOn;

            player.custonAltTrackingXr = altTracking;
            player.environmentCodeConfig = environment;
            
            player.gameObject.SetActive(true);
            vrInput.SetActive(true);
        }

        public void OnDeveloperMode(bool isOn) {
            var markerDrawer = GetComponent<AltEnvironmentMarkersDrawer>();
            if (markerDrawer)
                markerDrawer.enabled = isOn;
        }
    }
}