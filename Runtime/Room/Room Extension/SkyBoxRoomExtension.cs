using UnityEngine;

namespace Muco {
    public class SkyBoxRoomExtension : RoomExtension {
        public Color skyBoxColor = new Color(0, 0, 0);
        public Material skyBox;
        public Color ambientLightColor;

        public override void Init() {
            RenderSettings.ambientLight = ambientLightColor;

            Camera.main.backgroundColor = skyBoxColor;
            if (skyBox != null) {
                RenderSettings.skybox = skyBox;
                Camera.main.clearFlags = CameraClearFlags.Skybox;
                DynamicGI.UpdateEnvironment();
            }
            else {
                Camera.main.backgroundColor = skyBoxColor;
                Camera.main.clearFlags = CameraClearFlags.SolidColor;
            }
        }
    }
}
