using Muco;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

namespace Muco {
    public class DeviceTemperature : MonoBehaviour
    {
        IAdaptivePerformance ap;
        public ThermalMetrics metrics;

        void Start()
        {
            ap = Holder.Instance;
        }

        void Update()
        {
            if (ap == null)
                return;

            if (!ap.Active)
                return;

            metrics = ap.ThermalStatus.ThermalMetrics;
            VrDebug.SetValue("Stats", "TemperatureWarningLevel", "" + metrics.WarningLevel);
            VrDebug.SetValue("Stats", "TemperatureLevel", "" + metrics.TemperatureLevel);
            VrDebug.SetValue("Stats", "TemperatureTrend", "" + metrics.TemperatureTrend);
        }
    }
}
