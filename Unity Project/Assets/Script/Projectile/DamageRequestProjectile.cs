using Scripts.Combat;
using UnityEngine;

namespace Scripts.Projectile
{
    public class DamageRequestProjectile : Projectile3DBase
    {
        private DamageRequest _damageRequest;

        public void SetDamageRequest(DamageRequest damageRequest)
        {
            _damageRequest = damageRequest;
        }

        protected override void OnHit(Collider hitCollider, RaycastHit hit)
        {
            if (!TryGetDamageReceiver(hitCollider, out IDamageReceiver damageReceiver))
                return;

            GameObject receiverObject = damageReceiver is Component component ? component.gameObject : hitCollider.gameObject;
            DamageRequest hitDamageRequest = _damageRequest.WithHit(receiverObject, hitCollider, hit.point);

            damageReceiver.ReceiveDamage(hitDamageRequest);
        }

        private bool TryGetDamageReceiver(Collider hitCollider, out IDamageReceiver damageReceiver)
        {
            damageReceiver = hitCollider.GetComponentInParent<IDamageReceiver>();

            if (damageReceiver != null)
                return true;

            damageReceiver = hitCollider.GetComponentInChildren<IDamageReceiver>();
            return damageReceiver != null;
        }
    }
}