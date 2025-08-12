using System.Collections.Generic;

namespace Muco {
    public class NetworkComponentHidden : InteractibleComponentBase {
        public bool hidden {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive(value); }
        }

        public override void Ser(List<byte> buffer) {
            Serialize.SerBool(hidden, buffer);
        }

        public override void Des(ref int cursor, byte[] buffer) {
            bool isHidden;
            Serialize.DesBool(out isHidden, ref cursor, buffer);
            hidden = isHidden;
        }

        public void SetIsHiddenAndSend(bool isHidden) {
            hidden = !isHidden;
            interactible.SendState(true);
        }

        public void ToggleIsHiddenAndSend() {
            hidden = !hidden;
            interactible.SendState(true);
        }
    }
}
