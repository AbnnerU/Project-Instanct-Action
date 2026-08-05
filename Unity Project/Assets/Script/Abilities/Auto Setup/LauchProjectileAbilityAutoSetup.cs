using UnityEngine;

namespace Scripts.Abilities
{
    public class LauchProjectileAbilityAutoSetup : AbilityAutoSetupBase
    {
        [SerializeField] private LauchProjectileAbility _ability;

        public override void Setup(AbilityContext context)
        {
            if (_ability == null)
                TryGetComponent(out _ability);

            if (_ability == null || context == null)
                return;

            _ability.Setup( context.StatusSource, context.ComboController, context.OwnerObject, context.PlayerDash);
            _ability.SetDamageData(context.Lits_DamageData);
        }
    }
}
