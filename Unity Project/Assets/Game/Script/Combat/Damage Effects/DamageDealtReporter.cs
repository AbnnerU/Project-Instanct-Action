using System;
using UnityEngine;

namespace Scripts.Combat
{
    [DisallowMultipleComponent]
    public class DamageDealtReporter : MonoBehaviour, IDamageResultReporter
    {
        public event Action<DamageResult> OnDamageReport;
        //public event Action<DamageResult> OnDamageDealt;

        public void ReportDamage(DamageResult damageResult)
        {
            // Projectiles can outlive their owner.
            if (this == null || !isActiveAndEnabled || !damageResult.WasApplied)
                return;

            OnDamageReport?.Invoke(damageResult);
        }
    }
}
