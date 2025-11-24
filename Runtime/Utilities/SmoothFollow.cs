using UnityEngine;

namespace Muco {
    public class SmoothFollow : MonoBehaviour {
        public float lerpWeightPosition = 20f;
        public float lerpWeightRotation = 0.025f;
        Vector3 velocity = Vector3.zero;
        Transform target;
        public bool smoothRotation;

        void Start()
        {
            if (transform.parent != null)
            {
                target = transform.parent;
            }
            transform.parent = null;
            if (lerpWeightPosition == 0) Debug.LogWarning("SmoothFollow Lerp Weight is at 0. It won't move");
        }

        void Update() {
            if (target == null) {
                Destroy(gameObject);
                return;
            }
            transform.rotation = smoothRotation ? Quaternion.Slerp(transform.rotation, target.rotation, lerpWeightRotation) :  target.rotation;
            transform.position = Vector3.SmoothDamp(transform.position,target.position,ref velocity,1/lerpWeightPosition);
        }
    }
}
