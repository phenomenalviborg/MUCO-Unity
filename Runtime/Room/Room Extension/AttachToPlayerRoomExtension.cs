using UnityEngine;
using Muco;

public class AttachToPlayerRoomExtension : RoomExtension {
    public GameObject preFab;
    public GameObject attachedObject;

    public override void Enter() {
        attachedObject = Instantiate(preFab, Player.ThePlayer.head.transform.position, Player.ThePlayer.head.transform.rotation);
        attachedObject.transform.parent = Player.ThePlayer.head;
    }
    public override void Exit() {
        Debug.Log("Invoked exit!");
        Destroy(attachedObject);
    }
}
