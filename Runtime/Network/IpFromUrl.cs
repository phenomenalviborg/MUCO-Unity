using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Muco {
    public class IpFromUrl : ResolveIp
    {
        public string url;
        public string username;
        public string password;

        Coroutine coroutine;
        Address address;

        public override Address Poll()
        {
            if (coroutine == null)
            {
                coroutine = StartCoroutine(GetProtectedText(url, username, password));
            }

            return address;
        }

        IEnumerator GetProtectedText(string uri, string username, string password)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                string auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                webRequest.SetRequestHeader("Authorization", "Basic " + auth);

                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + webRequest.error);
                }
                else
                {
                    string result = webRequest.downloadHandler.text;
                    Debug.Log("Received Text: " + result);

                    var split = result.Split(":");

                    if (split.Length < 2)
                        yield break;

                    string ip = split[0];
                    int port = int.Parse(split[1]);

                    address = new Address
                    {
                        ip = ip,
                        port = port,
                    };
                }
            }
        }
    }
}