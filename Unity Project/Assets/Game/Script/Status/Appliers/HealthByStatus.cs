using UnityEngine;

namespace Scripts.Status
{
    [DefaultExecutionOrder(200)]
    public class HealthByStatus : MonoBehaviour
    {
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private HealthManager _healthManager;
        [SerializeField] private bool _healDifferenceWhenMaxHealthIncreases;
        [SerializeField] private bool _clampCurrentHealthWhenMaxHealthDecreases = true;
        [SerializeField] private bool _resetHealthOnEnable;

        private StatusData _healthStatusData;

        private void Awake()
        {
            if (_healthManager == null)
                TryGetComponent(out _healthManager);
        }

        private void OnEnable()
        {
            RefreshHealth();
        }

        private void Start()
        {
            if (_healthStatusData == null)
                RefreshHealth();
        }

        private void RefreshHealth()
        {
            CacheHealthStatus();

            if (_healthStatusData == null || _healthManager == null)
                return;

            ApplyHealthValue(_healthStatusData.CurrentValue);

            if (_resetHealthOnEnable)
                _healthManager.ResetHealth();
        }

        private void OnDisable()
        {
            if (_healthStatusData != null)
                _healthStatusData.OnCurrentValueUpdated -= HealthStatusData_OnCurrentValueUpdated;

            _healthStatusData = null;
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
            if (_healthStatusData != null)
                _healthStatusData.OnCurrentValueUpdated -= HealthStatusData_OnCurrentValueUpdated;

            _healthStatusData = null;

            if (_statusSource == null)
                return;

            if (!_statusSource.TryGetStatusData(StatusType.HEALTH, out _healthStatusData))
                return;

            _healthStatusData.OnCurrentValueUpdated += HealthStatusData_OnCurrentValueUpdated;
        }
    }
}
