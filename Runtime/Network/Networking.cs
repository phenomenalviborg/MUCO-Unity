using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

namespace Muco {
    public class Networking : MonoBehaviour {
        static Networking _networking;
        public static Networking TheNetworking {
            get {
                if (_networking != null)
                    return _networking;
                var networking = FindFirstObjectByType <Networking>();
                if (networking == null)
                    Debug.Log("Could not find Networking");
                _networking = networking;
                return networking;
            }
        }

        public ResolveIp resolveIp;

        public ServerConnection serverConnection;

        public GhostSystem ghostSystem;

        bool isVisible = true;

        // public OVRPlugin.SystemHeadset headsetType;

        public Material playerMaterial;
        public List<Color> genaricPlayerCollorFallbacks;

        public FpsCounter fpsCounter;
        public DeviceTemperature deviceTemperature;

        public Config config;

        public float reconnectTimer;
        public float reconnectTimeout;

        List<List<byte>> networkBuffers;
        int currentBufferIndex = 0;
        List<byte> diff;

        public Dictionary<ulong, byte[]> facts;
        
        public ulong CreateKey(byte room, ushort creator_id, ushort index)
        {
            ulong room_part = ((ulong)room) << 32;
            ulong creator_id_part = ((ulong)creator_id) << 16;
            ulong index_part = ((ulong)index) << 0;
            return room_part | creator_id_part | index_part;
        }

        void ProcessDataNotify(byte roomId, ushort creator_id, ushort index, byte[] data) {
            var key = CreateKey(roomId, creator_id, index);
            facts[key] = data;

            if (roomId >= RoomManager.TheRoomManager.rooms.Length)
                return;

            var room = RoomManager.TheRoomManager.rooms[roomId];
            if (room != null) {
                var interactibleKey = (uint)(key & 0xFFFF_FFFF);
                room.ProcessDataNotify(interactibleKey, data);
            }
        }

        public void TrySendDataAndMemoize(byte roomId, ushort creatorId, ushort interactibleId, byte[] data) {
            var key = CreateKey(roomId, creatorId, interactibleId);
            facts[key] = data;
            TheNetworking.serverConnection.TrySendData(roomId, creatorId, interactibleId, data);
        }

        void ProcessDataOwner(byte roomId, ushort creator_id, ushort index, ushort owner) {
            var room = RoomManager.TheRoomManager.rooms[roomId];
            if (room == null)
                return;

            var key = (((uint)creator_id) << 16) | index;
            if (!room.interactibles.ContainsKey(key))
                return;
            var interatible = room.interactibles[key];
            interatible.owner.user_id = owner;
        }

        string GetUniqueDeviceString() {
            return SystemInfo.deviceUniqueIdentifier;
        }

        public int GetUniqueDeviceInt() {
            return Math.Abs(GetUniqueDeviceString().GetHashCode());
        }

        bool isInitialized = false;
        void Init() {
            config.TryLoad(true);
            isInitialized = true;
        }

        public EnvData GetEnvironmentData() {
            if (!isInitialized)
                Init();

            return config.environmentData;
        }

        void ForgetServer() {
            serverConnection.Close();
            ghostSystem.RemoveAllGhosts();
        }


        void Awake() {
            if (ghostSystem.playerIndicatorPrefab == null)
            {
                Debug.LogWarning($"Warning: {ghostSystem.playerIndicatorPrefab} is not assigned in the inspector.");
            }

            VrDebug.SetValue("Config", "UniqueDeviceString", GetUniqueDeviceString());
            VrDebug.SetValue("Config", "UniqueDeviceInt", "" + GetUniqueDeviceInt());

            if (facts == null) {
                facts = new Dictionary<ulong, byte[]>();
            }

            if (!isInitialized) {
                Init();
            }

            reconnectTimer = 0;

            networkBuffers = new List<List<byte>> {
                new List<byte>(),
                new List<byte>()
            };

            diff = new List<byte>();
        }

        void Update() {
            VrDebug.SetValue("Networking", "is connected", "" + serverConnection.connectedToServer);
            VrDebug.SetValue("Networking", "is initialized", "" + serverConnection.networkInitialized);
            VrDebug.SetValue("Networking", "client id", "" + serverConnection.clientId);

            if (!serverConnection.connectedToServer) {
                if (serverConnection.client == null) {
                    serverConnection.client = new TcpClient {
                        NoDelay = true
                    };
                }
                if (serverConnection.client.Connected) {
                    Debug.Log("Client managed to connect");
                    serverConnection.connectedToServer = true;
                    if (!serverConnection.hasSentNetworkVersion) {
                        var initialMessage = new List<byte>();
                        byte[] network_version_msg = {0, 0, 6};
                        Debug.Log("sending network version: " + network_version_msg[0] + "." + network_version_msg[1] + "." + network_version_msg[2]);
                        initialMessage.AddRange(network_version_msg);
                        Serialize.SerI32(GetUniqueDeviceInt(), initialMessage);
                        var stream = serverConnection.client.GetStream();
                        stream.Write(initialMessage.ToArray());
                        serverConnection.hasSentNetworkVersion = true;
                    }
                    VrDebug.SetValue("Networking", "reconnect in", "---");
                }
                else {
                    reconnectTimer -= Time.deltaTime;
                    if (reconnectTimer <= 0) {
                        try {
                            var address = resolveIp.Poll();
                            if (address == null) {
                                reconnectTimer = 0;
                                VrDebug.SetValue("Networking", "reconnect in", "waiting for ip");
                            }
                            if (address != null) {
                                serverConnection.client.ConnectAsync(address.ip, address.port);
                                reconnectTimer = reconnectTimeout;
                            }
                        }
                        catch (Exception e) {
                            Debug.Log("Periodic reconnect failed with exception " + e);
                            ForgetServer();
                            reconnectTimer = reconnectTimeout;
                        }
                    }
                    else {
                        VrDebug.SetValue("Networking", "reconnect in", "" + reconnectTimer);
                    }
                }
            }

            if (serverConnection.connectedToServer) {
                if (serverConnection.networkInitialized) {
                    TryNotifyAllPlayerDataDiff();
                }

                if (!serverConnection.client.Connected) {
                    ForgetServer();
                    return;
                }

                var stream = serverConnection.client.GetStream();
                while (stream.DataAvailable) {
                    var recv = stream.Read(serverConnection.generalBuffer, 0, serverConnection.generalBuffer.Length);
                    for (int i = 0; i < recv; i++) {
                        serverConnection.clientBuffer.Add(serverConnection.generalBuffer[i]);
                    }
                }

                while (serverConnection.clientBuffer.Count > 4) {
                    int cursor = 0;
                    int length;
                    var arr = serverConnection.clientBuffer.ToArray();
                    if (Serialize.DesI32(out length, ref cursor, arr))
                        if (serverConnection.clientBuffer.Count < length + 4)
                            break;

                    ProcessMessageFromServer(ref cursor, arr, length + 4);

                    if (cursor != length + 4) {
                        var s = "";
                        for (int i = 0; i < length + 4; i++) {
                            s += serverConnection.clientBuffer[i].ToString();
                            s += " ";
                        }
                        Debug.Log("Problem with message length: " + s);

                        cursor = 4;
                        ProcessMessageFromServer(ref cursor, arr, length + 4);
                    }

                    serverConnection.clientBuffer.RemoveRange(0, length + 4);
                }
            }
        }

        public void SerializeAllPlayerData(List<byte> buffer) {
            for (int i = 0; i < (int)PlayerDataType.Count; i++) {
                var dataType = (PlayerDataType)i;
                SerializePlayerData(dataType, buffer);
            }
        }

        public void UpdateAllPlayerData(int senderId, ref int cursor, byte[] buffer) {
            var ghost = ghostSystem.GetGhost(senderId);
            ghost.UpdateAllPlayerData(ref cursor, buffer, ghostSystem);
        }

        public void UpdateAllPlayerDataDiff(int senderId, ref int cursor, byte[] buffer) {
            var ghost = ghostSystem.GetGhost(senderId);
            ghost.UpdateAllPlayerDataDiff(ref cursor, buffer, ghostSystem);
        }

        public void SerializePlayerData(PlayerDataType dataType, List<byte> buffer) {
            switch (dataType) {
                case PlayerDataType.DeviceId:
                    var device_id = GetUniqueDeviceInt();
                    Serialize.SerI32(device_id, buffer);
                    break;
                case PlayerDataType.Color:
                    Serialize.SerColor(config.color, buffer);
                    break;
                case PlayerDataType.Trans:
                    var head = Player.ThePlayer.head;
                    Serialize.SerTransGlobal(head, buffer);
                    break;
                case PlayerDataType.Level:
                    float level = Player.ThePlayer.currentLevelIndex;
                    Serialize.SerFloat(level, buffer);
                    break;
                case PlayerDataType.Hands:
                    var handTypeIndex = (byte)Player.ThePlayer.handType;
                    buffer.Add(handTypeIndex);
                    var confidenceL = Player.ThePlayer.handTrackingConfidenceL.GetConfidence();
                    var confidenceR = Player.ThePlayer.handTrackingConfidenceR.GetConfidence();
                    Serialize.SerBool(confidenceL, buffer);
                    Serialize.SerBool(confidenceR, buffer);
                    Serialize.SerTransGlobal(Player.ThePlayer.handParentL, buffer);
                    Serialize.SerTransGlobal(Player.ThePlayer.handParentR, buffer);
                    Player.ThePlayer.handL.Ser(buffer);
                    Player.ThePlayer.handR.Ser(buffer);
                    break;
                case PlayerDataType.DevMode:
                    var devMode = DeveloperMode.TheDeveloperMode.isOn;
                    Serialize.SerBool(devMode, buffer);
                    break;
                case PlayerDataType.IsVisible:
                    Serialize.SerBool(isVisible, buffer);
                    break;
                case PlayerDataType.Language:
                    Serialize.SerI32((int)Player.ThePlayer.language, buffer);
                    break;
                case PlayerDataType.EnvironmentData:
                    var envData = GetEnvironmentData();
                    var playerTrans = Player.ThePlayer.transform;
                    Serialize.SerString(envData.name, buffer);
                    Serialize.SerString(envData.code, buffer);
                    Serialize.SerVector3(playerTrans.position, buffer);
                    Serialize.SerVector3(playerTrans.rotation.eulerAngles, buffer);
                    break;
                case PlayerDataType.DeviceStats:
                    buffer.Add((byte)SystemInfo.batteryStatus);
                    Serialize.SerFloat(SystemInfo.batteryLevel, buffer);
                    
                    Serialize.SerFloat(fpsCounter.fps, buffer);
                    float trackingConfidence = 0f;
                    var altTrackingXr = Player.ThePlayer.custonAltTrackingXr;
                    if (altTrackingXr)
                        trackingConfidence = altTrackingXr.GetTrackingConfidence();
                    Serialize.SerFloat(trackingConfidence, buffer);

                    buffer.Add((byte)deviceTemperature.metrics.WarningLevel);
                    Serialize.SerFloat(deviceTemperature.metrics.TemperatureLevel, buffer);
                    Serialize.SerFloat(deviceTemperature.metrics.TemperatureTrend, buffer);
                    break;
                case PlayerDataType.AudioVolume:
                    var volume = VolumeService.SystemVolume;
                    Serialize.SerFloat(volume, buffer);
                    break;
            }
        }

        public bool TryNotifyPlayerData(PlayerDataType dataType) {
            var buffer = new List<byte>();
            Serialize.SerI32((int)DataOperationType.Notify, buffer);
            Serialize.SerI32((int)dataType, buffer);
            SerializePlayerData(dataType, buffer);
            return serverConnection.TrySendTaggedBuffer(BinaryMessageType.TaggedPlayerData, buffer, BroadcastType.Other, serverConnection.clientId);
        }

        public bool TryNotifyAllPlayerData() {
            networkBuffers[0].Clear();
            SerializeAllPlayerData(networkBuffers[0]);
            currentBufferIndex = 1;
            return serverConnection.TrySendTaggedBuffer(BinaryMessageType.AllPlayerData, networkBuffers[0], BroadcastType.Other, serverConnection.clientId);
        }

        public bool TryNotifyAllPlayerDataDiff() {
            networkBuffers[currentBufferIndex].Clear();
            SerializeAllPlayerData(networkBuffers[currentBufferIndex]);
            var oldBufferIndex = (currentBufferIndex + 1) % 2;
            if (networkBuffers[currentBufferIndex].SequenceEqual(networkBuffers[oldBufferIndex]))
                return true;
            Serialize.DiffData(networkBuffers[oldBufferIndex], networkBuffers[currentBufferIndex], diff);
            currentBufferIndex = oldBufferIndex;
            return serverConnection.TrySendTaggedBuffer(BinaryMessageType.Diff, diff, BroadcastType.Other, serverConnection.clientId);
        }

        public void ProcessMessageFromServer(ref int cursor, byte[] bufferList, int msgEnd) {
            int messageTypeInt;
            Serialize.DesI32(out messageTypeInt, ref cursor, bufferList);
            var messageType = (ServerToClientMessageType)messageTypeInt;

            switch (messageType) {
                case ServerToClientMessageType.BinaryMessageFrom: {
                    ushort senderId;
                    Serialize.DesU16(out senderId, ref cursor, bufferList);

                    ProcessBinaryMessageFromServer(senderId, ref cursor, bufferList);
                    break;
                }
                case ServerToClientMessageType.ClientDisconnected: {
                    ushort clientId;
                    Serialize.DesU16(out clientId, ref cursor, bufferList);

                    RemoveGhost(clientId);
                    break;
                }
                case ServerToClientMessageType.Hello: {
                    Serialize.DesU16(out serverConnection.clientId, ref cursor, bufferList);

                    uint factCount;
                    Serialize.DesU32(out factCount, ref cursor, bufferList);

                    for (int i = 0; i < factCount; i++) {
                        byte room;
                        Serialize.DesU8(out room, ref cursor, bufferList);

                        ushort creatorId;
                        Serialize.DesU16(out creatorId, ref cursor, bufferList);

                        ushort interatibleIndex;
                        Serialize.DesU16(out interatibleIndex, ref cursor, bufferList);

                        int len;
                        Serialize.DesI32(out len, ref cursor, bufferList);

                        var data = bufferList[cursor..(cursor+len)];
                        ProcessDataNotify(room, creatorId, interatibleIndex, data);

                        cursor += len;
                    }

                    serverConnection.networkInitialized = true;
                    serverConnection.TrySendSetClientType();
                    
                    TryNotifyAllPlayerData();
                    break;
                }
                case ServerToClientMessageType.ClientConnected: {
                    ushort clientId;
                    if (Serialize.DesU16(out clientId, ref cursor, bufferList)) {
                        Debug.Log("New Client joined with id: " + clientId);
                        TryNotifyAllPlayerData();
                    }
                    else {
                        Debug.Log("Problem");
                    }
                    break;
                }
                case ServerToClientMessageType.DataNotify: {
                    byte room;
                    if (!Serialize.DesU8(out room, ref cursor, bufferList)) {
                        Debug.Log("Problem");
                        return;
                    }
                    ushort creator_id;
                    if (!Serialize.DesU16(out creator_id, ref cursor, bufferList)) {
                        Debug.Log("Problem");
                        return;
                    }
                    ushort index;
                    if (!Serialize.DesU16(out index, ref cursor, bufferList)) {
                        Debug.Log("Problem");
                        return;
                    }
                    var data = bufferList[cursor..msgEnd];
                    ProcessDataNotify(room, creator_id, index, data);
                    cursor += data.Length;
                    break;
                }
                case ServerToClientMessageType.DataOwner: {
                    byte room;
                    if (!Serialize.DesU8(out room, ref cursor, bufferList)) {
                        Debug.Log("Problem");
                        return;
                    }
                    ushort creator_id;
                    if (!Serialize.DesU16(out creator_id, ref cursor, bufferList)) {
                        Debug.Log("Problem");
                        return;
                    }
                    ushort index;
                    if (!Serialize.DesU16(out index, ref cursor, bufferList)) {
                        Debug.Log("Problem");
                        return;
                    }
                    ushort owner;
                    if (!Serialize.DesU16(out owner, ref cursor, bufferList)) {
                        Debug.Log("Problem");
                        return;
                    }
                    ProcessDataOwner(room, creator_id, index, owner);
                    break;
                }
                default:
                    Debug.Log("Unhandeled message type: " + messageType);
                    break;
            }
        }

        public void RemoveGhost(int clientId) {
            PlayerGhost ghost;
            if (ghostSystem.ghosts.TryGetValue(clientId, out ghost)) {
                Destroy(ghost.playerIndicator.gameObject);
                if (ghost.handsParentL)
                    Destroy(ghost.handsParentL.gameObject);
                if (ghost.handsParentR)
                    Destroy(ghost.handsParentR.gameObject);
            }
            ghostSystem.ghosts.Remove(clientId);
        }

        public void ProcessPlayerDataMessage(int senderId, ref int cursor, byte[] bufferList) {
            int dataOperationIndex;
            if (!Serialize.DesI32(out dataOperationIndex, ref cursor, bufferList)) {
                Debug.Log("Problem");
                return;
            }
            var dataOperation = (DataOperationType)dataOperationIndex;

            int dataTypeIndex;
            if (!Serialize.DesI32(out dataTypeIndex, ref cursor, bufferList)) {
                Debug.Log("Problem");
                return;
            }
            var dataType = (PlayerDataType)dataTypeIndex;

            switch (dataOperation) {
                case DataOperationType.Notify:
                    ProcessPlayerDataNotify(senderId, dataType, ref cursor, bufferList);
                    break;
                case DataOperationType.Set:
                    ProcessPlayerDataSet(dataType, ref cursor, bufferList);
                    break;
                case DataOperationType.Request:
                    ProcessPlayerDataRequest(dataType, ref cursor, bufferList);
                    break;
            }
        }

        public void ProcessPlayerDataRequest(PlayerDataType dataType, ref int cursor, byte[] bufferList) {
            TryNotifyPlayerData(dataType);
        }

        public void ProcessPlayerDataSet(PlayerDataType dataType, ref int cursor, byte[] buffer) {
            Debug.Log("Try set Player Data");
            switch (dataType) {
                case PlayerDataType.Color: {
                    Color color;
                    if (!Serialize.DesColor(out color, ref cursor, buffer)) {
                        Debug.Log("Problem Deserializing Color");
                        return;
                    }

                    if (config.color == color) {
                        Debug.Log("Color matches the one we already have, bailing");
                        break;
                    }
                    config.color = color;
                    Debug.Log("Saving Color To Config");
                    config.Save();
                    if (serverConnection.networkInitialized) {
                        Debug.Log("Notifying other players of color change");
                    }
                    break;
                }
                case PlayerDataType.Language: {
                    int languageIndex;
                    if (!Serialize.DesI32(out languageIndex, ref cursor, buffer)) {
                        Debug.Log("Problem Setting Language");
                        return;
                    }
                    var language = (Language)languageIndex;
                    Player.ThePlayer.language = language;
                    foreach (var room in RoomManager.TheRoomManager.rooms) {
                        if (room) {
                            room.UpdateLanguage(language);
                        }
                    }
                    break;
                }
                case PlayerDataType.EnvironmentData: {
                    EnvData envData;
                    if (!Serialize.DesString(out envData.name, ref cursor, buffer)) {
                        Debug.Log("Problem Setting env Name");
                        return;
                    }
                    if (!Serialize.DesString(out envData.code, ref cursor, buffer)) {
                        Debug.Log("Problem Setting env Code");
                        return;
                    }
                    if (!Serialize.DesVector3(out envData.pos, ref cursor, buffer)) {
                        Debug.Log("Problem Setting env Pos");
                        return;
                    }
                    if (!Serialize.DesVector3(out envData.euler, ref cursor, buffer)) {
                        Debug.Log("Problem Setting env Euler");
                        return;
                    }
                    var isSame = config.environmentData.name == envData.name
                              && config.environmentData.code == envData.code
                              && config.environmentData.pos == envData.pos
                              && config.environmentData.euler == envData.euler;
                    if (!isSame) {
                        config.environmentData = envData;
                        Debug.Log("Saving new environment data");
                        config.Save();
                        var envCodeConfig = Player.ThePlayer.environmentCodeConfig;
                        if (envCodeConfig) {
                            envCodeConfig.Reload();
                            envCodeConfig.transform.position = envData.pos;
                            envCodeConfig.transform.rotation = Quaternion.Euler(envData.euler);
                        }
                    }
                    break;
                }
                case PlayerDataType.DevMode: {
                    Debug.Log("Try set DevMode");
                    bool devMode;
                    if (!Serialize.DesBool(out devMode, ref cursor, buffer)) {
                        Debug.Log("Problem setting DevMode");
                        return;
                    }
                    DeveloperMode.TheDeveloperMode.Set(devMode);
                    break;
                }
                case PlayerDataType.IsVisible: {
                    Debug.Log("Try set IsVisible");
                    if (!Serialize.DesBool(out isVisible, ref cursor, buffer)) {
                        Debug.Log("Problem");
                        return;
                    }
                    break;
                }
                case PlayerDataType.Level: {
                    Debug.Log("Try set Level");
                    float level;
                    if (!Serialize.DesFloat(out level, ref cursor, buffer)) {
                        Debug.Log("Problem");
                        return;
                    }
                    Player.ThePlayer.currentLevelIndex = (int)level;

                    break;
                }
                case PlayerDataType.AudioVolume: {
                    Debug.Log("Try set AudioVolume");
                    if (!Serialize.DesFloat(out float audio_volume, ref cursor, buffer)) {
                        Debug.Log("Problem");
                        return;
                    }
                    VolumeService.SystemVolume = audio_volume;
                    break;
                }
                default:
                    Debug.Log($"unhandled data type: {dataType}");
                    break;
            }
        }

        public void ProcessPlayerDataNotify(int senderId, PlayerDataType dataType, ref int cursor, byte[] bufferList) {
            var ghost = ghostSystem.GetGhost(senderId);
            ghost.ProcessPlayerDataNotify(dataType, ref cursor, bufferList, ghostSystem);
        }

        public void ProcessBinaryMessageFromServer(int senderId, ref int cursor, byte[] bufferList) {
            int messageTypeInt;
            Serialize.DesI32(out messageTypeInt, ref cursor, bufferList);
            var messageType = (BinaryMessageType)messageTypeInt;

            switch (messageType) {
                case BinaryMessageType.TaggedPlayerData: {
                    ProcessPlayerDataMessage(senderId, ref cursor, bufferList);
                    break;
                }
                case BinaryMessageType.Ping: {
                    break;
                }
                case BinaryMessageType.AllPlayerData: {
                    UpdateAllPlayerData(senderId, ref cursor, bufferList);
                    break;
                }
                case BinaryMessageType.Diff: {
                    UpdateAllPlayerDataDiff(senderId, ref cursor, bufferList);
                    break;
                }
                default:
                    Debug.Log("Unhandeled Binary Message Type: " + messageType);
                    break;
            }
        }

        private void OnDestroy() {
            serverConnection.Close();
        }

        void OnApplicationPause(bool pauseStatus) {
            if (pauseStatus) {
                ForgetServer();
            }
            else {
                reconnectTimer = 0;
            }
        }
    }
}
