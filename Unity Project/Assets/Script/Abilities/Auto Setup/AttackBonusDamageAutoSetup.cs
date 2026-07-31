using Scripts.Combat;
using UnityEngine;

namespace Scripts.Abilities
{
    public class AttackBonusDamageAutoSetup : AbilityAutoSetupBase
    {
        [SerializeField] private AttackBonusDamageAbility _ability;

        public override void Setup(AbilityContext context)
        {
            if (_ability == null)
                TryGetComponent(out _ability);

            if (_ability == null || context == null)
                return;

            _ability.Setup(context.ComboController, context.AttackHitbox, context.StatusSource);
        }
    }
}
