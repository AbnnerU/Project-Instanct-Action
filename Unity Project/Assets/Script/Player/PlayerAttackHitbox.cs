using System.Collections.Generic;
using Scripts.Combat;
using Scripts.Managers;
using UnityEngine;

namespace Scripts.Player
{
    public class PlayerAttackHitbox : MonoBehaviour
    {
        [SerializeField] private Transform _hitboxOrigin;
        [SerializeField] private PlayerDirection _playerDirection;
        [SerializeField] private MN_PlayerStatus _playerStatus;
        [SerializeField] private bool _drawGizmos = true;

        private Collider[] _hitResults = new Collider[8];
        private readonly HashSet<global::IHittable> _hashset_HitTargets = new HashSet<global::IHittable>();

        private ComboAttackData _lastAttackData;
        private Vector3 _lastCenter;

        private void Awake()
        {
            if (_hitboxOrigin == null)
                _hitboxOrigin = transform;

            if (_playerDirection == null)
                TryGetComponent(out _playerDirection);

            if (_playerStatus == null)
                _playerStatus = MN_PlayerStatus.Instance;
        }

        public void ExecuteHitbox(ComboAttackData attackData)
        {
            if (attackData == null)
            {
                Debug.LogError("Attack data is null.", this);
                return;
            }

            EnsureHitResultsCapacity(attackData.MaxTargets);
            _hashset_HitTargets.Clear();

            Vector3 center = GetHitboxCenter(attackData);
            _lastAttackData = attackData;
            _lastCenter = center;

            int hits = Physics.OverlapBoxNonAlloc(
                center,
                attackData.HitboxHalfExtents,
                _hitResults,
                Quaternion.identity,
                attackData.TargetLayerMask,
                attackData.QueryTriggerInteraction
            );

            for (int i = 0; i < hits; i++)
            {
                if (!TryGetHittable(_hitResults[i], out global::IHittable hittable)) continue;
                if (!_hashset_HitTargets.Add(hittable)) continue;

                ResolvedDamageData resolvedDamage = DamageResolver.Resolve(attackData.DamageData, _playerStatus);
                hittable.Hit(resolvedDamage.FinalDamage);
            }
        }

        private Vector3 GetHitboxCenter(ComboAttackData attackData)
        {
            Vector3 offset = attackData.HitboxCenterOffset;

            if (_playerDirection != null && _playerDirection.CurrentDirection == DirectionSide.LEFT)
                offset.x *= -1f;

            return _hitboxOrigin.position + offset;
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
            Gizmos.DrawWireCube(_lastCenter, _lastAttackData.HitboxHalfExtents * 2f);
        }
    }
}
