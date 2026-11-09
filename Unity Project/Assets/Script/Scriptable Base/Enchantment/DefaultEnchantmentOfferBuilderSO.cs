using System;
using System.Collections.Generic;
using Scripts.Abilities;
using Scripts.TierManipulation;
using Scripts.Weapon;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Enchantment
{
    [CreateAssetMenu(fileName = "DefaultEnchantmentOfferBuilderSO",
        menuName = "Scriptable Objects/Enchantment/Default Offer Builder")]
    public class DefaultEnchantmentOfferBuilderSO : EnchantmentOfferBuilderSOBase
    {
        [Serializable]
        private class TierChance
        {
            public Tier Tier;
            [Min(0)] public float Chance;

            public TierChance(Tier tier, float chance)
            {
                Tier = tier;
                Chance = chance;
            }
        }

        [Tooltip("Relative chances: 50/30/15/5 means 50%/30%/15%/5%. Empty tiers are excluded; zero disables a tier.")]
        [SerializeField] private List<TierChance> _list_TierChances = new List<TierChance>
        {
            new TierChance(Tier.TIER_1, 50),
            new TierChance(Tier.TIER_2, 30),
            new TierChance(Tier.TIER_3, 15),
            new TierChance(Tier.TIER_4, 5)
        };

        public override bool CanAcquire(EnchantmentSOBase enchantment, IReadOnlyList<EnchantmentSOBase> history)
        {
            if (enchantment == null) return false;

            if (enchantment is WeaponEnchantmentSO weapon)
                return MN_Weapon.Instance != null && weapon.WeaponLoadout != null &&
                    MN_Weapon.Instance.CurrentLoadout != weapon.WeaponLoadout;

            if (enchantment is AbilityEnchantmentSO ability)
                return MN_Abilities.Instance != null && ability.AbilityPrefabReference != null &&
                    ability.AbilityPrefabReference.RuntimeKeyIsValid() &&
                    !MN_Abilities.Instance.HasAbility(ability.AbilityPrefabReference);

            // Specific upgrades need ownership prerequisites before they can be offered.
            if (enchantment.EnchantmentType == EnchantmentType.ABILITY_UPGRADE ||
                enchantment.EnchantmentType == EnchantmentType.WEAPON_UPGRADE) return false;

            if (enchantment.CanRepeat || history == null) return true;
            for (int i = 0; i < history.Count; i++)
                if (IsSameEnchantment(enchantment, history[i])) return false;
            return true;
        }

        public override List<EnchantmentSOBase> Build(EnchantmentsDeckConfig config, IReadOnlyList<EnchantmentSOBase> availableEnchantments,  IReadOnlyList<EnchantmentSOBase> history, int amount)
        {
            var result = new List<EnchantmentSOBase>();
            if (config == null || availableEnchantments == null || amount <= 0) return result;

            var candidates = new List<EnchantmentSOBase>();
            for (int i = 0; i < availableEnchantments.Count; i++)
            {
                EnchantmentSOBase enchantment = availableEnchantments[i];
                if (CanAcquire(enchantment, history) && config.Allows(enchantment.EnchantmentType) &&
                    !candidates.Exists(item => IsSameEnchantment(item, enchantment)))
                    candidates.Add(enchantment);
            }

            while (result.Count < amount && TrySelectTier(candidates, out Tier tier))
            {
                var tierCandidates = candidates.FindAll(item => item.Tier == tier);
                EnchantmentSOBase selected = tierCandidates[Random.Range(0, tierCandidates.Count)];
                result.Add(selected);
                candidates.RemoveAll(item => ConflictsWith(item, selected));
            }
            return result;
        }

        public override EnchantmentSOBase BuildReplacement(EnchantmentsDeckConfig config,
            IReadOnlyList<EnchantmentSOBase> availableEnchantments,
            IReadOnlyList<EnchantmentSOBase> history,
            IReadOnlyList<EnchantmentSOBase> currentOffer, int replacedIndex)
        {
            if (availableEnchantments == null || currentOffer == null ||
                replacedIndex < 0 || replacedIndex >= currentOffer.Count) return null;

            var candidates = new List<EnchantmentSOBase>();
            for (int i = 0; i < availableEnchantments.Count; i++)
            {
                EnchantmentSOBase candidate = availableEnchantments[i];
                bool conflicts = IsSameEnchantment(candidate, currentOffer[replacedIndex]);
                for (int j = 0; j < currentOffer.Count && !conflicts; j++)
                    if (j != replacedIndex) conflicts = ConflictsWith(candidate, currentOffer[j]);

                if (!conflicts) candidates.Add(candidate);
            }

            List<EnchantmentSOBase> replacement = Build(config, candidates, history, 1);
            return replacement.Count > 0 ? replacement[0] : null;
        }

        private static bool ConflictsWith(EnchantmentSOBase candidate, EnchantmentSOBase selected) =>
            IsSameEnchantment(candidate, selected) ||
            (selected is WeaponEnchantmentSO && candidate is WeaponEnchantmentSO) ||
            (selected is AbilityEnchantmentSO && candidate is AbilityEnchantmentSO);

        private bool TrySelectTier(List<EnchantmentSOBase> candidates, out Tier tier)
        {
            tier = default;
            if (_list_TierChances == null) return false;

            float total = 0;
            foreach (TierChance entry in _list_TierChances)
            {
                if (!HasCandidates(entry, candidates)) continue;
                total += entry.Chance;
            }
            if (total <= 0) return false;

            float roll = Random.value * total;
            foreach (TierChance entry in _list_TierChances)
            {
                if (!HasCandidates(entry, candidates)) continue;
                tier = entry.Tier;
                roll -= entry.Chance;
                if (roll < 0) return true;
            }
            // Random.value can return 1; keep the last eligible tier at that boundary.
            return true;
        }

        private static bool HasCandidates(TierChance entry, List<EnchantmentSOBase> candidates) =>
            entry != null && entry.Chance > 0 && !float.IsInfinity(entry.Chance) &&
            candidates.Exists(item => item.Tier == entry.Tier);

        private static bool IsSameEnchantment(EnchantmentSOBase first, EnchantmentSOBase second) =>
            first != null && second != null && (first == second ||
                (!string.IsNullOrEmpty(first.ID) && first.ID == second.ID));
    }
}
