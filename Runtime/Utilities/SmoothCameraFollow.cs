using UnityEngine;

namespace Muco
{
    public class SmoothCameraFollow : MonoBehaviour
    {
        // camera will follow this object
        public Transform Target;
        //camera transform
        // offset between camera and target
        //public Vector3 Offset;
        // change this value to get desired smoothness
        public float smoothTime = 0.3f;
        public float rotationSpeed = 1;

        public Networking networking;

        // This value will change at the runtime depending on target movement. Initialize with zero vector.
        private Vector3 velocity = Vector3.zero;

        public PlayerRemoteIndicatorBase[] playerIndicatorSwappers;

        public int selectedGhost;

        public Transform mainCamera;

        void Start() {
            mainCamera.transform.SetParent(this.transform);
        }

        private void LateUpdate()
        {
            // update position
            Debug.Log(networking.ghostSystem.ghosts.Keys.Count);
            var selected = networking.ghostSystem.ghosts[selectedGhost];
            if (selected != null)
            {
                Target = selected.playerIndicator.transform;
                selected.playerIndicator.transform.GetChild(0).gameObject.SetActive(false);
            }
            
            this.transform.position = Vector3.SmoothDamp(this.transform.position, Target.position, ref velocity, smoothTime);
            //camTransform.rotation = Quaternion(Vector3.SmoothDamp(transform.rotation, Target.rotation, ref velocity, SmoothTime));
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Target.rotation, SmoothTime);

            // transform.rotation = Quaternion.RotateTowards(transform.rotation, Target.rotation, SmoothTime * Time.deltaTime);
            // update rotation
            //transform.LookAt(Target);

            //this.transform.rotation = Quaternion.Euler(Vector3.SmoothDamp(this.transform.rotation.eulerAngles, Target.rotation.eulerAngles, ref velocity, SmoothTime));

            //this.transform.position = Vector3.SmoothDamp(this.transform.position, player.transform.position, ref velocity, smoothTime);
            this.transform.rotation = Quaternion.Lerp(transform.rotation, Target.transform.rotation, Time.deltaTime * rotationSpeed);
        }
    }
}
