using Scripts.DashHandler;
using UnityEngine;

namespace Scripts.Health
{
    public class OnDeath_DisableDash : OnDeathActionBase
    {
        [SerializeField] private DashBase _dash;

        protected override void Awake()
        {
            if (_dash == null) TryGetComponent(out _dash);
            base.Awake();
        }

        protected override void HealthEvents_OnDeath()
        {
            if (_dash == null) return;
            _dash.Disable();
            _dash.CancelDash();
            _dash.enabled = false;
        }
    }
}
