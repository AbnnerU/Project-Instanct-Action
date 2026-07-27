using UnityEngine;

namespace Scripts.Combat
{
    public readonly struct DamageResult
    {
        public readonly DamageType DamageType;
        public readonly float FinalDamage;
        public readonly bool Killed;
        public readonly DamageRequest DamageRequest;

        public GameObject Attacker => DamageRequest.Attacker;
        public GameObject Receiver => DamageRequest.Receiver;
        public Collider HitCollider => DamageRequest.HitCollider;
        public Vector3 HitPoint => DamageRequest.HitPoint;

        public DamageResult(DamageType damageType, float finalDamage, bool killed, DamageRequest damageRequest)
        {
            DamageType = damageType;
            FinalDamage = finalDamage;
            Killed = killed;
            DamageRequest = damageRequest;
        }
    }
}
