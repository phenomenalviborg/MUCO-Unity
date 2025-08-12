using UnityEngine;

namespace Muco {
    

public class SetColor : MonoBehaviour {
    public Renderer rend;

    private void Awake() {
        checkForRenderer();
    }

    public void checkForRenderer(){
        if (rend != null)
            return;
        if (rend == null)
            rend = GetComponent<Renderer>();
    }

    public void Set(Color color) {
        checkForRenderer();
        rend.material.SetColor("_Color", color);
    }
}
}
