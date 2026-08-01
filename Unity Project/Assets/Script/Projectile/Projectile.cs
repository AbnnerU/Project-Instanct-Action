using System;
using UnityEngine;

public class Projectile : Projectile3DBase
{
    [SerializeField] private int _damage;

    public Action<IHittable> OnHitDamageableTarget;

    public void EnableProjectile(
        Vector3 startPoint,
        Vector3 speedOrietation,
        Vector3 gravityValue,
        LayerMask layerMask,
        bool useGroundLayer,
        LayerMask groundLayer,
        float radius,
        int damage,
        string targetTag)
    {
        _damage = damage;

        EnableProjectile(
            startPoint,
            speedOrietation,
            gravityValue,
            layerMask,
            useGroundLayer,
            groundLayer,
            radius,
            targetTag);
    }

    protected override void OnHit(Collider hitCollider, RaycastHit hit)
    {
        GameObject hitObject = hitCollider.gameObject;
        IHittable damageable = hitObject.GetComponent<IHittable>();
        IHittable damageableChild = hitObject.GetComponentInChildren<IHittable>();

        if (damageable != null)
        {
            damageable.Hit(_damage);
            OnHitDamageableTarget?.Invoke(damageable);
            return;
        }

        if (damageableChild == null)
            return;

        damageableChild.Hit(_damage);
        OnHitDamageableTarget?.Invoke(damageableChild);
    }
}
