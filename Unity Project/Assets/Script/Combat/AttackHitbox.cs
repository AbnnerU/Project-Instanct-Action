using System;
using System.Collections.Generic;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Combat
{
    public class AttackHitbox : MonoBehaviour, IDamageDealer
    {
        public event Action<DamageResult> OnDamageDealt;

        [SerializeField] private Transform _hitboxOrigin;
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private bool _useHitboxOriginRotation;
        [SerializeField] private bool _drawGizmos = true;

        private Collider[] _hitResults = new Collider[8];
        private readonly HashSet<IDamageReceiver> _hashSet_DamageReceivers = new HashSet<IDamageReceiver>();
        private readonly HashSet<IHittable> _hashSet_HittableTargets = new HashSet<IHittable>();

        private ComboAttackData _lastAttackData;
        private Vector3 _lastCenter;

        private void Awake()
        {
            if (_hitboxOrigin == null)
                _hitboxOrigin = transform;
        }

        public void ExecuteHitbox(ComboAttackData attackData)
        {
            if (attackData == null)
            {
                Debug.LogError("Attack data is null.", this);
                return;
            }

            EnsureHitResultsCapacity(attackData.MaxTargets);
            _hashSet_DamageReceivers.Clear();
            _hashSet_HittableTargets.Clear();

            Vector3 center = _hitboxOrigin.position + attackData.HitboxCenterOffset;
            _lastAttackData = attackData;
            _lastCenter = center;

            int hits = Physics.OverlapBoxNonAlloc(
                center,
                attackData.HitboxHalfExtents,
                _hitResults,
                GetHitboxRotation(),
                attackData.TargetLayerMask,
                attackData.QueryTriggerInteraction
            );

            for (int i = 0; i < hits; i++)
            {
                Collider hitCollider = _hitResults[i];

                if (TryGetDamageReceiver(hitCollider, out IDamageReceiver damageReceiver))
                {
                    if (!_hashSet_DamageReceivers.Add(damageReceiver)) continue;

                    DamageRequest damageRequest = CreateDamageRequest(attackData, hitCollider, GetDamageReceiverGameObject(damageReceiver));

                    DamageResult damageResult = damageReceiver.ReceiveDamage(damageRequest);
                    OnDamageDealt?.Invoke(damageResult);
                    continue;
                }

                if (!TryGetHittable(hitCollider, out IHittable hittable)) continue;
                if (!_hashSet_HittableTargets.Add(hittable)) continue;

                ResolvedDamageData resolvedDamage = DamageResolver.Resolve(attackData.DamageData, _statusSource?.GetStatusInfo());
                hittable.Hit(resolvedDamage.FinalDamage, out bool killed);

                DamageRequest damageRequestFallback = CreateDamageRequest(attackData, hitCollider, GetHittableGameObject(hittable));
                DamageResult damageResultFallback = new DamageResult(
                    resolvedDamage.DamageType,
                    resolvedDamage.RawDamage,
                    resolvedDamage.FinalDamage,
                    killed,
                    resolvedDamage.IsCritical,
                    true,
                    damageRequestFallback);

                OnDamageDealt?.Invoke(damageResultFallback);
            }
        }

        private Quaternion GetHitboxRotation()
        {
            return _useHitboxOriginRotation && _hitboxOrigin != null ? _hitboxOrigin.rotation : Quaternion.identity;
        }

        private bool TryGetDamageReceiver(Collider targetCollider, out IDamageReceiver damageReceiver)
        {
            damageReceiver = targetCollider.GetComponentInParent<IDamageReceiver>();
            if (damageReceiver != null) return true;

            damageReceiver = targetCollider.GetComponentInChildren<IDamageReceiver>();
            return damageReceiver != null;
        }

        private bool TryGetHittable(Collider targetCollider, out IHittable hittable)
        {
            hittable = targetCollider.GetComponentInParent<IHittable>();
            if (hittable != null) return true;

            hittable = targetCollider.GetComponentInChildren<IHittable>();
            return hittable != null;
        }

        private GameObject GetDamageReceiverGameObject(IDamageReceiver damageReceiver)
        {
            return damageReceiver is Component component ? component.gameObject : null;
        }

        private GameObject GetHittableGameObject(IHittable hittable)
        {
            return hittable is Component component ? component.gameObject : null;
        }

        private DamageRequest CreateDamageRequest(ComboAttackData attackData, Collider hitCollider, GameObject receiver)
        {
            return new DamageRequest(
                attackData.DamageData,
                _statusSource?.GetStatusInfo(),
                gameObject,
                receiver,
                hitCollider,
                hitCollider.ClosestPoint(_hitboxOrigin.position));
        }

        private void EnsureHitResultsCapacity(int maxTargets)
        {
            maxTargets = Mathf.Max(1, maxTargets);

            if (_hitResults.Length >= maxTargets) return;

            _hitResults = new Collider[maxTargets];
        }

        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmos || _lastAttackData == null) return;

            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(_lastCenter, GetHitboxRotation(), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, _lastAttackData.HitboxHalfExtents * 2f);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
