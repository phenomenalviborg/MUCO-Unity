using UnityEngine;

namespace Muco {
    public enum Platform {
        WindowsEditor,
        OsxEditor,
        Quest2,
        Pico4,
        Other,
        OtherAndroid,
    }

    public static class PlatformExtensions {
        public static bool IsHeadset(this Platform platform) {
            return platform switch
            {
                Platform.Quest2 or Platform.Pico4 => true,
                _ => false,
            };
        }

        public static bool IsAndroid(this Platform platform) {
            return platform switch
            {
                Platform.Quest2 or Platform.Pico4 or Platform.OtherAndroid => true,
                _ => false,
            };
        }

        public static bool IsEditor(this Platform platform) {
            return platform switch
            {
                Platform.WindowsEditor or Platform.OsxEditor => true,
                _ => false,
            };
        }
    }

    [System.Serializable]
    public struct PlatformCriteria {
        public bool developer;
        public bool release;
        public bool windowsEditor;
        public bool osxEditor;
        public bool quest2;
        public bool pico4;
    }

    public class PlatformDetection {
        static Platform? _platform;
        public static Platform ThePlatform => GetCashedPlatform();

        public static Platform GetCashedPlatform() {
            if (_platform is Platform platform)
                return platform;
            var detectedPlatform = DetectPlatform();
            _platform = detectedPlatform;
            return detectedPlatform;
        }

        public static Platform DetectPlatform() {
            switch (Application.platform) {
                case RuntimePlatform.Android:
                    var deviceModel = SystemInfo.deviceModel;
                    var deviceName = SystemInfo.deviceName;
                    if (deviceName == "Quest 2")
                        return Platform.Quest2;
                    if (deviceModel == "Pico A94Y0")
                        return Platform.Pico4;
                    if (deviceModel == "Pico A8250")
                        return Platform.Pico4;
                    return Platform.OtherAndroid;
                case RuntimePlatform.WindowsEditor:
                    return Platform.WindowsEditor;
                case RuntimePlatform.OSXEditor:
                    return Platform.OsxEditor;
                default:
                    return Platform.Other;
            }
        }

        public static bool PlatformCriteriaSatisfied(PlatformCriteria criteria) {
            var developerMode = DeveloperMode.TheDeveloperMode;
            var platformIsMet = ThePlatform switch
            {
                Platform.WindowsEditor => criteria.windowsEditor,
                Platform.OsxEditor => criteria.osxEditor,
                Platform.Quest2 => criteria.quest2,
                Platform.Pico4 => criteria.pico4,
                _ => false,
            };
            bool developerModeIsMet;
            if (developerMode.isOn) {
                developerModeIsMet = criteria.developer;
            }
            else {
                developerModeIsMet = criteria.release;
            }
            return platformIsMet & developerModeIsMet;
        }
    }
}
