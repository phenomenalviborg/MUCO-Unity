using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Muco {
    public class IngameCamera : RoomExtension {
        public RenderTexture renderTexture;
        public Texture2D image;
        public string referenceScreenShotURL = "localhost:3030/upload_photo";
        string screenShotURL;
        public ResolveIp resolveIp;

        void Update() {
            if (screenShotURL != null)
                return;
            var result = resolveIp.Poll();
            if (result == null)
                return;
            screenShotURL = "http://" + result.ip + ":" + result.port + "/upload_photo";
            Debug.Log(screenShotURL);
        }

        public void RTImage(Camera camera) {
            var currentRT = RenderTexture.active;

            RenderTexture.active = renderTexture;
            camera.targetTexture = renderTexture;
            camera.Render();

            image = new Texture2D(renderTexture.width, renderTexture.height);
            image.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            image.Apply();

            RenderTexture.active = currentRT;
            camera.targetTexture = currentRT;
        }

        public void MakePhoto() {
            RTImage(Player.ThePlayer.cam);
            var date_time_string = System.DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss");
            var device_id = Networking.TheNetworking.GetUniqueDeviceInt().ToString();
            var name = device_id + "_" + date_time_string;
            Debug.Log(name);
            StartCoroutine(UploadPNG(name, image));
        }

        IEnumerator UploadPNG(string name, Texture2D tex) {
            byte[] bytes = tex.EncodeToPNG();

            WWWForm form = new WWWForm();
            form.AddBinaryData(name, bytes);

            using (var w = UnityWebRequest.Post(screenShotURL, form)) {
                yield return w.SendWebRequest();
                if (w.result != UnityWebRequest.Result.Success) {
                    print(w.error);
                }
                else {
                    print("Finished Uploading Screenshot");
                }
            }
        }
    }
}
