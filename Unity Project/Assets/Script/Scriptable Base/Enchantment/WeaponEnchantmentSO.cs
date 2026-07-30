using Scripts.Weapon;
using UnityEngine;

namespace Scripts.Enchantment
{
    [CreateAssetMenu(fileName = "WeaponEnchantmentSO", menuName = "Scriptable Objects/Enchantment/Weapon")]
    public class WeaponEnchantmentSO : EnchantmentSOBase
    {
        [SerializeField] private WeaponLoadoutSO _weaponLoadout;

        public WeaponLoadoutSO WeaponLoadout => _weaponLoadout;

        public override void Apply(EnchantmentContext context)
        {
            if (_weaponLoadout == null || MN_PlayerWeapon.Instance == null)
                return;

            MN_PlayerWeapon.Instance.EquipWeaponLoadout(_weaponLoadout);
        }
    }
}
