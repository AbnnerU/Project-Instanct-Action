using Scripts.Abilities;
using Scripts.Combat;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scripts.Enchantment
{
    [CreateAssetMenu(fileName = "AbilityEnchantmentSO", menuName = "Scriptable Objects/Enchantment/Ability")]
    public class AbilityEnchantmentSO : EnchantmentSOBase
    {
        public AbilityKind AbilityKind = AbilityKind.PASSIVE;
        public AssetReferenceGameObject AbilityPrefabReference;
        public List<DamageData> List_DamageDatas;
        public List<BuffData> List_BuffDatas;


        private void OnValidate()
        {
            _canRepeat = false;
        }

        public override void Apply(EnchantmentContext context)
        {
            if (MN_PlayerAbilities.Instance == null)
                return;

            MN_PlayerAbilities.Instance.AddAbility(AbilityPrefabReference, AbilityKind, List_DamageDatas, List_BuffDatas, IconReference);
        }

        public override string GetResolvedDescription()
        {
            return EnchantmentDescriptionFormatter.ResolveAbilityDescription(this, Description);
        }
    }
}
