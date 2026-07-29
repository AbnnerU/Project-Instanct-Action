using UnityEngine;

namespace Scripts.Combat
{
    public readonly struct DamageResult
    {
        public readonly DamageType DamageType;
        public readonly float RawDamage;
        public readonly float FinalDamage;
        public readonly bool Killed;
        public readonly bool IsCritical;
        public readonly bool WasApplied;
        public readonly DamageRequest DamageRequest;

        public GameObject Attacker => DamageRequest.Attacker;
        public GameObject Receiver => DamageRequest.Receiver;
        public Collider HitCollider => DamageRequest.HitCollider;
        public Vector3 HitPoint => DamageRequest.HitPoint;

        public DamageResult(DamageType damageType, float rawDamage, float finalDamage, bool killed, bool isCritical, bool wasApplied, DamageRequest damageRequest)
        {
            DamageType = damageType;
            RawDamage = rawDamage;
            FinalDamage = finalDamage;
            Killed = killed;
            IsCritical = isCritical;
            WasApplied = wasApplied;
            DamageRequest = damageRequest;
        }

        public DamageResult(DamageType damageType, float finalDamage, bool killed, DamageRequest damageRequest)
            : this(damageType, finalDamage, finalDamage, killed, false, true, damageRequest)
        {
        }

        public static DamageResult Failed(DamageRequest damageRequest)
        {
            DamageType damageType = damageRequest.DamageData != null ? damageRequest.DamageData.DamageType : DamageType.TRUE;
            return new DamageResult(damageType, 0f, 0f, false, false, false, damageRequest);
        }
    }
}
