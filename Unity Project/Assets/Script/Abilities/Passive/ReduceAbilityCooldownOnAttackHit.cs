using System;
using System.Collections.Generic;
using Scripts.Combat;
using UnityEngine;

namespace Scripts.Abilities
{
    public class ReduceAbilityCooldownOnAttackHit : AbilityBase
    {
        [SerializeField] private ComboControllerBase _comboController;
        [SerializeField] private AttackHitbox _attackHitbox;
        [SerializeField] private MN_Abilities _abilities;
        [SerializeField, Min(0f)] private float _secondsRecovered = 0.5f;
        [SerializeField, Min(0f)] private float _minimumActivationInterval = 0.2f;

        private readonly HashSet<Guid> _hashSet_ConsumedExecutionIds = new HashSet<Guid>();
        private float _nextActivationTime;
        private bool _subscribed;

        public void Setup(ComboControllerBase comboController, AttackHitbox attackHitbox, MN_Abilities abilities)
        {
            Unsubscribe();
            _comboController = comboController;
            _attackHitbox = attackHitbox;
            _abilities = abilities;
        }

        public override void Initialize()
        {
            base.Initialize();
            _nextActivationTime = 0f;
            _hashSet_ConsumedExecutionIds.Clear();
            if (_subscribed || _comboController == null || _attackHitbox == null || _abilities == null)
                return;

            _attackHitbox.OnDamageDealtWithExecution += AttackHitbox_OnDamageDealtWithExecution;
            _comboController.OnAttackHitboxExecutionFinished += ComboController_OnAttackHitboxFinished;
            _subscribed = true;
        }

        public override void Execute() { }

        protected override void OnDeactivate()
        {
            Unsubscribe();
            _hashSet_ConsumedExecutionIds.Clear();
        }

        private void Unsubscribe()
        {
            if (!_subscribed) return;

            if (_attackHitbox != null)
                _attackHitbox.OnDamageDealtWithExecution -= AttackHitbox_OnDamageDealtWithExecution;
            if (_comboController != null)
                _comboController.OnAttackHitboxExecutionFinished -= ComboController_OnAttackHitboxFinished;
            _subscribed = false;
        }

        private void AttackHitbox_OnDamageDealtWithExecution(AttackExecutionData execution, DamageResult result)
        {
            if (!CanExecute() || !isActiveAndEnabled || !result.WasApplied || _abilities == null ||
                _secondsRecovered <= 0f || Time.time < _nextActivationTime ||
                !_hashSet_ConsumedExecutionIds.Add(execution.ExecutionId))
                return;

            _nextActivationTime = Time.time + _minimumActivationInterval;
            _abilities.ReduceActiveAbilityCooldowns(_secondsRecovered);
        }

        private void ComboController_OnAttackHitboxFinished(AttackExecutionData execution)
        {
            _hashSet_ConsumedExecutionIds.Remove(execution.ExecutionId);
        }
    }
}
