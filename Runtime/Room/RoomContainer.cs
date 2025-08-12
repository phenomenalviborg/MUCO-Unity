using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Muco
{
    [Serializable]
    public class RoomContainer
    {
        public AssetReferenceGameObject roomAsset;
        public TextAsset JSONFile;
    }
}
