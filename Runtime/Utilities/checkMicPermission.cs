using UnityEngine;
using UnityEngine.Android;

namespace Muco {
    
public class checkMicPermission : MonoBehaviour
{
    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
    {
        Debug.Log($"{permissionName} PermissionDeniedAndDontAskAgain");
    }

    internal void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        Debug.Log($"{permissionName} PermissionCallbacks_PermissionGranted");
    }

    internal void PermissionCallbacks_PermissionDenied(string permissionName)
    {
        Debug.Log($"{permissionName} PermissionCallbacks_PermissionDenied");
    }

    void Start()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            // The user authorized use of the microphone.
            Debug.Log("The user has microphone permission");
        }
        else
        {
            Debug.Log("The user does not have microphone permission, requesting.");
            bool useCallbacks = false;
            if (!useCallbacks)
            {
                // We do not have permission to use the microphone.
                // Ask for permission or proceed without the functionality enabled.
                Permission.RequestUserPermission(Permission.Microphone);
            }
            else
            {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
                callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
                callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
                Permission.RequestUserPermission(Permission.Microphone, callbacks);
            }
        }
    }
}
}
