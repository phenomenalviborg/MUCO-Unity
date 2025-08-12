using UnityEngine;

namespace Muco {
    public class PlayerRemoteIndicatorBase : MonoBehaviour {
        public int level;
        [HideInInspector] public RendererReference leftHandRendererReference;
        [HideInInspector] public RendererReference rightHandRendererReference;
        public virtual void Init() { }
        public virtual void SetLevel(int level) {
            this.level = level;
        }
        public virtual void SetColor(Color color) { }
    }
}
