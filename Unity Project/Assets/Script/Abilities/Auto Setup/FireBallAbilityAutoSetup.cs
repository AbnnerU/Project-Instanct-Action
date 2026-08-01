using Scripts.Player;
using UnityEngine;

namespace Scripts.Abilities
{
    public class FireBallAbilityAutoSetup : AbilityAutoSetupBase
    {
        [SerializeField] private FireBallAbility _ability;

        public override void Setup(AbilityContext context)
        {
            if (_ability == null)
                TryGetComponent(out _ability);

            if (_ability == null || context == null)
                return;

            PlayerDirection playerDirection = context.OwnerObject != null
                ? context.OwnerObject.GetComponentInChildren<PlayerDirection>()
                : null;

            _ability.Setup(playerDirection, context.StatusSource, context.ComboController, context.OwnerObject);
        }
    }
}
