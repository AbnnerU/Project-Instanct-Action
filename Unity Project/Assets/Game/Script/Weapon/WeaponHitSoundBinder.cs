using Scripts.Sound;
using UnityEngine;

namespace Scripts.Weapon
{
    public class WeaponHitSoundBinder : MonoBehaviour
    {
        [SerializeField] private MN_Weapon _weaponManager;
        [SerializeField] private PlayHitSoundOnDamageDealt _hitSound;

        private void OnEnable()
        {
            if (_weaponManager == null) _weaponManager = MN_Weapon.Instance;
            if (_weaponManager == null || _hitSound == null) return;

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
            _hitSound.SetAudioConfig(loadout != null ? loadout.HitAudioConfig : null);
        }
    }
}
