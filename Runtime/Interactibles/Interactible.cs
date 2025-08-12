using System.Collections.Generic;
using UnityEngine;

namespace Muco {
    public class Interactible : MonoBehaviour {
        public InteractibleId id;
        public InteractorId owner;
        public byte prefabIndex;
        public bool idAssigned = false;

        public bool isOwnedLocally {
            get {
                var clientId = Networking.TheNetworking.serverConnection.clientId;
                return owner.user_id == clientId;
            }
        }

        public uint GetInteractibleRoomKey() {
            return (((uint)id.creatorId) << 16) | id.interactibleId;
        }

        public void Init(byte roomId, ushort interactibleId, ushort creatorId, byte prefabIndex) {
            id.roomId = roomId;
            id.interactibleId = interactibleId;
            id.creatorId = creatorId;
            this.prefabIndex = prefabIndex;
            idAssigned = true;

            var components = GetComponents<InteractibleComponentBase>();
            foreach (var component in components) {
                component.Init();
            }
        }

        public void Deinit() {
            var components = GetComponents<InteractibleComponentBase>();
            foreach (var component in components) {
                component.Deinit();
            }
        }

        public InteractibleRoom GetRoom() {
            return RoomManager.TheRoomManager.rooms[id.roomId];
        }

        public void TryTakeOwnership(InteractorId owner) {
            this.owner = owner;
            Networking.TheNetworking.serverConnection.TryClaimData(id.roomId, id.creatorId, id.interactibleId);
        }

        public void TryTakeOwnership(ushort client_id) {
            owner.user_id = client_id;
            Networking.TheNetworking.serverConnection.TryClaimData(id.roomId, id.creatorId, id.interactibleId);
        }

        public void SendState(bool takeOwnership) {
            if (takeOwnership)
                TakeOwnership();
            SendStateWeak();
        }

        public void TakeOwnership() {
            var user_id = Networking.TheNetworking.serverConnection.clientId;
            TryTakeOwnership(user_id);
        }

        public void SendStateWeak() {
            var buffer = new List<byte> { prefabIndex };
            foreach (var component in GetComponents<InteractibleComponentBase>())
                component.Ser(buffer);
            Networking.TheNetworking.TrySendDataAndMemoize(id.roomId, id.creatorId, id.interactibleId, buffer.ToArray());
        }

        public void Notify(byte[] buffer) {
            int cursor = 1;
            var components = GetComponents<InteractibleComponentBase>();
            foreach (var component in components) {
                component.Des(ref cursor, buffer);
            }
        }
    }
}
