using System.Collections.Generic;
using UnityEngine.Events;

namespace Muco {
    public class InteractibleComponentFloat : InteractibleComponentBase
    {
        public float value;

        public UnityEvent OnValueChanged;
        public override void Ser(List<byte> buffer) {
                Serialize.SerFloat(value,buffer);
            }

            public override void Des(ref int cursor, byte[] buffer) {
                float newValue;
                Serialize.DesFloat(out newValue, ref cursor, buffer);
                value = newValue;
            }
            public void UpdateAndSend(bool takeOwnership) {
                interactible.SendState(takeOwnership);
                OnValueChanged.Invoke();
            }
    }

}