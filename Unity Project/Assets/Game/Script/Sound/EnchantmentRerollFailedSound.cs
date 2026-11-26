using Scripts.Enchantment;
using UnityEngine;

namespace Scripts.Sound
{
    public class EnchantmentRerollFailedSound : MonoBehaviour
    {
        [SerializeField] private MN_Enchantments _enchantmentsManager;
        [SerializeField] private AudioChannel _channel;
        [SerializeField] private AudioConfig _audioConfig;

        private void OnEnable()
        {
            if (_enchantmentsManager == null)
                _enchantmentsManager = MN_Enchantments.Instance;

            if (_enchantmentsManager != null)
                _enchantmentsManager.OnRerollFailed += HandleRerollFailed;
        }

        private void OnDisable()
        {
            if (_enchantmentsManager != null)
                _enchantmentsManager.OnRerollFailed -= HandleRerollFailed;
        }

        private void HandleRerollFailed()
        {
            if (_channel != null && _audioConfig != null)
                _channel.AudioRequest(_audioConfig, Vector3.zero);
        }
    }
}
