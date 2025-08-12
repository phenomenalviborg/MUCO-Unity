using UnityEngine;

namespace Muco {
    public class DeviceBatteryInfo : MonoBehaviour {
        public static float batteryLevel;
        public static BatteryStatus batteryStatus;

        void Update() {
            if (SystemInfo.batteryStatus != batteryStatus) {
                batteryStatus = SystemInfo.batteryStatus;
                VrDebug.SetValue("DeviceInfo", "Battery Status",batteryStatus.ToString());
            }
            if (SystemInfo.batteryLevel != batteryLevel) {
                batteryLevel = SystemInfo.batteryLevel;
                VrDebug.SetValue("DeviceInfo", "Battery Level", (batteryLevel*100).ToString());
            }
        }
    }
}
