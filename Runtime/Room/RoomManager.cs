using System.Linq;
using UnityEngine;

namespace Muco {
    public class RoomManager : MonoBehaviour
    {
        static RoomManager _roomManager;
        public static RoomManager TheRoomManager
        {
            get
            {
                if (_roomManager != null)
                    return _roomManager;
                var roomManager = FindFirstObjectByType<RoomManager>();
                if (roomManager == null)
                    Debug.Log("Could not find RoomManager");
                if (!roomManager.isInitialized)
                    roomManager.Init();
                roomManager.Init();
                _roomManager = roomManager;
                return roomManager;
            }
        }
        [HideInInspector] public Room[] rooms;

        [SerializeField] public Room[] staticRooms;
        [SerializeField] private bool autoPopulateStaticRoomsOnInit = true;

        [HideInInspector] public bool isInitialized;

        public virtual void Init()
        {
            if (isInitialized)
                return;
            var roomCount = staticRooms.Length;
            rooms = new Room[roomCount];
            if (autoPopulateStaticRoomsOnInit) {
                AutoPopulateStaticRooms();
            }
            InitializeStaticRooms();
            isInitialized = true;
        }

        public void InitializeStaticRooms()
        {
            if (staticRooms.Length == 0 ) return;
            for (byte i = 0; i < staticRooms.Length; i++)
            {
                var room = staticRooms[i];
                rooms[i] = room;
                room.roomId = i;
                room.InitializePreMadeInteractibles();
                room.InitRoom();
                room.EnterRoom();
            }
        }
        
        public void AutoPopulateStaticRooms()
        {
            Room[] roomsInScene = FindObjectsByType<Room>(FindObjectsSortMode.None);

            if (roomsInScene.Length == 0)
            {
                return;
            }

            var validRooms = roomsInScene.Where(room => room != null).ToArray();

            if (staticRooms == null)
            {
                staticRooms = new Room[0];
            }

            var existingRooms = staticRooms ?? new Room[0];
            var combinedRooms = existingRooms.Union(validRooms).Where(room => room != null).ToArray();

            if (combinedRooms.Length != existingRooms.Length)
            {
                staticRooms = combinedRooms;
            }
        }
    }
}
