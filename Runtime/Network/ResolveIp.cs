using UnityEngine;

namespace Muco
{
    public class ResolveIp : MonoBehaviour {
        public class Address {
            public string ip;
            public int port;
        }

        public virtual Address Poll() {
            return null;
        }
    }
}