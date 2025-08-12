using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Muco
{
    public class VrDebug : MonoBehaviour
    {
        public Player player;
        public Text debugValueText;
        public Text debugLogText;

        public Text hallwayDebugValueText;
        public static Dictionary<string, Dictionary<string, string>> TheValues;
        public string currentCategory;
        public int maxLogLength;

        public static VrDebug TheMucoVrDebug;

        public bool nextCategoryButtonWasPressed;
        public bool prevCategoryButtonWasPressed;

        [System.Serializable]
        public struct LogEntry
        {
            public string logString;
            public string stackTrace;
            public LogType type;
        }

        public List<LogEntry> log;

        void Awake()
        {
            TheMucoVrDebug = this;
            UpdateText();
            DeveloperMode.TheDeveloperMode.Register(DeveloperModeCallback);
        }

        private void Update() {
            // Debug.Log("Don't foget to fix Vr Debug");
            // bool nextCategoryButtonIsPressed = OVRInput.Get(OVRInput.Button.Two)
            //                                 || OVRInput.Get(OVRInput.Button.Four)
            //                                 || Input.GetKey(KeyCode.Period);
            // bool prevCategoryButtonIsPressed = OVRInput.Get(OVRInput.Button.One)
            //                                 || OVRInput.Get(OVRInput.Button.Three)
            //                                 || Input.GetKey(KeyCode.Comma);

            // bool nextCategoryButtonDown = nextCategoryButtonIsPressed && !nextCategoryButtonWasPressed;
            // bool prevCategoryButtonDown = prevCategoryButtonIsPressed && !prevCategoryButtonWasPressed;

            // nextCategoryButtonWasPressed = nextCategoryButtonIsPressed;
            // prevCategoryButtonWasPressed = prevCategoryButtonIsPressed;

            // if (nextCategoryButtonDown)
            //     CycleCategory(1);

            // if (prevCategoryButtonDown)
            //     CycleCategory(-1);
        }

        public void CycleCategory(int i)
        {
            if (TheValues == null)
                return;

            var keys = new List<string>();

            foreach (var key in TheValues.Keys)
                keys.Add(key);

            var index = keys.FindIndex(x => x == currentCategory);

            if (index == -1)
                index = 0;

            index += i;
            index += keys.Count;
            index = index % keys.Count;

            currentCategory = keys[index];
        }

        public void DeveloperModeCallback(bool isOn)
        {
            debugValueText.gameObject.SetActive(isOn);
            debugLogText.gameObject.SetActive(isOn);
        }

        public static void SetValue(string category, string key, string value)
        {
            if (TheValues == null)
                TheValues = new Dictionary<string, Dictionary<string, string>>();

            if (!TheValues.ContainsKey(category))
                TheValues.Add(category, new Dictionary<string, string>());

            var dict = TheValues[category];
            dict[key] = value;

            if (TheMucoVrDebug != null)
                TheMucoVrDebug.UpdateText();
        }

        public static void ClearCaterogy(string category)
        {
            if (TheValues == null)
                return;

            if (!TheValues.ContainsKey(category))
                return;

            TheValues[category].Clear();
        }

        void UpdateText()
        {
            if (TheValues == null)
                return;

            debugValueText.text = "";

            if (currentCategory == "")
            {
                foreach (var category in TheValues.Keys)
                {
                    WriteCategoryValues(category);
                    debugValueText.text += "\n";
                }
            }
            else
                WriteCategoryValues(currentCategory);
        }

        void WriteCategoryValues(string category)
        {
            if (TheValues == null)
                return;

            if (!TheValues.ContainsKey(category))
                return;

            var dict = TheValues[category];
            if (dict == null)
                return;

            var s = category + ":\n";
            foreach (var key in dict.Keys)
            {
                s += key;
                s += ": ";
                s += dict[key];
                s += "\n";
            }

            debugValueText.text += s;

            if (hallwayDebugValueText != null)
                hallwayDebugValueText.text = debugValueText.text;
        }

        void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Warning)
                return;

            log.Add(new LogEntry { logString = logString, stackTrace = stackTrace, type = type });

            if (log.Count > maxLogLength)
                log.RemoveAt(0);

            UpdateLogText();
        }

        void UpdateLogText()
        {
            debugLogText.text = "";

            foreach (var entry in log)
            {
                debugLogText.text += entry.logString;
                debugLogText.text += "\n";

                if (entry.type == LogType.Exception || entry.type == LogType.Error)
                {
                    debugLogText.text += entry.stackTrace;
                    debugLogText.text += "\n";
                }
            }


        }
    }
}
