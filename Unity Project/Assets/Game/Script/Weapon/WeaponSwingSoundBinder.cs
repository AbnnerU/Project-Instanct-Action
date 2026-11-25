using Scripts.Sound;
using UnityEngine;

namespace Scripts.Weapon
{
    public class WeaponSwingSoundBinder : MonoBehaviour
    {
        [SerializeField] private MN_Weapon _weaponManager;
        [SerializeField] private AttackSwingSound _swingSound;

        private void OnEnable()
        {
            if (_weaponManager == null) _weaponManager = MN_Weapon.Instance;
            if (_weaponManager == null || _swingSound == null) return;

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
            _swingSound.SetAudioConfig(loadout != null ? loadout.SwingAudioConfig : null);
        }
    }
}
