using Scripts.Animation;
using System.Collections;
using UnityEngine;

namespace Scripts.Status
{
    public class AttackSpeedByStatus : AnimationModuleBase
    {
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private string _attackSpeedParameter = "AttackSpeed";
        [Tooltip("Attack speed percentage. 100 = normal speed, 150 = 1.5x.")]
        [SerializeField] private float _defaultAttackSpeed = 100f;
        [Tooltip("Minimum attack speed percentage. 1 = 1% of normal speed.")]
        [SerializeField] private float _minimumAttackSpeed = 1f;

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
            _animationControllerComponent.SetFloat(_attackSpeedParameter, Mathf.Max(_minimumAttackSpeed, attackSpeed) / 100f);
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
