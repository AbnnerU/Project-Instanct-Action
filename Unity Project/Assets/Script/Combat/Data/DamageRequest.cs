using Scripts.Status;
using UnityEngine;

namespace Scripts.Combat
{
    public readonly struct DamageRequest
    {
        public readonly DamageData DamageData;
        public readonly StatusInfo AttackerStatus;
        public readonly GameObject Attacker;
        public readonly GameObject Receiver;
        public readonly Collider HitCollider;
        public readonly Vector3 HitPoint;

        public DamageRequest(
            DamageData damageData,
            StatusInfo attackerStatus,
            GameObject attacker,
            GameObject receiver,
            Collider hitCollider,
            Vector3 hitPoint)
        {
            DamageData = damageData;
            AttackerStatus = attackerStatus;
            Attacker = attacker;
            Receiver = receiver;
            HitCollider = hitCollider;
            HitPoint = hitPoint;
        }

        public DamageRequest WithHit(GameObject receiver, Collider hitCollider, Vector3 hitPoint)
        {
            return new DamageRequest(
                DamageData,
                AttackerStatus,
                Attacker,
                receiver,
                hitCollider,
                hitPoint);
        }
    }
}
