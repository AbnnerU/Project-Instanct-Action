using Scripts.Managers;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Combat
{
    public static class DamageResolver
    {
        public static ResolvedDamageData Resolve(DamageData damageData, MN_PlayerStatus playerStatus, float resistancePercentage = 0f)
        {
            if (damageData == null)
            {
                Debug.LogError("DamageData is null.");
                return new ResolvedDamageData(DamageType.TRUE, 0f);
            }

            float physicForce = playerStatus != null ? playerStatus.GetCurrentValue(StatusType.PHYSIC_FORCE) : 0f;
            float magicForce = playerStatus != null ? playerStatus.GetCurrentValue(StatusType.MAGIC_FORCE) : 0f;

            float rawDamage = damageData.BaseDamage +
                              physicForce * damageData.PhysicForceScaling +
                              magicForce * damageData.MagicForceScaling;

            float finalDamage = ApplyResistance(rawDamage, damageData.DamageType, resistancePercentage);

            return new ResolvedDamageData(damageData.DamageType, finalDamage);
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
        public readonly float FinalDamage;

        public ResolvedDamageData(DamageType damageType, float finalDamage)
        {
            DamageType = damageType;
            FinalDamage = finalDamage;
        }
    }
}
