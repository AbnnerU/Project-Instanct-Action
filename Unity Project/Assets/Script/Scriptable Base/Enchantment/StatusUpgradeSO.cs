using System;
using System.Collections.Generic;
using Scripts.Managers;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Enchantment
{
    [CreateAssetMenu(fileName = "StatusUpgradeSO", menuName = "Scriptable Objects/Enchantment/Status Upgrade")]
    public class StatusUpgradeSO : EnchantmentSOBase
    {
        [SerializeField] private List<StatusEnchantmentModifier> _list_StatusModifiers = new List<StatusEnchantmentModifier>();

        public IReadOnlyList<StatusEnchantmentModifier> List_StatusModifiers => _list_StatusModifiers;

        public override void Apply(EnchantmentContext context)
        {
            if (context == null || context.PlayerStatus == null)
                return;

            for (int i = 0; i < _list_StatusModifiers.Count; i++)
                _list_StatusModifiers[i].Apply(context.PlayerStatus);
        }

        public override string GetResolvedDescription()
        {
            return EnchantmentDescriptionFormatter.ResolveStatusUpgradeDescription(this, Description);
        }
    }

    [Serializable]
    public class StatusEnchantmentModifier
    {
        public StatusType StatusType;
        public EnchantmentValueType ValueType;
        public float Value;

        public void Apply(MN_PlayerStatus playerStatus)
        {
            if (playerStatus == null)
                return;

            float currentValue = playerStatus.GetCurrentValue(StatusType);
            float valueToAdd = GetValueToAdd(currentValue);

            playerStatus.AddCurrentValue(StatusType, valueToAdd);
        }

        private float GetValueToAdd(float currentValue)
        {
            return ValueType == EnchantmentValueType.PERCENT_OF_CURRENT
                ? currentValue * (Value / 100f)
                : Value;
        }
    }

    public enum EnchantmentValueType
    {
        FLAT = 0,
        PERCENT_OF_CURRENT = 1
    }
}
