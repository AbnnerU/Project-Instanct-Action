using System.Collections;
using System.Collections.Generic;
using Scripts.Combat;
using Scripts.DashHandler;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Abilities
{
    public class TemporaryStatusBuffOnDashAbility : AbilityBase, IBuffDataAbility
    {
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private DashHandler.DashBase _dashComponent;
        [Header("Trigger")]
        [SerializeField] private AbilityTriggerType _triggerType = AbilityTriggerType.ON_DASH_FINISHED;

        [Header("Buff")]
        [SerializeField] private BuffData _buffData;
        [SerializeField] private AbilityRefreshMode _refreshMode = AbilityRefreshMode.REFRESH_DURATION;

        private StatusData _statusToAffect;

        private Coroutine _buffRoutine;
        private float _appliedValue;
        private bool _subscribed;
        private bool _buffActive;

        #region Setup

        public void Setup(StatusInfoSourceBase statusSource, DashHandler.DashBase dashComponent)
        {
            _statusSource = statusSource;
            _dashComponent = dashComponent;
        }

        public override void Initialize()
        {
            base.Initialize();
            Subscribe();
        }

        public override void Deactivate()
        {
            Unsubscribe();
            RemoveBuff();
            base.Deactivate();
        }

        #endregion

        #region IBuffDataAbility
        public void SetBuffData(IReadOnlyList<BuffData> list_BuffData)
        {
            if (list_BuffData != null && list_BuffData.Count > 0 && list_BuffData[0] != null)
                _buffData = list_BuffData[0];
        }
        #endregion

        #region Public

        public override void Execute()
        {
            ApplyBuff();
        }

        #endregion

        #region Events
        private void Subscribe()
        {
            if (_subscribed || _dashComponent == null)
                return;

            _statusSource.TryGetStatusData(_buffData.StatusType, out _statusToAffect);

            _dashComponent.OnDashStarted += PlayerDash_OnDashStarted;
            _dashComponent.OnDashFinished += PlayerDash_OnDashFinished;
            _subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_subscribed || _dashComponent == null)
                return;

            _dashComponent.OnDashStarted -= PlayerDash_OnDashStarted;
            _dashComponent.OnDashFinished -= PlayerDash_OnDashFinished;
            _subscribed = false;
        }

        private void PlayerDash_OnDashStarted(float directionDot)
        {
            if (_triggerType == AbilityTriggerType.ON_DASH_STARTED)
                Execute();
        }

        private void PlayerDash_OnDashFinished(float directionDot)
        {
            if (!_dashComponent.LastDashCanceled && _triggerType == AbilityTriggerType.ON_DASH_FINISHED)
                Execute();
        }
        #endregion

        #region Buff
        private void ApplyBuff()
        {
            if (_statusSource == null || _statusToAffect == null)
            {
                Debug.LogWarning($"[TemporaryStatusBuffOnDashAbility] {gameObject.name} Ability do not have any _statusSource ou _statusToAffect");
                return;
            }

            if (_buffActive)
            {
                if (_refreshMode == AbilityRefreshMode.IGNORE_WHILE_ACTIVE)
                    return;

                if (_refreshMode == AbilityRefreshMode.REFRESH_DURATION)
                    RemoveBuff();
            }

            float valueToAdd = GetValueToAdd();
            _statusToAffect.AddCurrentValue(valueToAdd);

            _appliedValue += valueToAdd;
            _buffActive = true;

            if (_buffRoutine != null)
                StopCoroutine(_buffRoutine);

            if (_buffData.Duration > 0f)
                _buffRoutine = StartCoroutine(BuffRoutine());
        }

        private IEnumerator BuffRoutine()
        {
            yield return new WaitForSeconds(_buffData.Duration);
            RemoveBuff();
        }

        private void RemoveBuff()
        {
            if (!_buffActive || _statusSource == null || _statusToAffect == null)
                return;

            if (_buffRoutine != null)
            {
                StopCoroutine(_buffRoutine);
                _buffRoutine = null;
            }

            _statusToAffect.RemoveCurrentValue(_appliedValue);
            _appliedValue = 0f;
            _buffActive = false;
        }

        private float GetValueToAdd()
        {
            if (_buffData.ValueType == AbilityValueType.FLAT)
                return _buffData.Value;

            return _statusToAffect.CurrentValue * (_buffData.Value / 100f);
        }
        #endregion
    }
}
