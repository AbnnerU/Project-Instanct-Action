using Scripts.Abilities;
using Scripts.Status;
using System;
using UnityEngine;

namespace Scripts.Combat
{
    [Serializable]
    public class BuffData
    {
        [SerializeField] private StatusType _statusType = StatusType.PHYSIC_FORCE;
        [SerializeField] private AbilityValueType _valueType = AbilityValueType.PERCENT_OF_CURRENT;
        [SerializeField] private float _value = 25f;
        [SerializeField] private float _duration = 3f;

        public StatusType StatusType => _statusType;
        public AbilityValueType ValueType => _valueType;
        public float Value => _value;
        public float Duration => _duration;
    }
}
