using UnityEngine;
using UnityEngine.Events;

namespace Muco {
    public class PlayerRemoteIndicatorBasic : PlayerRemoteIndicatorBase {
        public RendererReference rendererReference;
        public TMPro.TextMeshPro text;
        MaterialPropertyBlock propertyBlock;
        int colorPropertyId;

        [SerializeField] UnityEvent isInstantiated;

        override public void Init() {
            propertyBlock = new MaterialPropertyBlock();
            isInstantiated.Invoke();
            colorPropertyId = Shader.PropertyToID("_Color");
        }

        public override void SetColor(Color color) {
            propertyBlock.SetColor(colorPropertyId, color);
            rendererReference.SetPropertyBlock(propertyBlock);
            leftHandRendererReference?.SetPropertyBlock(propertyBlock);
            rightHandRendererReference?.SetPropertyBlock(propertyBlock);
        }
    }
}
