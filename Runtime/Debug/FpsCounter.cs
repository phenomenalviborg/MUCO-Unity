using UnityEngine;

namespace Muco {
    public class FpsCounter : MonoBehaviour {
        public float lastFrameTime;
        public float deltaTime;
        public float fps;
        void Update() {
            var framTime = Time.time;
            deltaTime = framTime - lastFrameTime;
            fps = 1f / deltaTime;
            lastFrameTime = framTime;
            VrDebug.SetValue("Stats", "Fps", "" + fps);
        }
    }
}
