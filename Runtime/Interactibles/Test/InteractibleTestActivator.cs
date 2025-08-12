using UnityEngine;

namespace Muco
{
    public class InteractibleTestActivator : MonoBehaviour
    {
        public void OnPress()
        {
            Debug.Log("I was pressed");
        }

        public void OnSlide(float x)
        {
            Debug.Log("Slide!!" + x);
        }
    }
}
