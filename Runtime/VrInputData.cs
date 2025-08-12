using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Muco {
    public class VrInputData : MonoBehaviour {
        static VrInputData _vrInputData;
        public static VrInputData TheVrInputData {
            get {
                if (_vrInputData != null)
                    return _vrInputData;
                var vrInputData = FindFirstObjectByType<VrInputData>();
                if (vrInputData == null)
                    Debug.Log("Could not find VrInputData");
                if (!vrInputData.isInitialized)
                    vrInputData.Init();
                _vrInputData = vrInputData;
                return vrInputData;
            }

        }
        public enum DeviceType {
            Head,
            LCtrl,
            RCtrl,
        }

        private InputDevice[] devices;
        private InputDeviceCharacteristics[] inputDeviceCharacteristics;
        bool isInitialized;

        void Init() {
            devices = new InputDevice[3];
            inputDeviceCharacteristics = new InputDeviceCharacteristics[] {
                InputDeviceCharacteristics.HeadMounted,
                InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left,
                InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right,
            };
            isInitialized = true;
        }

        void Awake() {
            if (!isInitialized)
                Init();
        }

        void Update() {
            for (int i = 0; i < devices.Length; i++)
            {
                if (!devices[i].isValid)
                {
                    if (GetDeviceWithCharacteristics(inputDeviceCharacteristics[i]) is InputDevice device)
                        devices[i] = device;
                }
            }
        }

        public InputDevice GetDevice(DeviceType deviceType) {
            return devices[(int)deviceType];
        }

        public InputDevice head => GetDevice(DeviceType.Head);
        public InputDevice lCtrl => GetDevice(DeviceType.LCtrl);
        public InputDevice rCtrl => GetDevice(DeviceType.RCtrl);

        InputDevice? GetDeviceWithCharacteristics(InputDeviceCharacteristics characteristics)
        {
            List<InputDevice> headDevices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(characteristics, headDevices);
            if (headDevices.Count == 0)
                return null;
            return headDevices[0];
        }
    }
}
