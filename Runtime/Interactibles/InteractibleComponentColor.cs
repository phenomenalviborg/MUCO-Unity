using System.Collections.Generic;
using UnityEngine;

namespace Muco {
    public class InteractibleComponentColor : InteractibleComponentBase {
        public Renderer[] renderers;
        public Color color;

        public override void Init() {
            CheckForRenderer();
            ApplyColor();
        }

        public override void Ser(List<byte> buffer) {
            Serialize.SerColor(color, buffer);
        }
        
        public override void Des(ref int cursor, byte[] buffer) {
            Serialize.DesColor(out color, ref cursor, buffer);
            ApplyColor();
        }

        void ApplyColor() {
            foreach (var renderer in renderers) {
                renderer.material.SetColor("_Color", color);
            }
        }

        public void SetColorAndSend(Color color) {
            this.color = color;
            ApplyColor();
            interactible.SendState(true);
        }

        public void CheckForRenderer(){
            if (renderers.Length > 0)
                return;
            var renderer = GetComponent<Renderer>();
            if (renderer) {
                renderers = new Renderer[] { renderer };
            }
        }
    }
}
