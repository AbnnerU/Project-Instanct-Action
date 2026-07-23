using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scripts.Spawner
{
    public interface IMutableAddressableSpawnerWithPreview : ISpawner
    {
        void SetPrefabAndPreview(GameObject previewObject, AssetReferenceGameObject realPrefabAssetRef);

        void SetLayerMask(LayerMask layerMask);

        void SetTag(string tag);
    }
}