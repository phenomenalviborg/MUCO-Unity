using UnityEngine;

namespace Muco {
    public class SmoothFollow : MonoBehaviour {
        public float lerpWeightPosition = 20f;
        public float lerpWeightRotation = 0.025f;
        Vector3 velocity = Vector3.zero;
        Transform target;
        public bool smoothRotation;

        //public float lockY = 1.7f;
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
            //transform.position.Set(transform.position.x,lockY,transform.position.z);
            //velocity.y = 0;
            transform.rotation = smoothRotation ? Quaternion.Lerp(transform.rotation, target.rotation, lerpWeightRotation) :  target.rotation;
            //transform.position = Vector3.Lerp(transform.position, target.position, lerpWeightPosition);
            transform.position = Vector3.SmoothDamp(transform.position,target.position,ref velocity,1/lerpWeightPosition);
            //transform.localPosition.Set(transform.localPosition.x,lockY, transform.localPosition.z);
        }
    }
}
