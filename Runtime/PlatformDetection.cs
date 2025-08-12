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
            switch (platform) {
                case Platform.Quest2:
                case Platform.Pico4:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsAndroid(this Platform platform) {
            switch (platform) {
                case Platform.Quest2:
                case Platform.Pico4:
                case Platform.OtherAndroid:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsEditor(this Platform platform) {
            switch (platform) {
                case Platform.WindowsEditor:
                case Platform.OsxEditor:
                    return true;
                default:
                    return false;
            }
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
            bool platformIsMet;
            switch (ThePlatform) {
                case Platform.WindowsEditor:
                    platformIsMet = criteria.windowsEditor;
                    break;
                case Platform.OsxEditor:
                    platformIsMet = criteria.osxEditor;
                    break;
                case Platform.Quest2:
                    platformIsMet = criteria.quest2;
                    break;
                case Platform.Pico4:
                    platformIsMet = criteria.pico4;
                    break;
                default:
                    platformIsMet = false;
                    break;
            }
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
