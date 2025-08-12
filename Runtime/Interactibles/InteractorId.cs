[System.Serializable]
public struct InteractorId
{
    public ushort user_id;
    public byte system_id;
    public byte interactor_id;

    public InteractorId(ushort user_id, byte system_id, byte interactor_id)
    {
        this.user_id = user_id;
        this.system_id = system_id;
        this.interactor_id = interactor_id;
    }

    public bool Equals(InteractorId other) {
        return user_id == other.user_id &&
               system_id == other.system_id &&
               interactor_id == other.interactor_id;
    }
}
