using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using Random = UnityEngine.Random;

namespace Scripts.TierManipulation.Visual
{
    [CreateAssetMenu(fileName = "CreatureEvolutionDataSO", menuName = "Scriptable Objects/Evolution/CreatureEvolutionDataSO")]
    public class CreatureEvolutionDataSO : ScriptableObject
    {
        public string Id;
        public string DisplayName;

        [Tooltip("Arraste o Prefab do Corpo Addressable aqui")]
        public AssetReferenceGameObject VisualPrefabRef;

        public List<TextureVariantSO> List_TextureVariants = new List<TextureVariantSO>();


        public TextureVariantSO GetRandomTexture()
        {
            if (List_TextureVariants.Count == 0)
                return null;

            return List_TextureVariants[Random.Range(0, List_TextureVariants.Count)];
        }

#if UNITY_EDITOR
        private void CreateID()
        {
            Guid guid = Guid.NewGuid();
            Id = guid.ToString();

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}