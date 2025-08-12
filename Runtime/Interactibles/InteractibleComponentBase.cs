using System.Collections.Generic;
using UnityEngine;

namespace Muco {
    [RequireComponent(typeof(Interactible))]
    public class InteractibleComponentBase : MonoBehaviour {
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
