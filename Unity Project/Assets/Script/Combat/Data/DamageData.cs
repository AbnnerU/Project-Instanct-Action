using System;
using UnityEngine;

namespace Scripts.Combat
{
    [Serializable]
    public class DamageData
    {
        [SerializeField] private DamageType _damageType;
        [SerializeField] private float _baseDamage;
        [SerializeField] private float _physicForceScaling;
        [SerializeField] private float _magicForceScaling;

        public DamageType DamageType => _damageType;
        public float BaseDamage => _baseDamage;
        public float PhysicForceScaling => _physicForceScaling;
        public float MagicForceScaling => _magicForceScaling;
    }
}
