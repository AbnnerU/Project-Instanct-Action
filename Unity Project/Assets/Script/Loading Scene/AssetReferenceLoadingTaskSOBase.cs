using System;
using UnityEngine.AddressableAssets;

namespace Scripts.LoadScene
{
    [Serializable]
    public class AssetReferenceLoadingTaskSOBase : AssetReferenceT<LoadingTaskSOBase>
    {
        public AssetReferenceLoadingTaskSOBase(string guid) : base(guid)
        {
        }
    }
}