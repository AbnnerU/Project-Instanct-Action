using UnityEngine;

namespace Scripts.Combat
{
    public class CriticalPermissions : MonoBehaviour, ICriticalDamagePermissionsProvider
    {
        [SerializeField] private CriticalDamageTypes _allowedTypes = DamageRequest.DefaultCriticalTypes;

        public CriticalDamageTypes GetAllowedCriticalDamageTypes() => _allowedTypes;

        public void SetAllowedCriticalDamageTypes(CriticalDamageTypes types) => _allowedTypes = types;

        public void Allow(CriticalDamageTypes types) => _allowedTypes |= types;
    }
}
