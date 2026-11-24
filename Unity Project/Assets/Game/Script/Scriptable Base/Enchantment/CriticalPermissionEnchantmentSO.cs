using Scripts.Combat;
using UnityEngine;

namespace Scripts.Enchantment
{
    [CreateAssetMenu(fileName = "CriticalPermissionEnchantmentSO", menuName = "Scriptable Objects/Enchantment/Critical Permission")]
    public class CriticalPermissionEnchantmentSO : EnchantmentSOBase
    {
        [SerializeField] private CriticalDamageTypes _allowedTypes = CriticalDamageTypes.MAGIC | CriticalDamageTypes.TRUE;

        public override void Apply(EnchantmentContext context)
        {
            if (context == null || context.TargetObject == null) return;

            ICriticalDamagePermissionsProvider permissions =
                context.TargetObject.GetComponent<ICriticalDamagePermissionsProvider>();

            if (permissions == null)
            {
                Debug.LogWarning($"[{nameof(CriticalPermissionEnchantmentSO)}] Target has no critical permissions provider.", context.TargetObject);
                return;
            }

            permissions.Allow(_allowedTypes);
        }
    }
}
