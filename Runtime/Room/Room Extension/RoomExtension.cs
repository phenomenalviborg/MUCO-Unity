using UnityEngine;

namespace Muco
{
    public class RoomExtension : MonoBehaviour
    {
        public virtual void Init() { }
        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void UpdateLanguage(Language language) { }
    }
}
