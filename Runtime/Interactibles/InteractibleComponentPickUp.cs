using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Muco {
    public class InteractibleComponentPickUp : InteractibleComponentBase {
        public SnapPoint snapPoint;
        bool wasSleeping;
        public UnityEvent onMovedLocal;
        public UnityEvent onGrabbedLocal;
        public UnityEvent onReleasedLocal;
        bool doRigidbody;
        public bool allowRotation = true;
        public bool isBeingHeldByLocalPlayer = false;
        public bool limitPosition;
        public Vector3 limitPositionMin;
        public Vector3 limitPositionMax;
        public bool removeRigidbodyOnSleep = true;
        public bool removeRigidbodyWhenHeld = true;
        public void Unsnap() {
            if (!snapPoint)
                return;
            
            snapPoint.pickUpObject = null;
            snapPoint = null;
            transform.parent = interactible.GetRoom().transform;
        }

        public override void Init() {
            doRigidbody = GetComponent<Rigidbody>();
        }

        public bool physicsEnabled {
            get {return GetComponent<Rigidbody>();}
            set {
                if (value) {
                    if (!physicsEnabled)
                        gameObject.AddComponent<Rigidbody>();
                } else {
                    Destroy(GetComponent<Rigidbody>());
                }
            }
        }

        public Rigidbody rigidBody {
            get {
                return GetComponent<Rigidbody>();
            }
        }

        void Update() {
            var rb = GetComponent<Rigidbody>();
            if (!rigidBody)
                return;
            var isSleeping = rb.IsSleeping();
            if (isSleeping && !wasSleeping && removeRigidbodyOnSleep)
                OnRigidbodySleep();
            wasSleeping = isSleeping;
        }

        void OnRigidbodySleep() {
            physicsEnabled = false;
            if (interactible.owner.user_id == Networking.TheNetworking.serverConnection.clientId) {
                interactible.SendStateWeak();
            }
        }

        SnapPointId GetSnapPointId() {
            if (!snapPoint) {
                var NullSnapPointId = new SnapPointId {
                    index = 0,
                    interactibleId = 0,
                    creatorId = ushort.MaxValue,
                };
                return NullSnapPointId;
            }
            return snapPoint.snapPointId;
        }

        public override void Ser(List<byte> buffer) {
            Serialize.SerVector3(transform.position, buffer);
            Serialize.SerQuat(transform.rotation, buffer);
            Serialize.SerBool(physicsEnabled, buffer);
            GetSnapPointId().Ser(buffer);
        }

        public SnapPoint GetSnapPointFromId(SnapPointId snapPointId) {
            var room = interactible.GetRoom();
            if (!room)
                return null;

            uint key = (((uint)snapPointId.creatorId) << 16) | snapPointId.interactibleId;
            if (!room.interactibles.ContainsKey(key))
                return null;
            
            var snappingInteractible = room.interactibles[key];
            if (!snappingInteractible)
                return null;

            var ncSnapPoints = snappingInteractible.GetComponent<InteractibleComponentSnapPoints>();
            if (!ncSnapPoints)
                return null;

            if (snapPointId.index >= ncSnapPoints.snapPoints.Count)
                return null;

            return ncSnapPoints.snapPoints[snapPointId.index];
        }

        public override void Des(ref int cursor, byte[] buffer) {
            Vector3 pos;
            Serialize.DesVector3(out pos, ref cursor, buffer);
            Quaternion rot;
            Serialize.DesQuat(out rot, ref cursor, buffer);
            bool physicsEnabled;
            Serialize.DesBool(out physicsEnabled, ref cursor, buffer);
            SnapPointId snapPointId;
            SnapPointId.Des(out snapPointId, ref cursor, buffer);

            transform.position = pos;
            transform.rotation = rot;
            this.physicsEnabled = physicsEnabled;

            var newSnapPoint = GetSnapPointFromId(snapPointId);
            if (snapPoint != newSnapPoint) {
                if(snapPoint) {
                    snapPoint.onUnsnap.Invoke();
                    snapPoint.pickUpObject = null;
                    snapPoint = null;
                    transform.parent = interactible.GetRoom().transform;
                }
                if (newSnapPoint) {
                    snapPoint = newSnapPoint;
                    snapPoint.pickUpObject = gameObject;
                    transform.parent = snapPoint.transform;
                }
            }
        }

        public SnapPoint GetSnapPoint() {
            return snapPoint;
        }

        public void SetSnapPointAndSend(SnapPoint snapPoint, bool takeOwnership) {
            if (this.snapPoint){
                this.snapPoint.pickUpObject = null;
                transform.parent = interactible.GetRoom().transform;
                this.snapPoint.onUnsnap.Invoke();
            }
            this.snapPoint = snapPoint;
            if (this.snapPoint) {
                this.snapPoint.pickUpObject = gameObject;
                transform.parent = this.snapPoint.transform;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                physicsEnabled = false;
                this.snapPoint.onSnap.Invoke();
            }
            interactible.SendState(takeOwnership);
        }

        public void SetSnapPointAndSend_OneArg(SnapPoint snapPoint){
            SetSnapPointAndSend(snapPoint, true);
        }

        public void SetTransformAndSend(Vector3 desiredPos, Quaternion rot, bool free, bool takeOwnership) {
            onMovedLocal.Invoke();
            Debug.DrawLine(desiredPos, Vector3.zero, Color.red);
            Debug.DrawLine(transform.position, Vector3.zero,Color.blue);
            if (limitPosition) desiredPos = LimitPosition(desiredPos);
            var momentum = (desiredPos - transform.position) / Time.fixedDeltaTime;
            transform.position = desiredPos;
            if (allowRotation) transform.rotation = rot;
            physicsEnabled = (!removeRigidbodyWhenHeld | free) & doRigidbody;
            if (rigidBody != null) rigidBody.useGravity = free & doRigidbody;
            if (free) {
                if (rigidBody != null) rigidBody.linearVelocity = momentum;
                onReleasedLocal.Invoke();
                isBeingHeldByLocalPlayer = false;
            } else {
                if (!isBeingHeldByLocalPlayer) {
                    onGrabbedLocal.Invoke();
                    isBeingHeldByLocalPlayer = true;
                }
            }
            interactible.SendState(takeOwnership);
        }

        Vector3 LimitPosition(Vector3 pos){
                Vector3 limitMin = limitPositionMin + transform.parent.position;
                Vector3 limitMax = limitPositionMax + transform.parent.position;
                pos = Vector3.Min(Vector3.Max(pos, limitMin), limitMax);
                return pos;
        }

        public void SetPhysicsEnabledAndSend(bool free, bool takeOwnership) {
            onMovedLocal.Invoke();
            physicsEnabled = (!removeRigidbodyWhenHeld | free) & doRigidbody;
            interactible.SendState(takeOwnership);
        }
    }
    }



