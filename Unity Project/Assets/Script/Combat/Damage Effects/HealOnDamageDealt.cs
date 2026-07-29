using System.Collections.Generic;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Combat
{
    public class HealOnDamageDealt : MonoBehaviour
    {
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private StatusType _healingStatusType = StatusType.LIFE_STEAL;
        [SerializeField] private List<DamageType> _list_ValidDamageTypes = new List<DamageType> { DamageType.PHYSICAL };

        private IDamageDealer _damageDealer;
        private IHealable _healableHandler;

        private void Awake()
        {
            CacheReferences();
        }

        private void OnEnable()
        {
            if (_damageDealer == null)
                CacheReferences();

            if (_damageDealer != null)
                _damageDealer.OnDamageDealt += DamageDealer_OnDamageDealt;
        }

        private void OnDisable()
        {
            if (_damageDealer != null)
                _damageDealer.OnDamageDealt -= DamageDealer_OnDamageDealt;
        }

        private void DamageDealer_OnDamageDealt(DamageResult damageResult)
        {
            if (damageResult.FinalDamage <= 0f || !IsValidDamageType(damageResult.DamageType))
                return;

            if (_healableHandler == null)
                CacheReferences();

            if (_healableHandler == null)
                return;

            float healPercentage = GetHealingPercentage();
            if (healPercentage <= 0f)
                return;

            _healableHandler.Heal(damageResult.FinalDamage * healPercentage);
        }

        private float GetHealingPercentage()
        {
            StatusInfo statusInfo = _statusSource != null ? _statusSource.GetStatusInfo() : null;
            if (statusInfo == null) return 0f;

            return Mathf.Max(0f, statusInfo.GetCurrentValueOf(_healingStatusType)) / 100f;
        }

        private bool IsValidDamageType(DamageType damageType)
        {
            return _list_ValidDamageTypes != null && _list_ValidDamageTypes.Contains(damageType);
        }

        private void CacheReferences()
        {
            _damageDealer = GetComponent<IDamageDealer>();

            _healableHandler = GetComponent<IHealable>();
        }
    }
}
