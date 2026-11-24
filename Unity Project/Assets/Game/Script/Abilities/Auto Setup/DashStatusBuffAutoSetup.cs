using UnityEngine;

namespace Scripts.Abilities
{
    public class DashStatusBuffAutoSetup : AbilityAutoSetupBase
    {
        [SerializeField] private TemporaryStatusBuffOnDashAbility _ability;

        public override void Setup(AbilityContext context)
        {
            if (_ability == null)
                TryGetComponent(out _ability);

            if (_ability == null || context == null)
                return;

            _ability.Setup(context.StatusSource, context.PlayerDash);
            _ability.SetBuffData(context.Lits_BuffData);
        }
    }
}
