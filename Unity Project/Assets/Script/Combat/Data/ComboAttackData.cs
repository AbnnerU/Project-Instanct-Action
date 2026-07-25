using System;
using UnityEngine;

namespace Scripts.Combat
{
    [Serializable]
    public class ComboAttackData
    {
        [SerializeField] private string _animationStateName;
        [SerializeField] private float _transitionDuration = 0.05f;
        [SerializeField] private DamageData _damageData;
        [SerializeField] private Vector3 _hitboxCenterOffset = Vector3.right;
        [SerializeField] private Vector3 _hitboxHalfExtents = Vector3.one * 0.5f;
        [SerializeField] private LayerMask _targetLayerMask;
        [SerializeField] private QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.Collide;
        [SerializeField] private int _maxTargets = 8;

        public string AnimationStateName => _animationStateName;
        public float TransitionDuration => _transitionDuration;
        public DamageData DamageData => _damageData;
        public Vector3 HitboxCenterOffset => _hitboxCenterOffset;
        public Vector3 HitboxHalfExtents => _hitboxHalfExtents;
        public LayerMask TargetLayerMask => _targetLayerMask;
        public QueryTriggerInteraction QueryTriggerInteraction => _queryTriggerInteraction;
        public int MaxTargets => _maxTargets;
    }
}
