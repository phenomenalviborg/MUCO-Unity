using UnityEngine;
using Muco;

public class DebugFingerInteractor : MonoBehaviour
{
    public bool isPinched;
    public bool wasPinched;

    public Muco.SetColor setColor;

    private void Update()
    {
        if(isPinched && !wasPinched)
        {
            setColor.Set(Color.red);
        }
        else if(!isPinched && wasPinched)
        {
            setColor.Set(Color.white);
        }

        wasPinched = isPinched;
    }
}
