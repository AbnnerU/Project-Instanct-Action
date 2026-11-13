using Scripts.Combat;
using UnityEngine;

namespace Scripts.Weapon
{
    public class WeaponHitEffectBinder : MonoBehaviour
    {
        [SerializeField] private MN_Weapon _weaponManager;
        [SerializeField] private SpawnHitEffectOnDamageDealt _hitEffectSpawner;

        private void OnEnable()
        {
            if (_weaponManager == null) _weaponManager = MN_Weapon.Instance;
            if (_weaponManager == null || _hitEffectSpawner == null) return;

            _weaponManager.OnWeaponLoadoutEquipped += ApplyLoadout;
            ApplyLoadout(_weaponManager.CurrentLoadout);
        }

        private void OnDisable()
        {
            if (_weaponManager != null)
                _weaponManager.OnWeaponLoadoutEquipped -= ApplyLoadout;
        }

        private void ApplyLoadout(WeaponLoadoutSO loadout)
        {
            _hitEffectSpawner.SetEffectPrefab(loadout != null ? loadout.HitEffectPrefab : null);
        }
    }
}
