using System.Collections.Generic;
using UnityEngine;

namespace Muco {
    [RequireComponent(typeof(Interactible))]
    public class InteractibleComponentBase : MonoBehaviour {
        [Tooltip("When true, this component never broadcasts its state over the network. " +
                 "State changes and events still apply locally per-client, but its bytes are omitted " +
                 "from the interactible's network packet. For consistent behaviour set this identically " +
                 "on every client (e.g. in the prefab/scene).")]
        public bool localOnly = false;

        public Interactible interactible {
            get {
                return GetComponent<Interactible>();
            }
        }

        public bool ownedLocally {
            get {
                var owner = interactible.owner.user_id;
                var clientId = Networking.TheNetworking.serverConnection.clientId;
                return owner == clientId;
            }
        }

        public virtual void Ser(List<byte> buffer) {}
        public virtual void Des(ref int cursor, byte[] buffer) {}
        public virtual void Init() {}
        public virtual void Deinit() {}
    }
}
