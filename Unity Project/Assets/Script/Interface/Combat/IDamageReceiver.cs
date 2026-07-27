using System;

namespace Scripts.Combat
{
    public interface IDamageReceiver
    {
        event Action<DamageResult> OnDamageReceived;

        void ReceiveDamage(DamageRequest damageRequest);
    }
}
