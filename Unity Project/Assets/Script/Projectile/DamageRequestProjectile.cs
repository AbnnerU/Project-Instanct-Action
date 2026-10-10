using Scripts.Combat;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Projectile
{
    public class DamageRequestProjectile : Projectile3DBase
    {
        private DamageRequest _damageRequest;
        protected readonly HashSet<IDamageReceiver> _hashSet_DamageReceivers = new HashSet<IDamageReceiver>();
        public void SetDamageRequest(DamageRequest damageRequest)
        {
            _damageRequest = damageRequest;
        }

        public override void EnableProjectile(Vector3 startPoint, Vector3 speedOrientation, Vector3 gravityValue, LayerMask layerMask, bool useGroundLayer, LayerMask groundLayer, float radius, string targetTag)
        {
            _hashSet_DamageReceivers.Clear();
            base.EnableProjectile(startPoint, speedOrientation, gravityValue, layerMask, useGroundLayer, groundLayer, radius, targetTag);
        }

        protected override void OnHit(Collider hitCollider, RaycastHit hit)
        {
            if (!TryGetDamageReceiver(hitCollider, out IDamageReceiver damageReceiver))
                return;

            GameObject receiverObject = damageReceiver is Component component ? component.gameObject : hitCollider.gameObject;
            DamageRequest hitDamageRequest = _damageRequest.WithHit(receiverObject, hitCollider, hit.point);

            damageReceiver.ReceiveDamage(hitDamageRequest);

            _hashSet_DamageReceivers.Add(damageReceiver);
        }

        private bool TryGetDamageReceiver(Collider hitCollider, out IDamageReceiver damageReceiver)
        {
            damageReceiver = hitCollider.GetComponentInParent<IDamageReceiver>();

            if (damageReceiver != null && !_hashSet_DamageReceivers.Contains(damageReceiver))
                return true;
            else
                return false;
        }
    }
}