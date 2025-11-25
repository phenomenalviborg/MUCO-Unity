using UnityEngine;
using UnityEngine.InputSystem;

namespace Muco {
    public class InteractibleSystemPC : MonoBehaviour {
        [System.Serializable]
        public enum InteractionMode {
            Default,
            Interacting,
        }


        public DeveloperMode developerMode;

        byte system_id = 0;

        public Interactible currentInteractible;
        public InteractionMode interactionMode;

        public Vector2 lastMousePosition;
        public Vector3 localPickUpPosition;
        public Quaternion localPickUpRotation;
        public float pickUpDistance;
        public float defaultMoveSpeed = 0.02f;
        [Tooltip("Hold shift for sprint")]
        public float sprintMoveSpeed = 0.05f;

        void Update() {
            var interactorId = new InteractorId(Networking.TheNetworking.serverConnection.clientId, system_id, 0);
            switch (interactionMode) {
                case InteractionMode.Default:
                    UpdateDefault(interactorId);
                    break;
                case InteractionMode.Interacting:
                    UpdateInteracting(interactorId);
                    break;
            }

            lastMousePosition = Mouse.current?.position.value ?? Vector2.zero;
        }

        void UpdateDefault(InteractorId interactorId) {
            if (Mouse.current?.leftButton.wasPressedThisFrame ?? false) {
                var cam = Player.ThePlayer.cam;
                Ray ray = cam.ScreenPointToRay(Mouse.current?.position.value ?? Vector2.zero);

                Interactible interactible = null;
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo)) {
                    GetInteractibleInGameObjectOrParent(hitInfo.collider.gameObject, out interactible);
                }

                if (interactible != null) {
                    currentInteractible = interactible;
                    interactionMode = InteractionMode.Interacting;

                    interactible.TryTakeOwnership(interactorId);

                    var button = interactible.GetComponent<InteractibleComponentButton>();
                    if (button) {
                        button.UpdateValueAndSend(button.fireValue, true, true, new Vector2(hitInfo.transform.position.x-hitInfo.point.x, hitInfo.transform.position.z-hitInfo.point.z));
                    }

                    var slider = interactible.GetComponent<InteractibleComponentSlider>();
                    if (slider) {
                        SetSliderFromMousePosition(slider, false, true);
                    }

                    var movable = interactible.GetComponent<InteractibleComponentPickUp>();
                    if (movable) {
                        pickUpDistance = hitInfo.distance;
                        localPickUpRotation = Quaternion.Inverse(cam.transform.rotation) * hitInfo.transform.rotation;
                        localPickUpPosition = Quaternion.Inverse(cam.transform.rotation) * (hitInfo.transform.position - hitInfo.point);
                    }
                }
            }

            float moveSpeed =defaultMoveSpeed;
            if (Keyboard.current.leftShiftKey.isPressed)
                moveSpeed = sprintMoveSpeed;

            if (Keyboard.current.wKey.isPressed)
                DragWorld.TheDragWorld.MoveFPS(Vector3.forward * moveSpeed);
            if (Keyboard.current.sKey.isPressed)
                DragWorld.TheDragWorld.MoveFPS(Vector3.back * moveSpeed);
            if (Keyboard.current.aKey.isPressed)
                DragWorld.TheDragWorld.MoveFPS(Vector3.left * moveSpeed);
            if (Keyboard.current.dKey.isPressed)
                DragWorld.TheDragWorld.MoveFPS(Vector3.right * moveSpeed);
            if (Keyboard.current.qKey.isPressed)
                DragWorld.TheDragWorld.MoveFPS(Vector3.down * moveSpeed);
            if (Keyboard.current.eKey.isPressed)
                DragWorld.TheDragWorld.MoveFPS(Vector3.up * moveSpeed);

            if (Mouse.current?.rightButton.isPressed ?? false) {
                var mouseDelta = (Mouse.current?.position.value ?? Vector2.zero) - lastMousePosition;
                float mouseSensitivity = 0.5f;
                Vector3 m = mouseDelta * mouseSensitivity;
                DragWorld.TheDragWorld.RotateFps(-m.y, m.x);
            }
        }

        SnapPoint FindClosestSnappingPointScreenSpace(Vector3 screenPos, float maxDist, InteractibleComponentPickUp pickUp) {
            var cam = Player.ThePlayer.cam;
            var snappingPoints = FindObjectsByType<SnapPoint>(0);
            SnapPoint closestSnappingPoint = null;
            float closesedDist = maxDist;
            foreach(var snappingPoint in snappingPoints) {
                if (!snappingPoint.CanSnapToMe(pickUp))
                    continue;
                var snapScreenPoint = cam.WorldToScreenPoint(snappingPoint.transform.position);
                var dist = Vector3.Distance(screenPos, snapScreenPoint);
                if (dist < closesedDist) {
                    closestSnappingPoint = snappingPoint;
                    closesedDist = dist;
                }
            }
            return closestSnappingPoint;
        }

        void UpdateInteracting(InteractorId interactorId) {
            if (currentInteractible == null) {
                interactionMode = InteractionMode.Default;
                return;
            }
            if (!currentInteractible.isOwnedLocally) {
                interactionMode = InteractionMode.Default;
                return;
            }

            var releasing = Mouse.current?.leftButton.wasReleasedThisFrame ?? false;
            
            var button = currentInteractible.GetComponent<InteractibleComponentButton>();
            if (button) {
                if (releasing)
                    button.UpdateValueAndSend(0, false, false, new Vector2());
            }

            var slider = currentInteractible.GetComponent<InteractibleComponentSlider>();
            if (slider) {
                SetSliderFromMousePosition(slider, releasing, false);
            }

            var movable = currentInteractible.GetComponent<InteractibleComponentPickUp>();
            if (movable) {

                if (movable.forceRelease) {
                    releasing = true;
                    movable.forceRelease = false;
                }

                var cam = Player.ThePlayer.cam;
                var ray = cam.ScreenPointToRay(Mouse.current?.position.value ?? Vector2.zero);
                var p = ray.GetPoint(pickUpDistance);
                var rot = cam.transform.rotation * localPickUpRotation;
                var pos = p + cam.transform.rotation * localPickUpPosition;

                var screenPos = Mouse.current?.position.value ?? Vector2.zero;
                var closestSnappingPoint = FindClosestSnappingPointScreenSpace(screenPos, 50f, movable);
                if (movable.GetSnapPoint() != closestSnappingPoint) {
                    var taken = closestSnappingPoint != null && closestSnappingPoint.pickUpObject != null;
                    if (!taken) {
                        movable.SetSnapPointAndSend(closestSnappingPoint, false);
                    }
                }
                if (movable.GetSnapPoint() == null)
                    movable.SetTransformAndSend(pos, rot, releasing, false);
            }

            if (releasing) {
                currentInteractible = null;
                interactionMode = InteractionMode.Default;
            }
        }

        void SetSliderFromMousePosition(InteractibleComponentSlider slider, bool releasing, bool takeOwnership) {
            Ray ray = Player.ThePlayer.cam.ScreenPointToRay(Mouse.current?.position.value ?? Vector2.zero);
            Vector3 p;
            if (RayPlaneIntersection(ray, slider.transform.position, slider.transform.up, out p)) {
                var rel = slider.transform.InverseTransformPoint(p);
                var x = rel.x;
                var w = (x - slider.handleMin) / (slider.handleMax - slider.handleMin);
                var v = Mathf.Lerp(slider.valueMin, slider.valueMax, w);
                slider.UpdateAndSend(v, !releasing, takeOwnership);
            }
        }

        bool RayPlaneIntersection(Ray ray, Vector3 p0, Vector3 n, out Vector3 p) {
            p = Vector3.zero;

            var rel = p0 - ray.origin;
            var dist = -Vector3.Dot(n, rel);
            if (dist < 0f) {
                return false;
            }

            var speed = -Vector3.Dot(ray.direction, n);
            var t = dist / speed;
            p = ray.origin + ray.direction * t;

            return true;
        }

        public static bool GetInteractibleInGameObjectOrParent(GameObject gameObject, out Interactible interactible) {
            interactible = gameObject.GetComponent<Interactible>();
            if (interactible) {
                return true;
            }

            var parent_transform = gameObject.transform.parent;
            if (parent_transform) {
                var parent = parent_transform.gameObject;
                return GetInteractibleInGameObjectOrParent(parent, out interactible);
            }

            interactible = null;
            return false;
        }
    }
}
