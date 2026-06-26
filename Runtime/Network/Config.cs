using System;
using System.IO;
using UnityEngine;

namespace Muco {

    public enum GuardianType {
        Rectangle = 1,
        // Future: Circle = 2, Polygon = 3
    }

    [Serializable]
    public struct GuardianConfig {
        public GuardianType type;
        public float width;
        public float height;

        public static GuardianConfig Default() {
            return new GuardianConfig {
                type = GuardianType.Rectangle,
                width = 9.0f,
                height = 18.0f
            };
        }
    }

    [Serializable]
    public struct EnvData {
        public string name;
        public string code;
        public Vector3 pos;
        public Vector3 euler;
        public GuardianConfig guardian;
    }

    [Serializable]
    public struct Config {
        public EnvData environmentData;
        public Color color;
        
        static string GetPath() {
            string fname = "config.json";
            string path = Path.Combine(Application.persistentDataPath, fname);
            return path;
        }

        public void Save() {
            var path = GetPath();
            string configText = JsonUtility.ToJson(this, true);
            File.WriteAllText(path, configText);
            VrDebug.SetValue("Config", "config source", "game");
        }

        public void TryLoad(bool makeNewIfNotExist) {
            var path = GetPath();

            if (!File.Exists(path)) {
                if (makeNewIfNotExist) {
                    environmentData.guardian = GuardianConfig.Default();
                    Save();
                }
            }
            else {
                string configText = File.ReadAllText(path);
                this = JsonUtility.FromJson<Config>(configText);

                // Ensure guardian config exists (backward compatibility)
                if (environmentData.guardian.width == 0 && environmentData.guardian.height == 0) {
                    environmentData.guardian = GuardianConfig.Default();
                }

                VrDebug.SetValue("Config", "config source", "file");
            }

            VrDebug.SetValue("Config", "config path", path);
        }
    }
}
