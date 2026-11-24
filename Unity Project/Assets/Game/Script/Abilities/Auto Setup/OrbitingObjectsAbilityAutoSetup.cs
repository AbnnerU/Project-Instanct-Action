using UnityEngine;

namespace Scripts.Abilities
{
    public class OrbitingObjectsAbilityAutoSetup : AbilityAutoSetupBase
    {
        [SerializeField] private OrbitingObjectsAbility _ability;

        public override void Setup(AbilityContext context)
        {
            if (_ability == null) TryGetComponent(out _ability);
            if (_ability == null || context == null) return;
            _ability.Setup(context.StatusSource, context.OwnerObject);
            _ability.SetDamageData(context.Lits_DamageData);
        }
    }
}
