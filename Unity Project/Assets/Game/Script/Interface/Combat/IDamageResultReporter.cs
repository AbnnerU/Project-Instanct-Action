using System;

namespace Scripts.Combat
{
    public interface IDamageResultReporter
    {
        public event Action<DamageResult> OnDamageReport;
        void ReportDamage(DamageResult damageResult);
    }
}
