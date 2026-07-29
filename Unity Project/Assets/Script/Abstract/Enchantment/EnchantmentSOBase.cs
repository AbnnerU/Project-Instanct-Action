using CustomInspector;
using Scripts.Managers;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scripts.Enchantment
{
    public abstract class EnchantmentSOBase : ScriptableObject
    {
        [Button(nameof(CreateId))]
        public string ID;
        public string DisplayName;
        [TextArea]
        public string Description;
        public AssetReferenceSprite IconReference ;

        public abstract void Apply(EnchantmentContext context);

#if UNITY_EDITOR
        public void CreateId()
        {
            ID = Guid.NewGuid().ToString(); 
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    public class EnchantmentContext
    {
        public MN_PlayerStatus PlayerStatus { get; }
        public GameObject PlayerObject { get; }
        public Transform PlayerTransform { get; }

        public EnchantmentContext(MN_PlayerStatus playerStatus, GameObject playerObject)
        {
            PlayerStatus = playerStatus;
            PlayerObject = playerObject;
            PlayerTransform = playerObject != null ? playerObject.transform : null;
        }
    }
}
