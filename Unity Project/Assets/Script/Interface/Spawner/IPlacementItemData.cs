using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scripts.Spawner
{
    public interface IPlacementItemData
    {
        AssetReferenceGameObject PreviewReference { get; }
        AssetReferenceGameObject RealPrefabReference { get; }

        LayerMask PlacementMask { get; }

        string PlacementTag {  get; }
    }
}