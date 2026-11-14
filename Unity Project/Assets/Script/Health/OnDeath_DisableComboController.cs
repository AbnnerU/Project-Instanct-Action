using Scripts.Combat;
using UnityEngine;

namespace Scripts.Health
{
    public class OnDeath_DisableComboController : OnDeathActionBase
    {
        [SerializeField] private ComboControllerBase _comboController;

        protected override void Awake()
        {
            if (_comboController == null) TryGetComponent(out _comboController);
            base.Awake();
        }

        protected override void HealthEvents_OnDeath()
        {
            if (_comboController == null) return;
            _comboController.Disable();
            _comboController.CancelAttack(playIdle: false, maintainComboIndex: false);
            _comboController.enabled = false;
        }
    }
}
