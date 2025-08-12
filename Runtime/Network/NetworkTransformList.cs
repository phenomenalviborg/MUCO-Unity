using System.Collections.Generic;
using UnityEngine;

namespace Muco
{
    public class NetworkTransformList : MonoBehaviour
    {
        public List<Transform> transforms;

        public void Ser(List<byte> buffer)
        {
            Serialize.SerTransArrLocal(transforms, buffer);
        }

        public bool Des(ref int cursor, byte[] buffer)
        {
            return Serialize.DesTransArrLocal(transforms, ref cursor, buffer);
        }
    }
}
