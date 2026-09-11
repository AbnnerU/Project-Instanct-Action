using Scripts.Status;
using UnityEngine;

namespace Scripts.Combat
{
    public static class DamageResolver
    {
        public static ResolvedDamageData Resolve(DamageData damageData, StatusInfo attackerStatus)
        {
            return Resolve(damageData, attackerStatus, null);
        }

        public static ResolvedDamageData Resolve(DamageData damageData, StatusInfo attackerStatus, StatusInfo receiverStatus)
        {
            if (damageData == null)
            {
                Debug.LogError("DamageData is null.");
                return new ResolvedDamageData(DamageType.TRUE, 0f);
            }

            float physicForce = attackerStatus != null ? attackerStatus.GetCurrentValueOf(StatusType.PHYSIC_FORCE) : 0f;
            float magicForce = attackerStatus != null ? attackerStatus.GetCurrentValueOf(StatusType.MAGIC_FORCE) : 0f;

            float rawDamage = damageData.BaseDamage +
                              (physicForce * damageData.PhysicForceScaling / 100f) +
                              (magicForce * damageData.MagicForceScaling / 100f);

            bool isCritical = RollCritical(attackerStatus);

            if (isCritical)
                rawDamage *= GetCriticalDamageMultiplier(attackerStatus);

            float resistancePercentage = GetResistancePercentage(damageData.DamageType, receiverStatus);
            float finalDamage = ApplyResistance(rawDamage, damageData.DamageType, resistancePercentage);

            return new ResolvedDamageData(damageData.DamageType, rawDamage, finalDamage, isCritical);
        }

        private static bool RollCritical(StatusInfo attackerStatus)
        {
            if (attackerStatus == null) return false;

            float criticalChance = Mathf.Clamp(attackerStatus.GetCurrentValueOf(StatusType.CRITICAL_CHANCE), 0f, 100f) / 100f;
            return criticalChance > 0f && Random.value <= criticalChance;
        }

        private static float GetCriticalDamageMultiplier(StatusInfo attackerStatus)
        {
            StatusData criticalStatus = attackerStatus?.GetStatusData(StatusType.CRITICAL_DAMAGE);
            float criticalDamagePercent = criticalStatus != null ? criticalStatus.CurrentValue : 200f;
            return Mathf.Max(100f, criticalDamagePercent) / 100f;
        }

        private static float GetResistancePercentage(DamageType damageType, StatusInfo receiverStatus)
        {
            if (receiverStatus == null) return 0f;

            float resistanceValue = damageType switch
            {
                DamageType.PHYSICAL => receiverStatus.GetCurrentValueOf(StatusType.PHYSICAL_RESISTANCE),
                DamageType.MAGIC => receiverStatus.GetCurrentValueOf(StatusType.MAGIC_RESISTANCE),
                _ => 0f
            };

            return Mathf.Clamp(resistanceValue, 0f, 100f) / 100f;
        }

        private static float ApplyResistance(float damage, DamageType damageType, float resistancePercentage)
        {
            if (damageType == DamageType.TRUE) return Mathf.Max(0f, damage);

            resistancePercentage = Mathf.Clamp01(resistancePercentage);
            return Mathf.Max(0f, damage * (1f - resistancePercentage));
        }
    }

    public readonly struct ResolvedDamageData
    {
        public readonly DamageType DamageType;
        public readonly float RawDamage;
        public readonly float FinalDamage;
        public readonly bool IsCritical;

        public ResolvedDamageData(DamageType damageType, float finalDamage)
            : this(damageType, finalDamage, finalDamage, false)
        {
        }

        public ResolvedDamageData(DamageType damageType, float rawDamage, float finalDamage, bool isCritical)
        {
            DamageType = damageType;
            RawDamage = rawDamage;
            FinalDamage = finalDamage;
            IsCritical = isCritical;
        }
    }
}
