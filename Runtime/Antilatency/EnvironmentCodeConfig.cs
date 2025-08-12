using UnityEngine;
using Antilatency.Alt.Environment;

namespace Muco {
    public class EnvironmentCodeConfig : Antilatency.SDK.AltEnvironmentComponent {
        private IEnvironment _environment = null;
        public string fallbackCode;

        public void Reload() {
            _environment = null;
        }

        public override IEnvironment GetEnvironment() {
            if (_environment != null) {
                return _environment;
            }

            using (var selectorLibrary = Antilatency.Alt.Environment.Selector.Library.load()) {
                if (selectorLibrary == null) {
                    Debug.LogError("Failed to Alt Environment Selector library");
                    return null;
                }

                var environmentData = Networking.TheNetworking.GetEnvironmentData();
                var environmentCode = environmentData.code;
                transform.position = environmentData.pos;
                transform.rotation = Quaternion.Euler(environmentData.euler);
                VrDebug.SetValue("EnvCode", "code", environmentCode);

                if (environmentCode == null) {
                    Debug.Log("Environment Code Not Set using fallback code");
                    environmentCode = fallbackCode;
                }

                try {
                    _environment = selectorLibrary.createEnvironment(environmentCode);
                    VrDebug.SetValue("EnvCode", "could create env", "true");
                }
                catch {
                    _environment = selectorLibrary.createEnvironment("AntilatencyAltEnvironmentHorizontalGrid~AgACBLhTiT_cRqA-r45jvZqZmT4AAAAAAAAAAACamRk_AQEAAgM");
                    Debug.Log("Problem with environment code: " + enabled);
                    VrDebug.SetValue("EnvCode", "could create env", "false");
                }

                if (_environment == null) {
                    Debug.LogError("Failed to create environment");
                    return null;
                }

                return _environment;
            }
        }
    }
}
