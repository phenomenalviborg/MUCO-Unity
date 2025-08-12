using UnityEngine;

namespace Muco {
    public class RendererReference : MonoBehaviour {
        public Renderer[] renderers;

        public void SetPropertyBlock(MaterialPropertyBlock propertyBlock) {
            foreach (var renderer in renderers) {
                renderer.SetPropertyBlock(propertyBlock);
            }
        }

        public void SetMaterial(Material material) {
            foreach (var renderer in renderers) {
                renderer.material = material;
            }
        }
    }
}
