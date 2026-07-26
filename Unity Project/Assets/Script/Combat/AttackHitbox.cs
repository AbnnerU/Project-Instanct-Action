using System.Collections.Generic;
using Scripts.Managers;
using Scripts.Player;
using UnityEngine;

namespace Scripts.Combat
{
    public class AttackHitbox : MonoBehaviour
    {
        [SerializeField] private Transform _hitboxOrigin;
        [SerializeField] private MN_PlayerStatus _playerStatus;
        [SerializeField] private bool _useHitboxOriginRotation;
        [SerializeField] private bool _drawGizmos = true;

        private Collider[] _hitResults = new Collider[8];
        private readonly HashSet<global::IHittable> _hashSet_HitTargets = new HashSet<global::IHittable>();

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
            _hashSet_HitTargets.Clear();

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
                if (!TryGetHittable(_hitResults[i], out global::IHittable hittable)) continue;
                if (!_hashSet_HitTargets.Add(hittable)) continue;

                ResolvedDamageData resolvedDamage = DamageResolver.Resolve(attackData.DamageData, _playerStatus);
                hittable.Hit(resolvedDamage.FinalDamage);
            }
        }

        private Quaternion GetHitboxRotation()
        {
            return _useHitboxOriginRotation && _hitboxOrigin != null ? _hitboxOrigin.rotation : Quaternion.identity;
        }

        private bool TryGetHittable(Collider targetCollider, out global::IHittable hittable)
        {
            hittable = targetCollider.GetComponentInParent<global::IHittable>();
            if (hittable != null) return true;

            hittable = targetCollider.GetComponentInChildren<global::IHittable>();
            return hittable != null;
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
