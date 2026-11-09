using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Enchantment
{
    [Serializable]
    public class EnchantmentsDeckConfig
    {
        public List<EnchantmentType> EnchantmentsTypesAvailable = new List<EnchantmentType>
        {
            EnchantmentType.STATUS, EnchantmentType.ABILITY, EnchantmentType.WEAPON
        };
        public int Rolls;
        private int AdditionalRolls;
        [SerializeField] private bool _allowSkip = true;
        public bool AllowSkip => _allowSkip;

        public bool Allows(EnchantmentType type) => EnchantmentsTypesAvailable != null && EnchantmentsTypesAvailable.Contains(type);

        public void AddRoll(int amount) => AdditionalRolls += Mathf.Max(0, amount);

        public int GetRollsAmount()
        {
            return Mathf.Max(0, Rolls) + ConsumeAdditionalRolls();
        }

        public int ConsumeAdditionalRolls()
        {
            int value = AdditionalRolls;
            AdditionalRolls = 0;
            return value;
        }
    }
}
