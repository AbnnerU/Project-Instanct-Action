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

        //[Header("Advance")]
        //[SerializeField] private AttackAdvance _attackAdvance;

        [Header("Knockback")]
        [SerializeField] private AttackKnockback _attackKnockback;

        [Header("Hitbox")]
        [SerializeField] private Vector3 _hitboxCenterOffset = Vector3.right;
        [SerializeField] private Vector3 _hitboxHalfExtents = Vector3.one * 0.5f;
        [SerializeField] private LayerMask _targetLayerMask;
        [SerializeField] private QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.Collide;
        [SerializeField] private int _maxTargets = 8;

        public string AnimationStateName => _animationStateName;
        public float TransitionDuration => _transitionDuration;
        public DamageData DamageData => _damageData;
        //public float AdvanceDistance => _attackAdvance.AdvanceDistance;
        //public float AdvanceDuration => _attackAdvance.AvanceDuration;
        //public float AdvanceDelay => _attackAdvance.AdvanceDelay;
        ////public AnimationCurve AdvanceCurve => _attackAdvance.AdvanceCurve;
        public float KnockbackDistance => _attackKnockback.KnockbackDistance;
        public float KnockbackDuration => _attackKnockback.KnockbackDuration;
        public AnimationCurve KnockbackCurve => _attackKnockback.KnockbackCurve;
        public Vector3 HitboxCenterOffset => _hitboxCenterOffset;
        public Vector3 HitboxHalfExtents => _hitboxHalfExtents;
        public LayerMask TargetLayerMask => _targetLayerMask;
        public QueryTriggerInteraction QueryTriggerInteraction => _queryTriggerInteraction;
        public int MaxTargets => _maxTargets;
    }

    //[Serializable]
    //public class AttackAdvance
    //{
    //    public float AdvanceDistance;
    //    public float AvanceDuration;
    //    public float AdvanceDelay;
    //    public AnimationCurve AdvanceCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    //}

    [Serializable]
    public class AttackKnockback
    {
        public float KnockbackDistance;
        public float KnockbackDuration;
        public AnimationCurve KnockbackCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    }
}
