using System.Collections.Generic;
using UnityEngine;

namespace Muco {
    [System.Serializable]
    public class GhostSystem {
        public PlayerRemoteIndicatorBase playerIndicatorPrefab;
        public HandPrefabs[] ghostHandPrefabs;

        public Dictionary<int, PlayerGhost> ghosts = new Dictionary<int, PlayerGhost>();

        public void RemoveAllGhosts() {
            Debug.Log("Removing all ghosts");
            foreach (var id in ghosts.Keys) {
                var ghost = ghosts[id];
                GameObject.Destroy(ghost.playerIndicator.gameObject);
                if (ghost.handL)
                    GameObject.Destroy(ghost.handL.gameObject);
                if (ghost.handR)
                    GameObject.Destroy(ghost.handR.gameObject);
            }

            ghosts.Clear();
        }

        public PlayerGhost GetGhost(int id) {
            PlayerGhost ghost;
            if (ghosts.TryGetValue(id, out ghost)) {
                return ghost;
            }
            else {
                return AddPlayerGhost(id);
            }
        }

        public void SetGhostLevel(int id, int level) {
            GetGhost(id).SetLevel(level);
        }

        public bool DesGhostHands(int id, ref int cursor, byte[] buffer) {
            var ghost = GetGhost(id);
            return ghost.DesHands(ref cursor, buffer, ghostHandPrefabs);
        }

        public void SetGhostColor(int id, Color color) {
            GetGhost(id).SetColor(color);
        }
        
        public void SetGhostIsVisible(int id, bool isVisible) {
            GetGhost(id).SetIsVisible(isVisible);
        }

        public PlayerGhost AddPlayerGhost(int id) {
            var playerIndicator = MonoBehaviour.Instantiate(playerIndicatorPrefab);
            playerIndicator.Init();
            var ghost = new PlayerGhost { playerIndicator = playerIndicator };
            ghosts.Add(id, ghost);
            return ghost;
        }
    }
}
