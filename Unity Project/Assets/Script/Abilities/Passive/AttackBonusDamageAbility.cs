using System.Collections.Generic;
using Scripts.Combat;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Abilities
{
    public class AttackBonusDamageAbility : AbilityBase, IDamageDataAbility
    {
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private ComboControllerBase _comboController;
        [SerializeField] private AttackHitbox _attackHitbox;
        [Header("Trigger")]
        [SerializeField] private AttackBonusDamageTriggerMode _triggerMode = AttackBonusDamageTriggerMode.EVERY_N_ATTACKS;
        [SerializeField] private int _attackInterval = 3;
        [SerializeField] private AttackBonusDamageApplyMode _applyMode = AttackBonusDamageApplyMode.ALL_HITS_OF_TRIGGERED_ATTACK;

        [Header("Damage")]
        [SerializeField] private DamageData _bonusDamageData = new DamageData();

        private int _attackCounter;
        private bool _subscribed;

        private readonly HashSet<int> _hashSet_BonusExecutionIds = new HashSet<int>();
        private readonly HashSet<int> _hashSet_ConsumedExecutionIds = new HashSet<int>();

        #region IDamageDataAbility
        public void SetDamageData(IReadOnlyList<DamageData> list_Damage)
        {
            if (list_Damage != null && list_Damage.Count > 0 && list_Damage[0] != null)
                _bonusDamageData = list_Damage[0];
        }
        #endregion

        #region Setup

        public void Setup(ComboControllerBase comboController, AttackHitbox attackHitbox, StatusInfoSourceBase statusSource)
        {
            _comboController = comboController;
            _attackHitbox = attackHitbox;
            _statusSource = statusSource;
        }

        public override void Initialize()
        {
            base.Initialize();
            Subscribe();
        }

        public override void Deactivate()
        {
            Unsubscribe();
            _hashSet_BonusExecutionIds.Clear();
            _hashSet_ConsumedExecutionIds.Clear();
            base.Deactivate();
        }

        #endregion

        #region Public

        public override void Execute()
        {
        }

        #endregion

        #region Events

        private void Subscribe()
        {
            if (_subscribed || _comboController == null || _attackHitbox == null)
                return;

            _comboController.OnAttackHitboxExecuted += ComboController_OnAttackHitboxExecuted;
            _comboController.OnAttackHitboxFinished += ComboController_OnAttackHitboxFinished;
            _attackHitbox.OnDamageDealtWithExecution += AttackHitbox_OnDamageDealtWithExecution;
            _subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_subscribed)
                return;

            if (_comboController != null)
            {
                _comboController.OnAttackHitboxExecuted -= ComboController_OnAttackHitboxExecuted;
                _comboController.OnAttackHitboxFinished -= ComboController_OnAttackHitboxFinished;
            }

            if (_attackHitbox != null)
                _attackHitbox.OnDamageDealtWithExecution -= AttackHitbox_OnDamageDealtWithExecution;

            _subscribed = false;
        }

        private void ComboController_OnAttackHitboxExecuted(AttackExecutionData executionData)
        {
            if (!ShouldApplyBonusToExecution())
                return;

            _hashSet_BonusExecutionIds.Add(executionData.ExecutionId);
        }

        private void ComboController_OnAttackHitboxFinished(AttackExecutionData executionData)
        {
            _hashSet_BonusExecutionIds.Remove(executionData.ExecutionId);
            _hashSet_ConsumedExecutionIds.Remove(executionData.ExecutionId);
        }

        private void AttackHitbox_OnDamageDealtWithExecution(AttackExecutionData executionData, DamageResult damageResult)
        {
            if (!damageResult.WasApplied || !_hashSet_BonusExecutionIds.Contains(executionData.ExecutionId))
                return;

            if (_applyMode == AttackBonusDamageApplyMode.FIRST_HIT_OF_TRIGGERED_ATTACK &&
                _hashSet_ConsumedExecutionIds.Contains(executionData.ExecutionId))
                return;

            if (!TryGetDamageReceiver(damageResult, out IDamageReceiver damageReceiver))
                return;

            ApplyBonusDamage(damageReceiver, damageResult);
            _hashSet_ConsumedExecutionIds.Add(executionData.ExecutionId);
        }

        #endregion

        #region Damage

        private bool ShouldApplyBonusToExecution()
        {
            if (_triggerMode == AttackBonusDamageTriggerMode.EVERY_ATTACK)
                return true;

            _attackCounter++;

            if (_attackCounter < Mathf.Max(1, _attackInterval))
                return false;

            _attackCounter = 0;
            return true;
        }

        private void ApplyBonusDamage(IDamageReceiver damageReceiver, DamageResult sourceDamageResult)
        {
            DamageRequest damageRequest = new DamageRequest(
                _bonusDamageData,
                _statusSource != null ? _statusSource.GetStatusInfo() : null,
                sourceDamageResult.Attacker != null ? sourceDamageResult.Attacker : gameObject,
                sourceDamageResult.Receiver,
                sourceDamageResult.HitCollider,
                sourceDamageResult.HitPoint);

            DamageResult bonusDamageResult = damageReceiver.ReceiveDamage(damageRequest);
            _attackHitbox?.NotifyDamageDealt(bonusDamageResult);
        }

        private bool TryGetDamageReceiver(DamageResult damageResult, out IDamageReceiver damageReceiver)
        {
            damageReceiver = null;

            if (damageResult.Receiver != null)
            {
                damageReceiver = damageResult.Receiver.GetComponentInParent<IDamageReceiver>();

                if (damageReceiver != null)
                    return true;

                damageReceiver = damageResult.Receiver.GetComponentInChildren<IDamageReceiver>();

                if (damageReceiver != null)
                    return true;
            }

            if (damageResult.HitCollider == null)
                return false;

            damageReceiver = damageResult.HitCollider.GetComponentInParent<IDamageReceiver>();

            if (damageReceiver != null)
                return true;

            damageReceiver = damageResult.HitCollider.GetComponentInChildren<IDamageReceiver>();
            return damageReceiver != null;
        }

        #endregion
    }
}
