using System.Collections.Generic;
using System.Net.Sockets;

namespace Muco
{
    public class Message
    {
        public static bool TrySendMessage(NetworkStream stream, byte[] sendData)
        {
            try
            {
                var bytes = new List<byte>();
                Serialize.SerI32(sendData.Length, bytes);
                bytes.AddRange(sendData);
                stream.Write(bytes.ToArray(), 0, bytes.Count);
                stream.Flush();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
