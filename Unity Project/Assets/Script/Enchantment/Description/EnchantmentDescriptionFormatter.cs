using System.Globalization;
using System.Text.RegularExpressions;
using Scripts.Abilities;
using Scripts.Combat;
using Scripts.Status;
using Scripts.Weapon;
using UnityEngine;

namespace Scripts.Enchantment
{
    public static class EnchantmentDescriptionFormatter
    {
        private static readonly Regex _indexedTagRegex = new Regex(
            @"<\s*(?<tag>[a-zA-Z]+)\s+index\s*=\s*""?(?<index>\d+)""?\s*>",
            RegexOptions.Compiled);

        #region Public

        public static string Resolve(EnchantmentSOBase enchantment, string description)
        {
            if (enchantment == null)
                return description;

            return enchantment switch
            {
                StatusUpgradeSO statusUpgrade => ResolveStatusUpgradeDescription(statusUpgrade, description),
                AbilityEnchantmentSO abilityEnchantment => ResolveAbilityDescription(abilityEnchantment, description),
                WeaponEnchantmentSO weaponEnchantment => ResolveWeaponDescription(weaponEnchantment, description),
                _ => description
            };
        }

        public static string ResolveStatusUpgradeDescription(StatusUpgradeSO enchantment, string description)
        {
            return ReplaceIndexedTags(description, (tag, index) =>
            {
                if (!IsTag(tag, "statusmodifier"))
                    return null;

                if (enchantment == null || !TryGet(enchantment.List_StatusModifiers, index, out StatusEnchantmentModifier modifier))
                    return GetInvalidTagText(tag, index);

                return FormatStatusModifier(modifier);
            });
        }

        public static string ResolveAbilityDescription(AbilityEnchantmentSO enchantment, string description)
        {
            return ReplaceIndexedTags(description, (tag, index) =>
            {
                if (IsTag(tag, "damage"))
                {
                    if (enchantment == null || !TryGet(enchantment.List_DamageDatas, index, out DamageData damageData))
                        return GetInvalidTagText(tag, index);

                    return FormatDamage(damageData);
                }

                if (IsTag(tag, "buff"))
                {
                    if (enchantment == null || !TryGet(enchantment.List_BuffDatas, index, out BuffData buffData))
                        return GetInvalidTagText(tag, index);

                    return FormatBuff(buffData);
                }

                if (IsTag(tag, "buffduration"))
                {
                    if (enchantment == null || !TryGet(enchantment.List_BuffDatas, index, out BuffData buffData))
                        return GetInvalidTagText(tag, index);

                    return FormatDuration(buffData.Duration);
                }

                return null;
            });
        }

        public static string ResolveWeaponDescription(WeaponEnchantmentSO enchantment, string description)
        {
            return ReplaceIndexedTags(description, (tag, index) =>
            {
                if (!IsTag(tag, "attackdamage"))
                    return null;

                ComboAttackSetSO attackSet = enchantment != null && enchantment.WeaponLoadout != null
                    ? enchantment.WeaponLoadout.ComboAttackSet
                    : null;

                if (attackSet == null || !TryGet(attackSet.List_ComboAttackData, index, out ComboAttackData attackData) || attackData == null)
                    return GetInvalidTagText(tag, index);

                return FormatDamage(attackData.DamageData);
            });
        }

        public static string FormatDamage(DamageData damageData)
        {
            if (damageData == null)
                return "[damage]";

            string baseDamage = FormatDamageBase(damageData);
            string scaling = FormatDamageScaling(damageData);

            if (string.IsNullOrEmpty(scaling))
                return baseDamage;

            if (Mathf.Approximately(damageData.BaseDamage, 0f))
                return scaling;

            return $"{baseDamage} + ({scaling})";
        }

        public static string FormatBuff(BuffData buffData)
        {
            if (buffData == null)
                return "[buff]";

            return $"{GetStatusSpriteTag(buffData.StatusType)} {FormatSignedValue(buffData.Value, IsPercent(buffData.ValueType))}";
        }

        public static string FormatStatusModifier(StatusEnchantmentModifier modifier)
        {
            if (modifier == null)
                return "[statusmodifier]";

            return $"{GetStatusSpriteTag(modifier.StatusType)} {FormatSignedValue(modifier.Value, IsPercent(modifier.ValueType))}";
        }

        #endregion

        #region Replace

        private static string ReplaceIndexedTags(string description, System.Func<string, int, string> replaceCallback)
        {
            if (string.IsNullOrEmpty(description))
                return description;

            return _indexedTagRegex.Replace(description, match =>
            {
                string tag = match.Groups["tag"].Value;

                if (!int.TryParse(match.Groups["index"].Value, out int index))
                    return match.Value;

                string replacement = replaceCallback?.Invoke(tag, index);
                return string.IsNullOrEmpty(replacement) ? match.Value : replacement;
            });
        }

        private static bool IsTag(string tag, string expectedTag)
        {
            return string.Equals(tag, expectedTag, System.StringComparison.OrdinalIgnoreCase);
        }

        private static string GetInvalidTagText(string tag, int index)
        {
            return $"[{tag} {index}]";
        }

        private static bool TryGet<T>(System.Collections.Generic.IReadOnlyList<T> list, int index, out T value)
        {
            value = default;

            if (list == null || index < 0 || index >= list.Count)
                return false;

            value = list[index];
            return value != null;
        }

        #endregion

        #region Damage

        private static string FormatDamageBase(DamageData damageData)
        {
            string color = GetDamageColor(damageData.DamageType);
            return $"<color={color}>{FormatNumber(damageData.BaseDamage)}</color>";
        }

        private static string FormatDamageScaling(DamageData damageData)
        {
            System.Collections.Generic.List<string> list_ScalingTexts = new System.Collections.Generic.List<string>();

            if (!Mathf.Approximately(damageData.PhysicForceScaling, 0f))
                list_ScalingTexts.Add($"{GetStatusSpriteTag(StatusType.PHYSIC_FORCE)} {FormatPercent(damageData.PhysicForceScaling * 100f)}");

            if (!Mathf.Approximately(damageData.MagicForceScaling, 0f))
                list_ScalingTexts.Add($"{GetStatusSpriteTag(StatusType.MAGIC_FORCE)} {FormatPercent(damageData.MagicForceScaling * 100f)}");

            return string.Join(" + ", list_ScalingTexts);
        }

        private static string GetDamageColor(DamageType damageType)
        {
            return damageType switch
            {
                DamageType.PHYSICAL => "#F03B18",
                DamageType.MAGIC => "#6A5CFF",
                DamageType.TRUE => "#F2F2F2",
                _ => "#FFFFFF"
            };
        }

        #endregion

        #region Status

        private static string GetStatusSpriteTag(StatusType statusType)
        {
            return $"<sprite name=\"{GetStatusSpriteName(statusType)}\">";
        }

        private static string GetStatusSpriteName(StatusType statusType)
        {
            return statusType switch
            {
                StatusType.HEALTH => "HealthIcon",
                StatusType.ATTACK_SPEED => "AttackSpeedIcon",
                StatusType.VAMP => "VampIcon",
                StatusType.PHYSIC_FORCE => "PhysicalForceIcon",
                StatusType.MAGIC_FORCE => "MagicForceIcon",
                StatusType.CRITICAL_CHANCE => "CriticalChanceIcon",
                StatusType.CRITICAL_DAMAGE => "CriticalDamageIcon",
                StatusType.STAMINA => "StaminaIcon",
                StatusType.LIFE_STEAL => "LifeStealIcon",
                StatusType.MAGIC_RESISTANCE => "MagicResistanceIcon",
                StatusType.PHYSICAL_RESISTANCE => "PhysicalResistanceIcon",
                _ => $"{statusType}Icon"
            };
        }

        #endregion

        #region Values

        private static bool IsPercent(EnchantmentValueType valueType)
        {
            return valueType == EnchantmentValueType.PERCENT_OF_CURRENT;
        }

        private static bool IsPercent(AbilityValueType valueType)
        {
            return valueType == AbilityValueType.PERCENT_OF_CURRENT;
        }

        private static string FormatSignedValue(float value, bool percent)
        {
            string sign = value > 0f ? "+" : string.Empty;
            return percent ? $"{sign}{FormatPercent(value)}" : $"{sign}{FormatNumber(value)}";
        }

        private static string FormatPercent(float value)
        {
            return $"{FormatNumber(value)}%";
        }

        private static string FormatDuration(float duration)
        {
            return $"{FormatNumber(duration)}s";
        }

        private static string FormatNumber(float value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
