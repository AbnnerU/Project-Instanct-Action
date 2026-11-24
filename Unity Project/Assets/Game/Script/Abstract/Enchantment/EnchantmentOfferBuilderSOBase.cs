using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Enchantment
{
    public abstract class EnchantmentOfferBuilderSOBase : ScriptableObject
    {
        public abstract EnchantmentSOBase BuildReplacement(EnchantmentsDeckConfig config,
            IReadOnlyList<EnchantmentSOBase> availableEnchantments,
            IReadOnlyList<EnchantmentSOBase> history,
            IReadOnlyList<EnchantmentSOBase> currentOffer, int replacedIndex);

        public abstract bool CanAcquire(EnchantmentSOBase enchantment,
            IReadOnlyList<EnchantmentSOBase> history);

        public abstract List<EnchantmentSOBase> Build(EnchantmentsDeckConfig config,
            IReadOnlyList<EnchantmentSOBase> availableEnchantments,
            IReadOnlyList<EnchantmentSOBase> history, int amount);
    }
}
