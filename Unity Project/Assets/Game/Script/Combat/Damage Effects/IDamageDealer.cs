using System;

namespace Scripts.Combat
{
    public interface IDamageDealer 
    {
        public event Action<DamageResult> OnDamageDealt;
    }
}
