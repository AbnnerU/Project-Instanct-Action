using System;
using System.Collections.Generic;
using Scripts.Player;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Combat
{
    public class AttackHitbox : MonoBehaviour, IDamageDealer
    {
        public event Action<DamageResult> OnDamageDealt;
        public event Action<AttackExecutionData, DamageResult> OnDamageDealtWithExecution;

        [SerializeField] private Transform _hitboxOrigin;
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private bool _useHitboxOriginRotation;
        [SerializeField] private bool _drawGizmos = true;
        [SerializeField] private bool _drawExecutedHitboxGizmos = true;
        [SerializeField] private Color _executedHitboxColor = Color.red;
        [SerializeField] private float _executedHitboxGizmosDuration = 0.15f;

        private Collider[] _hitResults = new Collider[8];
        private readonly HashSet<IDamageReceiver> _hashSet_DamageReceivers = new HashSet<IDamageReceiver>();
        private readonly HashSet<IHittable> _hashSet_HittableTargets = new HashSet<IHittable>();

        private ComboAttackData _lastAttackData;
        private AttackExecutionData _lastAttackExecutionData;
        private Vector3 _lastCenter;
        private Quaternion _lastRotation;
        private float _lastExecutedHitboxExpireTime;

        private void Awake()
        {
            GetReferences();
        }

        public void ExecuteHitbox(ComboAttackData attackData)
        {
            ExecuteHitbox(attackData, default);
        }

        public void ExecuteHitbox(ComboAttackData attackData, AttackExecutionData executionData)
        {
            if (attackData == null)
            {
                Debug.LogError("Attack data is null.", this);
                return;
            }

            EnsureHitResultsCapacity(attackData.MaxTargets);
            _hashSet_DamageReceivers.Clear();
            _hashSet_HittableTargets.Clear();

            Vector3 center = GetHitboxCenter(attackData);
            _lastAttackData = attackData;
            _lastAttackExecutionData = executionData;
            _lastCenter = center;
            _lastRotation = GetHitboxRotation();
            _lastExecutedHitboxExpireTime = Time.time + _executedHitboxGizmosDuration;

            int hits = Physics.OverlapBoxNonAlloc(
                center,
                attackData.HitboxHalfExtents,
                _hitResults,
                _lastRotation,
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
                    OnDamageDealtWithExecution?.Invoke(executionData, damageResult);
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
                OnDamageDealtWithExecution?.Invoke(executionData, damageResultFallback);
            }
        }

        public void NotifyDamageDealt(DamageResult damageResult)
        {
            OnDamageDealt?.Invoke(damageResult);
        }

        public Vector3 GetHitboxCenter(ComboAttackData attackData)
        {
            GetReferences();

            return _hitboxOrigin.position + GetHitboxCenterOffset(attackData);
        }

        private void GetReferences()
        {
            if (_hitboxOrigin == null)
                _hitboxOrigin = transform;
        }

        public Quaternion GetHitboxRotation()
        {
            return _useHitboxOriginRotation && _hitboxOrigin != null ? _hitboxOrigin.rotation : Quaternion.identity;
        }

        private Vector3 GetHitboxCenterOffset(ComboAttackData attackData)
        {
            Vector3 offset = attackData.HitboxCenterOffset;

            if (!_useHitboxOriginRotation || _hitboxOrigin == null)
                return offset;

            return _hitboxOrigin.rotation * offset;
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
                hitCollider.ClosestPoint(_hitboxOrigin.position),
                attackData);
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

            Gizmos.color = _executedHitboxColor;
            Gizmos.matrix = Matrix4x4.TRS(_lastCenter, _lastRotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, _lastAttackData.HitboxHalfExtents * 2f);
            Gizmos.matrix = Matrix4x4.identity;
        }

        private void OnDrawGizmos()
        {
            if (!_drawExecutedHitboxGizmos || _lastAttackData == null || Time.time > _lastExecutedHitboxExpireTime)
                return;

            Gizmos.color = _executedHitboxColor;
            Gizmos.matrix = Matrix4x4.TRS(_lastCenter, _lastRotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, _lastAttackData.HitboxHalfExtents * 2f);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
