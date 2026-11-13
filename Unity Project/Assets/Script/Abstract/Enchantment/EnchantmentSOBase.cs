using CustomInspector;
using Scripts.Status;
using Scripts.TierManipulation;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scripts.Enchantment
{
    public abstract class EnchantmentSOBase : ScriptableObject
    {
#if UNITY_EDITOR
        [Button(nameof(CreateId))]
#endif
        public string ID;
        public string DisplayName;
        public Tier Tier;
        public EnchantmentType EnchantmentType;
        [TextArea]
        public string Description;
        public AssetReferenceSprite IconReference ;
        [SerializeField] protected bool _canRepeat = true;

        public bool CanRepeat => _canRepeat;

        public abstract void Apply(EnchantmentContext context);

        public virtual string GetResolvedDescription()
        {
            return EnchantmentDescriptionFormatter.Resolve(this, Description);
        }

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
        public IHasStatusInfoReference StatusProvider { get; }
        public GameObject TargetObject { get; }
        public Transform TargetTransform { get; }

        public EnchantmentContext(IHasStatusInfoReference statusProvider, GameObject targetObject)
        {
            StatusProvider = statusProvider;
            TargetObject = targetObject;
            TargetTransform = targetObject != null ? targetObject.transform : null;
        }
    }
}
