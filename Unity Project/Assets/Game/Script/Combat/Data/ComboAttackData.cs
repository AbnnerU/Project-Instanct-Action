using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Combat
{
    [Serializable]
    public class ComboAttackData
    {
        [SerializeField] private string _animationStateName;
        [SerializeField] private float _transitionDuration = 0.05f;
        [SerializeField] private List<AttackHitData> _list_Hits = new List<AttackHitData> { new AttackHitData() };

        public string AnimationStateName => _animationStateName;
        public float TransitionDuration => _transitionDuration;
        public IReadOnlyList<AttackHitData> Hits => _list_Hits;
        public AttackHitData GetHit(int index) => _list_Hits != null && index >= 0 && index < _list_Hits.Count ? _list_Hits[index] : null;
    }

    [Serializable]
    public class AttackKnockback
    {
        public float KnockbackDistance;
        public float KnockbackDuration;
        public AnimationCurve KnockbackCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    }
}
