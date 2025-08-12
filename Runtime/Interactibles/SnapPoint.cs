using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Muco {
    public struct SnapPointId {
        public ushort interactibleId;
        public ushort creatorId;
        public byte index;

        public void Ser(List<byte> buffer) {
            Serialize.SerU16(interactibleId, buffer);
            Serialize.SerU16(creatorId, buffer);
            Serialize.SerU8(index, buffer);
        }

        public static void Des(out SnapPointId snapPointId, ref int cursor, byte[] buffer) {
            Serialize.DesU16(out snapPointId.interactibleId, ref cursor, buffer);
            Serialize.DesU16(out snapPointId.creatorId, ref cursor, buffer);
            Serialize.DesU8(out snapPointId.index, ref cursor, buffer);
        }
    }

    public class SnapPoint : MonoBehaviour {
        public InteractibleComponentSnapPoints networkComponent;
        public GameObject pickUpObject;
        public InteractibleComponentPickUp acceptedGameObject;
        public UnityEvent onSnap;
        public UnityEvent onUnsnap;
        public SnapPointId snapPointId;

        public Interactible interactible {
            get { return networkComponent.interactible; }
        }

        private bool IsIndirectChildOff(Transform parent) {
            return IsIndirectChildOff(transform, parent);
        }

        private static bool IsIndirectChildOff(Transform child, Transform parent) {
            if (child.parent == null)
                return false;
            if (child.parent == parent)
                return true;
            return IsIndirectChildOff(child.parent, parent);
        }

        public bool CanSnapToMe(InteractibleComponentPickUp pickUp){
            if(IsIndirectChildOff(pickUp.transform))
                return false;
            if(acceptedGameObject)
                return acceptedGameObject == pickUp;
            return true;
        }
    }
}
