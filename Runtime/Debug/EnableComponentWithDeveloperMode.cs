using UnityEngine;

namespace Muco {
    public class EnableComponentWithDeveloperMode : MonoBehaviour {
        public Behaviour[] components;

        public Behaviour[] componentsOnlyAndroid;
        
        void Start() {
            DeveloperMode.TheDeveloperMode.Register(DeveloperModeCallback);
        }

        public void DeveloperModeCallback(bool isOn) {
            foreach(Behaviour component in components) {
                component.enabled = isOn;
            }
            foreach(Behaviour component in componentsOnlyAndroid) {
                if (Application.platform == RuntimePlatform.Android)
                    component.enabled = isOn;
            }
        }
    }
}