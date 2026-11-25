using UnityEngine;

namespace Scripts.Sound
{
    public class OnHitRequestSound : MonoBehaviour
    {
        [SerializeField] private HealthManager _healthManager;
        [SerializeField] private AudioChannel _channel;
        [SerializeField] private AudioConfig _hitAudioConfig;
        [SerializeField] private bool _useDeathSound = true;
        [SerializeField] private AudioConfig _deathAudioConfig;

        [Header("Hit Sound Delay")]
        [SerializeField] private bool _useDelay;
        [SerializeField, Min(0f)] private float _delay = 0.2f;

        private float _nextHitSoundTime;

        private void OnEnable()
        {
            _nextHitSoundTime = float.NegativeInfinity;
        }

        private void Awake()
        {
            if (_healthManager == null)
                _healthManager = GetComponent<HealthManager>();

            if (_healthManager == null)
            {
                Debug.LogWarning($"[{nameof(OnHitRequestSound)}] Missing HealthManager.", this);
                return;
            }

            _healthManager.OnHitted += HandleHit;
        }

        private void OnDestroy()
        {
            if (_healthManager != null)
                _healthManager.OnHitted -= HandleHit;
        }

        private void HandleHit(int damage, bool isAlive)
        {
            if (!enabled || _channel == null || (isAlive && !gameObject.activeInHierarchy))
                return;

            if (isAlive && _useDelay && Time.time < _nextHitSoundTime)
                return;

            AudioConfig config = !isAlive && _useDeathSound && _deathAudioConfig != null
                ? _deathAudioConfig
                : _hitAudioConfig;

            if (config != null)
            {
                _nextHitSoundTime = Time.time + Mathf.Max(0f, _delay);
                _channel.AudioRequest(config, Vector3.zero);
            }
        }
    }
}
