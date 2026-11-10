using Scripts.Status;
using UnityEngine;

namespace Scripts.Combat
{
    public readonly struct DamageRequest
    {
        public const CriticalDamageTypes DefaultCriticalTypes = CriticalDamageTypes.PHYSICAL;
        public readonly CriticalDamageTypes AllowedCriticalTypes;
        public readonly DamageData DamageData;
        public readonly StatusInfo AttackerStatus;
        public readonly GameObject Attacker;
        public readonly GameObject Receiver;
        public readonly Collider HitCollider;
        public readonly Vector3 HitPoint;
        public readonly AttackHitData AttackHitData;
        public readonly IDamageResultReporter AttackerDamageResultReporter;

        public DamageRequest(DamageData damageData, StatusInfo attackerStatus, GameObject attacker, GameObject receiver, Collider hitCollider, Vector3 hitPoint, AttackHitData hitData = null,
            CriticalDamageTypes allowedCriticalTypes = DefaultCriticalTypes,
            IDamageResultReporter attackerDamageResultReporter = null)
        {
            AttackerDamageResultReporter = attackerDamageResultReporter;
            AllowedCriticalTypes = allowedCriticalTypes;
            DamageData = damageData;
            AttackerStatus = attackerStatus;
            Attacker = attacker;
            Receiver = receiver;
            HitCollider = hitCollider;
            HitPoint = hitPoint;
            AttackHitData = hitData;
        }

        public DamageRequest WithHit(GameObject receiver, Collider hitCollider, Vector3 hitPoint)
        {
            return new DamageRequest(
                DamageData,
                AttackerStatus,
                Attacker,
                receiver,
                hitCollider,
                hitPoint,
                AttackHitData,
                AllowedCriticalTypes,
                AttackerDamageResultReporter);
        }
    }
}
