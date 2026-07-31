using Scripts.Abilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scripts.Enchantment
{
    [CreateAssetMenu(fileName = "AbilityEnchantmentSO", menuName = "Scriptable Objects/Enchantment/Ability")]
    public class AbilityEnchantmentSO : EnchantmentSOBase
    {
        public AbilityKind AbilityKind = AbilityKind.PASSIVE;
        public AssetReferenceGameObject AbilityPrefabReference;

        private void OnValidate()
        {
            _canRepeat = false;
        }

        public override void Apply(EnchantmentContext context)
        {
            if (MN_PlayerAbilities.Instance == null)
                return;

            MN_PlayerAbilities.Instance.AddAbility(AbilityPrefabReference, AbilityKind);
        }
    }
}
