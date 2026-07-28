using UnityEngine;

namespace Scripts.Progression
{
    public class OnDeath_AddXp : OnDeathActionBase
    {
        [SerializeField] private int _xpValue = 10;

        protected override void HealthEvents_OnDeath()
        {
            if (MN_Xp.Instance == null)
                return;

            MN_Xp.Instance.AddXp(_xpValue);
        }
    }
}
