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

        public void ReceiveDamage(DamageRequest damageRequest)
        {
            if (_hittableHandler == null)
                CacheHittable();

            if (_hittableHandler == null || !_hittableHandler.IsAlive())
                return;

            ResolvedDamageData resolvedDamage = DamageResolver.Resolve(
                damageRequest.DamageData,
                damageRequest.AttackerStatus,
                _statusSource?.GetStatusInfo());

            _hittableHandler.Hit(resolvedDamage.FinalDamage, out bool killed);

            DamageResult damageResult = new DamageResult(
                resolvedDamage.DamageType,
                resolvedDamage.FinalDamage,
                killed,
                damageRequest);

            OnDamageReceived?.Invoke(damageResult);
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
