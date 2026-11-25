using System;
using System.Collections.Generic;
using Scripts.Enchantment;
using Scripts.TierManipulation;
using UnityEngine;

namespace Scripts.Sound
{
    public class EnchantmentRerollSound : MonoBehaviour
    {
        [Serializable]
        public class TierAudio
        {
            public Tier _tier;
            public AudioConfig _audioConfig;
        }

        [SerializeField] private MN_Enchantments _enchantmentsManager;
        [SerializeField] private AudioChannel _channel;
        [SerializeField] private List<TierAudio> _tierAudios = new List<TierAudio>();

        private void OnEnable()
        {
            if (_enchantmentsManager == null)
                _enchantmentsManager = MN_Enchantments.Instance;

            if (_enchantmentsManager != null)
                _enchantmentsManager.OnRerolled += HandleRerolled;
        }

        private void OnDisable()
        {
            if (_enchantmentsManager != null)
                _enchantmentsManager.OnRerolled -= HandleRerolled;
        }

        private void HandleRerolled(Tier tier)
        {
            if (_channel == null) return;

            foreach (TierAudio entry in _tierAudios)
            {
                if (entry == null || entry._tier != tier) continue;

                if (entry._audioConfig != null)
                    _channel.AudioRequest(entry._audioConfig, Vector3.zero);
                return;
            }
        }
    }
}
