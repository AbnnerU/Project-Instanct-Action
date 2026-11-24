using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "TextureVariantSO", menuName = "Scriptable Objects/Texture/TextureVariantSO")]
public class TextureVariantSO : ScriptableObject
{
    public string ID;
    public string DisplayName;

    [Tooltip("Arraste o Material Addressable aqui")]
    public AssetReferenceT<Material> MaterialRef;

    public AssetReferenceSprite IconRef;

#if UNITY_EDITOR
    private void CreateID()
    {
        Guid guid = Guid.NewGuid();
        ID = guid.ToString();

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
