using System;
using System.IO;
using UnityEngine;

namespace Muco {

    public struct EnvData {
        public string name;
        public string code;
        public Vector3 pos;
        public Vector3 euler;
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
                    Save();
                }
            }
            else {
                string configText = File.ReadAllText(path);
                this = JsonUtility.FromJson<Config>(configText);
                VrDebug.SetValue("Config", "config source", "file");
            }

            VrDebug.SetValue("Config", "config path", path);
        }
    }
}
