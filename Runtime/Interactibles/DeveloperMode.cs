using System.Collections.Generic;
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

        public void Register(DeveloperModeCallback callback) {
            callback(isOn);
            callbacks ??= new List<DeveloperModeCallback>();
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
            VrInputData.TheVrInputData.LCtrl.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool leftTriggerIsPressed);
            VrInputData.TheVrInputData.RCtrl.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool rightTriggerIsPressed);
            bool toggleButtonIsPressed = leftTriggerIsPressed || rightTriggerIsPressed || Keyboard.current.lKey.wasPressedThisFrame;

            var toggleButtonDown = toggleButtonIsPressed && !toggleButtonWasPressed;
            toggleButtonWasPressed = toggleButtonIsPressed;

            if (toggleButtonDown)
                Toggle();
        }
    }
}
