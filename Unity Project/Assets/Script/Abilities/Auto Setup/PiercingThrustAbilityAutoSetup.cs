using UnityEngine;

namespace Scripts.Abilities
{
    public class PiercingThrustAbilityAutoSetup : AbilityAutoSetupBase
    {
        [SerializeField] private PiercingThrustAbility _ability;

        public override void Setup(AbilityContext context)
        {
            if (_ability == null)
                TryGetComponent(out _ability);

            if (_ability == null || context == null)
                return;

            _ability.Setup(context.OwnerTransform,context.StatusSource,context.ComboController,context.OwnerObject,context.PlayerDash);
            _ability.SetDamageData(context.Lits_DamageData);
        }
    }
}
