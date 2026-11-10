using System;
using UnityEngine;

namespace Scripts.Combat
{
    [Serializable]
    public class AttackHitData
    {
        [SerializeField] private DamageData _damageData = new DamageData();


        [Header("Knockback")]
        [SerializeField] private AttackKnockback _attackKnockback = new AttackKnockback();

        [Header("Hitbox")]
        [SerializeField] private Vector3 _hitboxCenterOffset = Vector3.right;
        [SerializeField] private Vector3 _hitboxHalfExtents = Vector3.one * 0.5f;
        [SerializeField] private LayerMask _targetLayerMask;
        [SerializeField] private QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.Collide;
        [SerializeField] private int _maxTargets = 8;

        public DamageData DamageData => _damageData;
        public float KnockbackDistance => _attackKnockback.KnockbackDistance;
        public float KnockbackDuration => _attackKnockback.KnockbackDuration;
        public AnimationCurve KnockbackCurve => _attackKnockback.KnockbackCurve;
        public Vector3 HitboxCenterOffset => _hitboxCenterOffset;
        public Vector3 HitboxHalfExtents => _hitboxHalfExtents;
        public LayerMask TargetLayerMask => _targetLayerMask;
        public QueryTriggerInteraction QueryTriggerInteraction => _queryTriggerInteraction;
        public int MaxTargets => _maxTargets;
    }

}

