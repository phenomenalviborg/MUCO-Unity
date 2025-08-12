using System.Collections.Generic;
using Unity.VisualScripting;

namespace Muco {

    public class InteractibleComponentInt : InteractibleComponentBase
    {
        public int value;
        public override void Ser(List<byte> buffer)
        {
            Serialize.SerI32(value, buffer);
        }

        public override void Des(ref int cursor, byte[] buffer)
        {
            int newValue;
            Serialize.DesI32(out newValue, ref cursor, buffer);
            value = newValue;
        }
        public void UpdateAndSend(bool takeOwnership)
        {
            interactible.SendState(takeOwnership);
        }

        public void Increment(int val)
        {
            value += val;
            UpdateAndSend(true);
        }
    }

}