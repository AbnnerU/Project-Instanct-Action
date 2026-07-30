using Scripts.Managers;
using UnityEngine;

namespace Scripts.Status
{
    public class PlayerHealthByStatus : MonoBehaviour
    {
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private HealthManager _healthManager;
        [SerializeField] private bool _healDifferenceWhenMaxHealthIncreases;
        [SerializeField] private bool _clampCurrentHealthWhenMaxHealthDecreases = true;

        private StatusData _healthStatusData;

        private void Awake()
        {
            if (_healthManager == null)
                TryGetComponent(out _healthManager);
        }

        private void Start()
        {
            CacheHealthStatus();

            if (_healthStatusData != null)
                ApplyHealthValue(_healthStatusData.CurrentValue);
        }

        private void OnDestroy()
        {
            if (_healthStatusData != null)
                _healthStatusData.OnCurrentValueUpdated -= HealthStatusData_OnCurrentValueUpdated;
        }

        private void HealthStatusData_OnCurrentValueUpdated(float value)
        {
            ApplyHealthValue(value);
        }

        private void ApplyHealthValue(float value)
        {
            if (_healthManager == null)
                return;

            float newMaxHealth = Mathf.Max(1f, value);
            float previousMaxHealth = _healthManager.GetMaxHealth();
            float currentHealth = _healthManager.GetCurrentHealth();

            _healthManager.SetMaxHealthValue(newMaxHealth, false);

            if (_healDifferenceWhenMaxHealthIncreases && newMaxHealth > previousMaxHealth)
            {
                _healthManager.SetCurrentHealthValue(currentHealth + (newMaxHealth - previousMaxHealth));
                return;
            }

            if (_clampCurrentHealthWhenMaxHealthDecreases && currentHealth > newMaxHealth)
                _healthManager.SetCurrentHealthValue(newMaxHealth);
        }

        private void CacheHealthStatus()
        {
            if (_statusSource == null)
                return;

            if (!_statusSource.TryGetStatusData(StatusType.ATTACK_SPEED, out _healthStatusData))
                return;

            _healthStatusData.OnCurrentValueUpdated += HealthStatusData_OnCurrentValueUpdated;
        }
    }
}
