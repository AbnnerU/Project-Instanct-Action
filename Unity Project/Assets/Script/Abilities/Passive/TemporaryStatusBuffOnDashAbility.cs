using System.Collections;
using Scripts.Managers;
using Scripts.Player;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Abilities
{
    public class TemporaryStatusBuffOnDashAbility : AbilityBase
    {
        #region Fields
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private PlayerDash _dashComponent;
        [Header("Trigger")]
        [SerializeField] private AbilityTriggerType _triggerType = AbilityTriggerType.ON_DASH_FINISHED;

        [Header("Buff")]
        [SerializeField] private StatusType _statusType = StatusType.PHYSIC_FORCE;
        [SerializeField] private AbilityValueType _valueType = AbilityValueType.PERCENT_OF_CURRENT;
        [SerializeField] private float _value = 25f;
        [SerializeField] private float _duration = 3f;
        [SerializeField] private AbilityRefreshMode _refreshMode = AbilityRefreshMode.REFRESH_DURATION;

        private StatusData _statusToAffect;
       
        private Coroutine _buffRoutine;
        private float _appliedValue;
        private bool _subscribed;
        private bool _buffActive;

        #endregion

        #region Setup

        public void Setup(StatusInfoSourceBase statusSource, PlayerDash dashComponent)
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

            _statusSource.TryGetStatusData(_statusType, out _statusToAffect);

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

        private void PlayerDash_OnDashStarted(DirectionSide directionSide)
        {
            if (_triggerType == AbilityTriggerType.ON_DASH_STARTED)
                Execute();
        }

        private void PlayerDash_OnDashFinished(DirectionSide directionSide)
        {
            if (_triggerType == AbilityTriggerType.ON_DASH_FINISHED)
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

            if (_duration > 0f)
                _buffRoutine = StartCoroutine(BuffRoutine());
        }

        private IEnumerator BuffRoutine()
        {
            yield return new WaitForSeconds(_duration);
            RemoveBuff();
        }

        private void RemoveBuff()
        {
            if (!_buffActive || _statusSource == null || _statusToAffect==null)
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
            if (_valueType == AbilityValueType.FLAT)
                return _value;

            return _statusToAffect.CurrentValue * (_value / 100f);
        }
        #endregion
    }
}
