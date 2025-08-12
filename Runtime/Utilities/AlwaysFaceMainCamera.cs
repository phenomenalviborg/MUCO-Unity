using UnityEngine;

namespace Muco {
    [ExecuteInEditMode]
    public class AlwaysFaceMainCamera : MonoBehaviour {
       void LateUpdate() {
            Camera cam = Camera.main;
            if (cam == null && Camera.current != null) {
                cam = Camera.current;
            }

            if (cam != null) {
                Vector3 direction = transform.position - cam.transform.position;
                Quaternion rotation = direction == Vector3.zero
                                    ? Quaternion.identity
                                    : Quaternion.LookRotation(direction);
                transform.rotation = rotation;

                transform.Rotate(0, 180, 0);
            }
        }
    }
}
