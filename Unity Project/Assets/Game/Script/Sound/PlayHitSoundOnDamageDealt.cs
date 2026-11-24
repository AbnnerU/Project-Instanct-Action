using Scripts.Combat;
using UnityEngine;

namespace Scripts.Sound
{
    public class PlayHitSoundOnDamageDealt : MonoBehaviour
    {
        [Tooltip("GameObject with IDamageDealer. Defaults to this GameObject.")]
        [SerializeField] private GameObject _damageDealerObject;
        [SerializeField] private AudioChannel _channel;
        [SerializeField] private AudioConfig _audioConfig;

        private IDamageDealer _damageDealer;

        public void SetAudioConfig(AudioConfig config) => _audioConfig = config;

        private void OnEnable()
        {
            GameObject source = _damageDealerObject != null ? _damageDealerObject : gameObject;
            _damageDealer = source.GetComponent<IDamageDealer>();
            if (_damageDealer == null)
            {
                Debug.LogWarning($"[{nameof(PlayHitSoundOnDamageDealt)}] Missing IDamageDealer.", this);
                return;
            }

            _damageDealer.OnDamageDealt += HandleDamageDealt;
        }

        private void OnDisable()
        {
            if (_damageDealer != null)
                _damageDealer.OnDamageDealt -= HandleDamageDealt;
            _damageDealer = null;
        }

        private void HandleDamageDealt(DamageResult result)
        {
            if (!result.WasApplied || _channel == null || _audioConfig == null)
                return;

            _channel.AudioRequest(_audioConfig, Vector3.zero);
        }
    }
}
