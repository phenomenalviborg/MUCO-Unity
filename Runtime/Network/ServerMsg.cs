namespace Muco {
    public enum ClientType {
        Player,
        Manager,
    }

    public enum ClientToServerMessageType {
        Disconnect,
        BroadcastBytesAll,
        BroadcastBytesOther,
        BinaryMessageTo,
        SetClientType,
        Kick,
        SetData,
        ClaimData,
    }

    public enum ServerToClientMessageType {
        Hello,
        ClientConnected,
        ClientDisconnected,
        BinaryMessageFrom,
        DataNotify,
        DataOwner,
    }

    public enum PlayerDataType {
        DeviceId,
        Color,
        Trans,
        Level,
        Hands,
        Language,
        EnvironmentData,
        DevMode,
        IsVisible,
        DeviceStats,
        AudioVolume,
        Count,
    }

    public enum HandType {
        OpenXr
    }

    public enum DataOperationType {
        Notify,
        Set,
        Request,
    }

    public enum BinaryMessageType {
        TaggedPlayerData,
        Ping,
        AllPlayerData,
        Diff,
    }

    public enum BroadcastType {
        All,
        Other,
        Spesific,
    }
}
