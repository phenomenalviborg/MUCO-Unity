using System;
using System.Collections.Generic;
using UnityEngine;

namespace Muco {
    public class InteractibleSystemHandTracking : MonoBehaviour {
        [HideInInspector]
        private bool isInitialized;
        private static InteractibleSystemHandTracking _interactibleSystemHandTracking;
        public static InteractibleSystemHandTracking TheInteractibleSystemHandTracking
        {
            get
            {
                if (_interactibleSystemHandTracking != null)
                {
                    return _interactibleSystemHandTracking;
                }

                InteractibleSystemHandTracking interactibleSystemHandTracking = UnityEngine.Object.FindFirstObjectByType<InteractibleSystemHandTracking>();
                if (interactibleSystemHandTracking == null)
                {
                    Debug.Log("No interacrible system hand tracking found");
                }

                if (!interactibleSystemHandTracking.isInitialized)
                {
                    interactibleSystemHandTracking.Init();
                }

                _interactibleSystemHandTracking = interactibleSystemHandTracking;
                return interactibleSystemHandTracking;
            }
        }
        public bool debugFingers;
        byte system_id = 1;
        public FingerProvider[] fingerProviders;
        [HideInInspector]
        public FingerProvider.Finger[] fingers;
        public FingerVis[] fingerVisualizers;
        public FingerVis fingerVisPrefab;
        public float maxPickUpDistance = 0.1f;
        public float maxSlideDistane = 0.1f;
        public float pressRadius = 0.005f;

        void Awake()
        {
            var fingerCount = FingerCount();
            fingers = new FingerProvider.Finger[fingerCount];

            for (int i = 0; i < fingerCount; i++)
            {
                fingers[i].Init();
            }

            if (debugFingers)
            {
                fingerVisualizers = new FingerVis[fingerCount];
                for (int i = 0; i < fingerCount; i++)
                {
                    fingerVisualizers[i] = Instantiate(fingerVisPrefab);
                }
            }
            if (!isInitialized)
            {
                Init();
            }
        }
        protected virtual void Init()
        {
            if (!isInitialized)
            {
                isInitialized = true;
            }
        }

        int FingerCount() {
            var acc = 0;
            foreach(var fingerProvider in fingerProviders) {
                acc += fingerProvider.FingerCount();
            }
            return acc;
        }

        public void UpdateFingerVis() {
            for (int i = 0; i < fingers.Length; i++) {
                VrDebug.SetValue("fingers", "" + i, "" + fingers[i].currentPosition);
                fingerVisualizers[i].transform.position = fingers[i].currentPosition;
                fingerVisualizers[i].SetVisual(fingers[i].isPinching,fingers[i].fingerState);
            }
        }

        void Update() {
            int fingerIndex = 0;

            foreach(var fingerProvider in fingerProviders) {
                var fingerCount = fingerProvider.FingerCount();
                var fingerEndIndex = fingerIndex + fingerCount;
                fingerProvider.UpdateFingers(fingers.AsSpan(fingerIndex..fingerEndIndex));
                fingerIndex = fingerEndIndex;
            }

            if (debugFingers)
                UpdateFingerVis();

            var rooms = RoomManager.TheRoomManager.rooms;
            for (int r = 0; r < rooms.Length; r++) {
                var room = rooms[r];
                if (room == null)
                    continue;

                var allInteractibles = room.interactibles;

                var user_id = Networking.TheNetworking.serverConnection.clientId;
                var interactor_id = new InteractorId(user_id, system_id, 0);

                // foreach (FingerProviderTransforms fingerProvider in fingerProviders) {
                //     switch (fingerProvider.handPoseType) {
                //         case HandPoseType.Grabbing:
                //             fingers[3].isPinching = true;
                //             break;
                //     }
                // }

                
                for (int i = 0; i < fingers.Length; i++) {
                    var finger = fingers[i];
                    
                    if (finger.activeInteractible) {
                        if (!finger.activeInteractible.isOwnedLocally) {
                            finger.activeInteractible = null;
                            finger.fingerState = FingerProvider.FingerState.Default;
                        }
                    }

                    AlwaysUpdate(finger, allInteractibles, interactor_id);

                    switch (finger.fingerState) {
                        case FingerProvider.FingerState.Default:
                            
                            DefaultUpdate(ref finger, allInteractibles, interactor_id);
                            break;
                        case FingerProvider.FingerState.Dragging:
                            DraggingUpdate(ref finger, interactor_id);
                            break;
                        case FingerProvider.FingerState.Movable:
                            MovableUpdate(ref finger);
                            break;
                    }

                    fingers[i] = finger;
                    interactor_id.interactor_id++;
                }
            }
        }

        public void AlwaysUpdate(FingerProvider.Finger finger, Dictionary<uint, Interactible> allInteractibles, InteractorId interactor_id) {
            foreach (var keyValuePair in allInteractibles) {
                var interactible = keyValuePair.Value;
                if (!interactible)
                    continue;
                if (!interactible.gameObject.activeInHierarchy)
                    continue;

                var button = interactible.GetComponent<InteractibleComponentButton>();
                if (button) {
                    ProcessFingerButtonInteraction(finger, interactor_id, interactible, button);
                }
            }
        }

        public void DefaultUpdate(ref FingerProvider.Finger finger, Dictionary<uint, Interactible> allInteractibles, InteractorId interactor_id) {
            
            if (finger.isPinching && !finger.wasPinching) {
                Interactible interactible;
                if (FindDraggableInteractible(allInteractibles, finger.currentPosition, out interactible)) {
                    var slider = interactible.GetComponent<InteractibleComponentSlider>();
                    if (slider) {
                        UpdateSliderFromPosition(slider, finger.currentPosition, false, true);
                    }
                    finger.activeInteractible = interactible;
                    finger.fingerState = FingerProvider.FingerState.Dragging;
                }
                if (FindInteractibleOfType<InteractibleComponentPickUp>(finger.currentPosition, out interactible)) {
                    var movable = interactible.GetComponent<InteractibleComponentPickUp>();
                    if (movable) {
                        Debug.Log(finger.hand.name+" is at "+finger.hand.position);
                        finger.pickUpLocalPosition = finger.hand.InverseTransformPoint(movable.transform.position);
                        finger.pickUpLocalRotation = Quaternion.Inverse(finger.hand.rotation) * movable.transform.rotation;
                        interactible.TryTakeOwnership(interactor_id);
                    }
                    finger.activeInteractible = interactible;
                    finger.fingerState = FingerProvider.FingerState.Movable;
                }
            }
        }

        public void UpdateSliderFromPosition(InteractibleComponentSlider slider, Vector3 p, bool releasing, bool takeOwnership) {
            var rel = slider.transform.InverseTransformPoint(p);
            var x = rel.x;
            var w = (x - slider.handleMin) / (slider.handleMax - slider.handleMin);
            var v = Mathf.Lerp(slider.valueMin, slider.valueMax, w);
            slider.UpdateAndSend(v, !releasing, takeOwnership);
        }

        public bool FindDraggableInteractible(Dictionary<uint, Interactible> allInteractibles, Vector3 p, out Interactible outInteractible) {
            outInteractible = null;

            float closestDist = Mathf.Infinity;

            foreach (var keyValuePair in allInteractibles) {
                var interactible = keyValuePair.Value;
                if (!interactible)
                    continue;

                var slider = interactible.GetComponent<InteractibleComponentSlider>();
                if (slider) {
                    var dist = (slider.handle.position - p).magnitude;
                    if (dist < closestDist && dist < maxSlideDistane) {
                        outInteractible = interactible;
                        closestDist = dist;
                    }
                }
            }

            return outInteractible != null;
        }

        public bool FindInteractibleOfType<InteractibleType>(Vector3 p, out Interactible outInteractible) {
            outInteractible = null;

            float closestDist = Mathf.Infinity;

            Collider[] hitColliders = Physics.OverlapSphere(p, maxPickUpDistance);
            foreach (var collider in hitColliders) {
                Interactible interactible;
                if(!InteractibleSystemPC.GetInteractibleInGameObjectOrParent(collider.gameObject, out interactible))
                    continue;

                var pickUp = interactible.GetComponent<InteractibleType>();
                if (pickUp == null)
                    continue;

                var closestPoint = collider ? collider.ClosestPoint(p) : interactible.transform.position;
                var dist = Vector3.Distance(p, closestPoint);
                if (dist < closestDist && dist < maxPickUpDistance) {
                    outInteractible = interactible;
                    closestDist = dist;
                }
            }

            return outInteractible != null;
        }

        public void DraggingUpdate(ref FingerProvider.Finger finger, InteractorId interactor_id) {
            var releasing = !finger.isPinching;

            var slider = finger.activeInteractible.GetComponent<InteractibleComponentSlider>();
            if (slider) {
                UpdateSliderFromPosition(slider, finger.currentPosition, releasing, false);
            }

            if (releasing) {
                finger.activeInteractible = null;
                finger.fingerState = FingerProvider.FingerState.Default;
                return;
            }
        }

        SnapPoint FindClosestSnappingPoint(Vector3 pos, float maxDist, InteractibleComponentPickUp pickUp) {
            var snappingPoints = FindObjectsByType<SnapPoint>(0);
            SnapPoint closestSnappingPoint = null;
            float closestDist = maxDist;
            foreach(var snappingPoint in snappingPoints) {
                if (!snappingPoint.CanSnapToMe(pickUp))
                    continue;
                var dist = Vector3.Distance(pos, snappingPoint.transform.position);
                if (dist < closestDist) {
                    closestSnappingPoint = snappingPoint;
                    closestDist = dist;
                }
            }
            return closestSnappingPoint;
        }

        public void MovableUpdate(ref FingerProvider.Finger finger) {
            var movable = finger.activeInteractible.GetComponent<InteractibleComponentPickUp>();
            var releasing = !finger.isPinching;
            if (movable.forceRelease) {
                releasing = true;
                movable.forceRelease = false;
            }
            if (movable) {
                var pos = finger.hand.TransformPoint(finger.pickUpLocalPosition);;
                var rot = finger.hand.rotation * finger.pickUpLocalRotation;
                var closestSnappingPoint = FindClosestSnappingPoint(pos, 0.05f, movable);
                if (movable.GetSnapPoint() != closestSnappingPoint) {
                    var taken = closestSnappingPoint != null && closestSnappingPoint.pickUpObject != null;
                    if (!taken)
                        movable.SetSnapPointAndSend(closestSnappingPoint, false);
                }
                if (movable.GetSnapPoint() == null)
                    movable.SetTransformAndSend(pos, rot, releasing, false);
            }

            if (releasing) {
                finger.activeInteractible = null;
                finger.fingerState = FingerProvider.FingerState.Default;
                return;
            }
        }

        public void ProcessFingerButtonInteraction(FingerProvider.Finger finger, InteractorId interactor_id, Interactible interactible, InteractibleComponentButton button) {
            var relCurrentPosition = button.transform.InverseTransformPoint(finger.currentPosition);

            var y = -relCurrentPosition.y + pressRadius;
            var xz = new Vector2(relCurrentPosition.x, relCurrentPosition.z);
            var h = xz.magnitude;

            var buttonRadius = 0.02f;
            var maxPressedDist = 0.05f;
            var hoverHeight = 0.15f;

            var is_in_cylinder = h < buttonRadius + pressRadius && y <= maxPressedDist && y >= -hoverHeight;
            var was_interacting = finger.currentInteractibles.Contains(interactible);
            var is_interacting = is_in_cylinder && (was_interacting | y < 0);
            var is_pressing = is_interacting && y > 0;
            var was_pressing = button.PressedLocally();

            if (is_interacting) {
                if (is_pressing)
                    button.UpdateValueAndSend(y, true, true, xz);
                else if (was_pressing)
                    button.UpdateValueAndSend(0, false, true, xz);
                
                if (!was_interacting)
                    finger.currentInteractibles.Add(interactible);
            }
            else if (was_interacting) {
                finger.currentInteractibles.Remove(interactible);
                if (was_pressing)
                    button.UpdateValueAndSend(0, false, true, xz);
            }
        }
    }
}
