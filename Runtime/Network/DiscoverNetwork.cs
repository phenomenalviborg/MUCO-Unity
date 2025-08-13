using UnityEngine;

namespace Muco {
    public class DiscoverNetwork : ResolveIp
    {
        public string serviceType;
        NetworkDiscoverer networkDiscoverer;

        public override Address Poll()
        {
            if (serviceType == "")
                Debug.Log("Service Type not specified!!!");

            if (networkDiscoverer == null)
                networkDiscoverer = new NetworkDiscoverer(serviceType);

            var result = networkDiscoverer.Poll();

            if (result == null)
                return null;

            networkDiscoverer.Dispose();
            networkDiscoverer = null;

            var split = result.Split(":");

            if (split.Length < 2)
                return null;

            string ip = split[0];
            int port = int.Parse(split[1]);

            return new Address
            {
                ip = ip,
                port = port,
            };
        }
    }
}