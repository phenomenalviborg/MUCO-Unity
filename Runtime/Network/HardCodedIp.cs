using System.Diagnostics;
using UnityEngine;


public class HardCodedIp : ResolveIp {
    public string ip;
    public int port;

    public override Address Poll() {
        if (Application.isEditor)
            UnityEngine.Debug.Log("Polling IP. If you have made changes, make sure to restart the server.");
        return new Address {
            ip = ip,
            port = port,
        };
    }
}
