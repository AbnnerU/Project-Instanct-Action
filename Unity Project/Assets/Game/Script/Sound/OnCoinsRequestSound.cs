using Scripts.Economy;
using UnityEngine;

namespace Scripts.Sound
{
    public class OnCoinsRequestSound : MonoBehaviour
    {
        [SerializeField] private MN_Coins _coinsManager;
        [SerializeField] private AudioChannel _channel;
        [SerializeField] private AudioConfig _onGainAudioConfig;
        [SerializeField] private AudioConfig _onRemoveAudioConfig;

        private bool _subscribed;

        private void OnEnable() => Subscribe();
        private void Start() => Subscribe();

        private void Subscribe()
        {
            if (_subscribed) return;
            if (_coinsManager == null) _coinsManager = MN_Coins.Instance;
            if (_coinsManager == null) return;

            _coinsManager.OnAdd += OnGain;
            _coinsManager.OnRemove += OnRemove;
            _subscribed = true;
        }

        private void OnDisable()
        {
            if (_subscribed && _coinsManager != null)
            {
                _coinsManager.OnAdd -= OnGain;
                _coinsManager.OnRemove -= OnRemove;
            }
            _subscribed = false;
        }

        private void OnGain(int amount) => Play(_onGainAudioConfig, amount);
        private void OnRemove(int amount) => Play(_onRemoveAudioConfig, amount);

        private void Play(AudioConfig config, int amount)
        {
            if (amount > 0 && _channel != null && config != null)
                _channel.AudioRequest(config, Vector3.zero);
        }
    }
}
