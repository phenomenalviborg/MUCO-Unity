using UnityEngine;

namespace Muco {
    public class Player : MonoBehaviour {
        static Player _player;
        public static Player ThePlayer {
            get {
                if (_player != null)
                    return _player;
                var player = FindFirstObjectByType<Player>();
                if (player == null) {
                    Debug.Log("No player found");
                }
                if (!player.isInitialized)
                    player.Init();
                _player = player;
                return player;
            }
        }
        public int currentLevelIndex;
        public Language language;
        public GameObject guardian;
        public Transform head;
        [HideInInspector] public Camera cam;
        public HandType handType;
        public NetworkTransformList handL;
        public NetworkTransformList handR;
        public Transform handParentL;
        public Transform handParentR;
        public HandTrackingConfidenceProvider handTrackingConfidenceL;
        public HandTrackingConfidenceProvider handTrackingConfidenceR;

        public SkinnedMeshRenderer handLMeshRenderer;
        public SkinnedMeshRenderer handRMeshRenderer;
        [HideInInspector] public CustomAltTrackingXr custonAltTrackingXr;
        [HideInInspector] public EnvironmentCodeConfig environmentCodeConfig;
        [HideInInspector] bool isInitialized;

        void Awake() {
            if (!isInitialized)
                Init();
        }

        protected virtual void Init() {
            if (isInitialized)
                return;
                
            cam = Camera.main ?? throw new System.Exception("Main camera is missing in the scene.");

            VrDebug.SetValue("Platform", "Platform", "" + PlatformDetection.ThePlatform);
            VrDebug.SetValue("Platform", "DeviceName", "" + SystemInfo.deviceName);
            VrDebug.SetValue("Platform", "DeviceModel", "" + SystemInfo.deviceModel);

            isInitialized = true;
        }
    }
}
