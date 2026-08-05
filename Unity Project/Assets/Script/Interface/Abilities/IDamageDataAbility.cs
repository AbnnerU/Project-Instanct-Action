using Scripts.Combat;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Abilities
{
    public interface IDamageDataAbility 
    {
        public void SetDamageData(IReadOnlyList<DamageData> list_Damage);
    }
}