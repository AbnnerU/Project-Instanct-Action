using System;

namespace Scripts.Combat
{
    public interface IDamageReceiver
    {
        event Action<DamageResult> OnDamageReceived;

        DamageResult ReceiveDamage(DamageRequest damageRequest);
    }
}
