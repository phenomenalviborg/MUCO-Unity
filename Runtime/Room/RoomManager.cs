using UnityEngine;

namespace Muco {
    public class RoomManager : MonoBehaviour {
        static RoomManager _roomManager;
        public static RoomManager TheRoomManager {
            get {
                if (_roomManager != null)
                    return _roomManager;
                var roomManager = FindFirstObjectByType<RoomManager>();
                if (roomManager == null)
                    Debug.Log("Could not find RoomManager");
                roomManager.Init();
                _roomManager = roomManager;
                return roomManager;
            }
        }
        [HideInInspector] public Room[] rooms;

        public Room[] staticRooms;

        public virtual void Init() {
            var roomCount = staticRooms.Length;
            rooms = new Room[roomCount];
            InitializeStaticRooms();
        }

        public void InitializeStaticRooms() {
            for (byte i = 0; i < staticRooms.Length; i++) {
                var room = staticRooms[i];
                rooms[i] = room;
                room.roomId = i;
                room.InitializePreMadeInteractibles();
                room.InitRoom();
                room.EnterRoom();
            }
        }
    }
}
