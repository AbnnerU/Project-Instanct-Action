using Scripts.Animation;
using System.Collections;
using UnityEngine;

namespace Scripts.Status
{
    public class AttackSpeedByStatus : AnimationModuleBase
    {
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private string _attackSpeedParameter = "AttackSpeed";
        [SerializeField] private float _defaultAttackSpeed = 1f;
        [SerializeField] private float _minimumAttackSpeed = 0.01f;

        private StatusData _attackSpeedStatusData;

        IEnumerator Start()
        {
            yield return null;

            CacheAttackSpeedStatus();

            if (_attackSpeedStatusData != null)
                ApplyAttackSpeed(_attackSpeedStatusData.CurrentValue);
            else
                ApplyAttackSpeed(_defaultAttackSpeed);
        }

        private void OnDestroy()
        {
            if (_attackSpeedStatusData != null)
                _attackSpeedStatusData.OnCurrentValueUpdated -= AttackSpeedStatusData_OnCurrentValueUpdated;
        }

        private void AttackSpeedStatusData_OnCurrentValueUpdated(float value)
        {
            ApplyAttackSpeed(value);
        }

        private void ApplyAttackSpeed(float value)
        {
            if (_animationControllerComponent == null)
                return;

            float attackSpeed = value > 0f ? value : _defaultAttackSpeed;
            _animationControllerComponent.SetFloat(_attackSpeedParameter, Mathf.Max(_minimumAttackSpeed, attackSpeed));
        }

        private void CacheAttackSpeedStatus()
        {
            if (_statusSource == null)
                return;

            if (!_statusSource.TryGetStatusData(StatusType.ATTACK_SPEED, out _attackSpeedStatusData))
                return;

            _attackSpeedStatusData.OnCurrentValueUpdated += AttackSpeedStatusData_OnCurrentValueUpdated;
        }
    }
}
