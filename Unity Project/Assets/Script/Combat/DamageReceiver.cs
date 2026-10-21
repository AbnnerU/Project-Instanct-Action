using Scripts.Status;
using System;
using UnityEngine;

namespace Scripts.Combat
{
    public class DamageReceiver : MonoBehaviour, IDamageReceiver
    {
        public event Action<DamageResult> OnDamageReceived;

        [SerializeField] private GameObject _hittableReference;
        [SerializeField] private StatusInfoSourceBase _statusSource;
        private IHittable _hittableHandler;

        private void Awake()
        {
            CacheHittable();
        }

        public DamageResult ReceiveDamage(DamageRequest damageRequest)
        {
            if (_hittableHandler == null)
                CacheHittable();

            if (_hittableHandler == null || !_hittableHandler.IsAlive())
                return DamageResult.Failed(damageRequest);

            ResolvedDamageData resolvedDamage = DamageResolver.Resolve(
                damageRequest.DamageData,
                damageRequest.AttackerStatus,
                _statusSource?.GetStatusInfo(),
                damageRequest.AllowedCriticalTypes);

            _hittableHandler.Hit(resolvedDamage.FinalDamage, out bool killed);

            DamageResult damageResult = new DamageResult(
                resolvedDamage.DamageType,
                resolvedDamage.RawDamage,
                resolvedDamage.FinalDamage,
                killed,
                resolvedDamage.IsCritical,
                true,
                damageRequest);

            OnDamageReceived?.Invoke(damageResult);
            return damageResult;
        }

        private void CacheHittable()
        {
            if (_hittableReference == null)
                _hittableReference = gameObject;

            _hittableHandler = _hittableReference.GetComponent<IHittable>();

            if (_hittableHandler == null)
                _hittableHandler = _hittableReference.GetComponentInParent<IHittable>();

            if (_hittableHandler == null)
                _hittableHandler = _hittableReference.GetComponentInChildren<IHittable>();
        }
    }
}
