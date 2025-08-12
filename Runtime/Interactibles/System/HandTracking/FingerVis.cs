using UnityEngine;
using Muco;

public class FingerVis : MonoBehaviour
{
    public Muco.SetColor setColor;

    public void SetVisual(bool isPinched, FingerProvider.FingerState fingerState)
    {
        Color color = Color.white;
        if (isPinched && fingerState != FingerProvider.FingerState.Movable) {
            color = Color.red;
        }else {
            switch (fingerState) {
                case FingerProvider.FingerState.Default:
                    break;
                case FingerProvider.FingerState.Dragging:
                    color = Color.blue;
                    break;
                case FingerProvider.FingerState.Movable:
                    color = Color.green;
                    break;
            }
        }
        setColor.Set(color);
    }

    void Awake() {
        var platform = PlatformDetection.ThePlatform;
        if (platform == Platform.WindowsEditor | platform == Platform.OsxEditor) {
            gameObject.SetActive(false);
        }
        else  {
            DeveloperMode.TheDeveloperMode.Register(DeveloperModeCallback);
        }
    }

    public void DeveloperModeCallback(bool isOn) {
        gameObject.SetActive(isOn);
    }
}
