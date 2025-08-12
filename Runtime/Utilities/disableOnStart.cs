using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace kaskelot {
    
public class disableOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }
    
}
}
