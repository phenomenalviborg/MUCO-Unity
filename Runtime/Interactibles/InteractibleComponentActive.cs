using System.Collections.Generic;

namespace Muco {
    public class InteractibleComponentActive : InteractibleComponentBase
    {
        public bool active
        {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive(value); }
        }

        public override void Ser(List<byte> buffer)
        {
            Serialize.SerBool(active, buffer);
        }

        public override void Des(ref int cursor, byte[] buffer)
        {
            bool isActive;
            Serialize.DesBool(out isActive, ref cursor, buffer);
            active = isActive;
        }

        public void SetIsActiveAndSend(bool isActive)
        {
            if (localOnly)
            {
                active = isActive;
                return;
            }
            active = isActive;
            interactible.SendState(true);
        }

        public void ToggleIsActiveAndSend()
        {
            if (localOnly)
            {
                active = !active;
                return;
            }
            active = !active;
            interactible.SendState(true);
        }

        // Sets active on this client only, without claiming ownership or broadcasting state.
        public void SetIsActiveLocal(bool isActive)
        {
            active = isActive;
        }

    }
}
