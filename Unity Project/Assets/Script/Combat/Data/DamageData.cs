using System;
using UnityEngine;

namespace Scripts.Combat
{
    [Serializable]
    public class DamageData
    {
        [SerializeField] private DamageType _damageType;
        [SerializeField] private float _baseDamage;
        [Tooltip("Percentage of physical force added to damage. 100 = 100%, 200 = 200%.")]
        [SerializeField] private float _physicForceScaling;
        [Tooltip("Percentage of magic force added to damage. 100 = 100%, 200 = 200%.")]
        [SerializeField] private float _magicForceScaling;

        public DamageType DamageType => _damageType;
        public float BaseDamage => _baseDamage;
        public float PhysicForceScaling => _physicForceScaling;
        public float MagicForceScaling => _magicForceScaling;
    }
}
