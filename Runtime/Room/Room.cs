using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Muco {
    public class Room : InteractibleRoom {
        [HideInInspector] public byte roomId;
        [HideInInspector] public RoomExtension[] extensions;
        List<Interactible> preMadeInteractibles;
        public List<Interactible> interactiblePrefabs;
        ushort interactibleIdCounter;

        public UnityEvent AfterInit;

        public Interactible SpawnInteractible(byte prefabIndex, Vector3 pos, Quaternion rot, Transform parent)
        {
            var prefab = interactiblePrefabs[prefabIndex];
            var interactible = Instantiate(prefab, pos, rot, parent);
            var creatorId = Networking.TheNetworking.serverConnection.clientId;
            var interactibleId = interactibleIdCounter++;
            interactible.Init(roomId, interactibleId, creatorId, prefabIndex);
            interactible.SendState(true);
            var key = ((uint)creatorId) << 16 | interactibleId;
            interactibles[key] = interactible;
            return interactible;
        }

        public void InitializePreMadeInteractibles() {
            AssignAllChildInteractibles();
            interactibles = new Dictionary<uint, Interactible>();
            for (ushort i = 0; i < preMadeInteractibles.Count; i++) {
                var interactible = preMadeInteractibles[i];
                var creatorId = ushort.MaxValue;
                if (interactible)
                    interactible.Init(roomId, i, creatorId, byte.MaxValue);
                var key = ((uint)creatorId << 16) | i;
                interactibles[key] = interactible;
            }
        }

        public void InitRoom() {
            if (Networking.TheNetworking.facts == null)
                    Networking.TheNetworking.facts = new Dictionary<ulong, byte[]>();
            var facts = Networking.TheNetworking.facts;
            AssignAllRoomExtensions();
            foreach(var extension in extensions) {
                extension.Init();
                extension.UpdateLanguage(Player.ThePlayer.language);
            }
            

            foreach (var key in facts.Keys)
            {
                byte factRoomId = (byte)(key >> 32);
                uint interactibleKey = (uint)(key & 0xFFFF_FFFF);
                if (factRoomId == roomId)
                {
                    ProcessDataNotify(interactibleKey, facts[key]);
                }
            }
            AfterInit.Invoke();
        }

        public void DeinitRoom() {
            foreach (var keyValuePair in interactibles) {
                var interactible = keyValuePair.Value;
                if (interactible)
                    interactible.Deinit();
            }
        }

        public void EnterRoom() {
            foreach(var extension in extensions) {
                extension.Enter();
                extension.UpdateLanguage(Player.ThePlayer.language);
            }
        }
        
        public void ExitRoom() {
            foreach(var extension in extensions) {
                extension.Exit();
            }
        }

        public void UpdateLanguage(Language language) {
            foreach(var extension in extensions) {
                extension.UpdateLanguage(language);
            }
        }

        public void ProcessDataNotify(uint key, byte[] data) {
            if (interactibles.ContainsKey(key)) {
                var interactible = interactibles[key];
                if (interactible)
                    interactible.Notify(data);
            }
            else {
                var prefabIndex = data[0];
                var prefab = interactiblePrefabs[prefabIndex];
                var interactible = Instantiate(prefab);
                var interactibleId = (ushort)(key & 0xFFFF);
                var creatorId = (ushort)(key >> 16);
                interactible.Init(roomId, interactibleId, creatorId, prefabIndex);
                interactibles[key] = interactible;
                interactible.Notify(data);
            }
        }

        public void AssignAllRoomExtensions(){
            extensions = GetComponents<RoomExtension>();
        }

        public void AssignAllChildInteractibles() {
            preMadeInteractibles = new List<Interactible>{ null };
            AssignChildInteractiblesRecursive(transform);
        }

        private void AssignChildInteractiblesRecursive(Transform root) {
            var interactible = root.gameObject.GetComponent<Interactible>();
            if (interactible)
                preMadeInteractibles.Add(interactible);
            for (int i = 0; i < root.childCount; i++) {
                var child = root.GetChild(i);
                AssignChildInteractiblesRecursive(child);
            }
        }
    }
}
