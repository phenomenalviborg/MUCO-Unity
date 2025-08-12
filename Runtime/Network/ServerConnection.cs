using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Muco {
    [Serializable]
    public class ServerConnection {
        public TcpClient client;
        public byte[] generalBuffer = new byte[1024];
        public List<byte> clientBuffer = new List<byte>();
        public ushort clientId = ushort.MaxValue;
        public bool connectedToServer = false;
        public bool hasSentNetworkVersion = false;
        public bool networkInitialized = false;

        public bool TrySendDisconnectMessage() {
            var data = new List<byte>();
            Serialize.SerI32((int)ClientToServerMessageType.Disconnect, data);
            var stream = client.GetStream();
            return Message.TrySendMessage(stream, data.ToArray());
        }

        public bool TrySendSetClientType() {
            var data = new List<byte>();
            Serialize.SerI32((int)ClientToServerMessageType.SetClientType, data);
            Serialize.SerI32((int)ClientType.Player, data);
            var stream = client.GetStream();
            return Message.TrySendMessage(stream, data.ToArray());
        }

        public bool TryBroadcastBytes(byte[] data, BroadcastType broadcastType, ushort address) {
            var buffer = new List<byte>();

            ClientToServerMessageType messageType;
            if (broadcastType == BroadcastType.All)
                messageType = ClientToServerMessageType.BroadcastBytesAll;
            else if (broadcastType == BroadcastType.Other)
                messageType = ClientToServerMessageType.BroadcastBytesOther;
            else if (broadcastType == BroadcastType.Spesific)
                messageType = ClientToServerMessageType.BinaryMessageTo;
            else {
                Debug.Log("Problem with message type");
                return false;
            }

            Serialize.SerI32((int)messageType, buffer);
            if (broadcastType == BroadcastType.Spesific) {
                Serialize.SerU16(address, buffer);
            }
            buffer.AddRange(data);
            try {
                var stream = client.GetStream();
                return Message.TrySendMessage(stream, buffer.ToArray());
            }
            catch {
                return false;
            }
        }

        public bool TrySendData(byte room, ushort creator_id, ushort index, byte[] data) {
            var buffer = new List<byte>();
            var messageType = ClientToServerMessageType.SetData;
            Serialize.SerI32((int)messageType, buffer);
            Serialize.SerU8(room, buffer);
            Serialize.SerU16(creator_id, buffer);
            Serialize.SerU16(index, buffer);
            buffer.AddRange(data);

            try {
                var stream = client.GetStream();
                return Message.TrySendMessage(stream, buffer.ToArray());
            }
            catch {
                return false;
            }
        }

        public bool TryClaimData(byte room, ushort creator_id, ushort index) {
            var buffer = new List<byte>();
            var messageType = ClientToServerMessageType.ClaimData;
            Serialize.SerI32((int)messageType, buffer);
            Serialize.SerU8(room, buffer);
            Serialize.SerU16(creator_id, buffer);
            Serialize.SerU16(index, buffer);

            try {
                var stream = client.GetStream();
                return Message.TrySendMessage(stream, buffer.ToArray());
            }
            catch {
                return false;
            }
        }

        public bool TrySendTaggedBuffer(BinaryMessageType tag, List<byte> buffer, BroadcastType broadcastType, ushort address) {
            var data = new List<byte>();
            Serialize.SerI32((int)tag, data);
            data.AddRange(buffer);
            return TryBroadcastBytes(data.ToArray(), broadcastType, address);
        }

        public void Close() {
            if (client != null && client.Connected) {
                try
                {
                    TrySendDisconnectMessage();
                    client.Close();
                }
                catch { }
            }
            client = null;
            connectedToServer = false;
            hasSentNetworkVersion = false;
            networkInitialized = false;
            clientId = ushort.MaxValue;
            clientBuffer.Clear();
        }
    }
}
