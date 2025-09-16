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
                if(!roomManager.isInitialized)
                    roomManager.Init();
                roomManager.Init();
                _roomManager = roomManager;
                return roomManager;
            }
        }
        [HideInInspector] public Room[] rooms;

        [SerializeField] public Room[] staticRooms;
        [HideInInspector] public bool isInitialized;

        public virtual void Init() {
            if (isInitialized)
                return;
            var roomCount = staticRooms.Length;
            rooms = new Room[roomCount];
            InitializeStaticRooms();
            isInitialized = true;
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
