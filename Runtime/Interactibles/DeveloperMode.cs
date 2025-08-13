using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Muco {
    public class DeveloperMode : MonoBehaviour {
        static DeveloperMode _developerMode;
        public static DeveloperMode TheDeveloperMode {
            get {
                if (_developerMode != null)
                    return _developerMode;
                var developerMode = FindFirstObjectByType<DeveloperMode>();
                if (developerMode == null)
                    Debug.Log("Could not find DeveloperMode");
                _developerMode = developerMode;
                return developerMode;
            }
        }

        public bool isOn;

        public delegate void DeveloperModeCallback(bool isOn);

        List<DeveloperModeCallback> callbacks;

        bool toggleButtonWasPressed;
        public VrInputData vrInputData;

        public void Register(DeveloperModeCallback callback) {
            if (callbacks == null)
                callbacks = new List<DeveloperModeCallback>();

            callback(isOn);

            callbacks.Add(callback);
        }

        public void UnRegister(DeveloperModeCallback callback) {
            callbacks.Remove(callback);
        }

        public void Toggle() {
            Set(!isOn);
        }
        
        public void Set(bool isOn) {
            this.isOn = isOn;
            if (callbacks != null) {
                foreach (var callback in callbacks)
                    callback(isOn);
            }
        }

        void Update() {
            vrInputData.lCtrl.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool leftTriggerIsPressed);
            vrInputData.rCtrl.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool rightTriggerIsPressed);
            bool toggleButtonIsPressed = leftTriggerIsPressed || rightTriggerIsPressed || Keyboard.current.lKey.wasPressedThisFrame;

            var toggleButtonDown = toggleButtonIsPressed && !toggleButtonWasPressed;
            toggleButtonWasPressed = toggleButtonIsPressed;

            if (toggleButtonDown)
                Toggle();
        }
    }
}
