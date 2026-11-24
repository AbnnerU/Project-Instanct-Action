using UnityEngine;

namespace Scripts.Abilities
{
    public class SpawnAreaDamageAbilityAutoSetup : AbilityAutoSetupBase
    {
        [SerializeField] private SpawnAreaDamageAbility _ability;

        public override void Setup(AbilityContext context)
        {
            if (_ability == null) TryGetComponent(out _ability);
            if (_ability == null || context == null) return;
            _ability.Setup(context.StatusSource, context.ComboController, context.OwnerObject);
            _ability.SetDamageData(context.Lits_DamageData);
        }
    }
}
