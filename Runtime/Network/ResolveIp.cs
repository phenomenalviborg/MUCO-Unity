using UnityEngine;

public class ResolveIp : MonoBehaviour {
    public class Address {
        public string ip;
        public int port;
    }

    public virtual Address Poll() {
        return null;
    }
}
