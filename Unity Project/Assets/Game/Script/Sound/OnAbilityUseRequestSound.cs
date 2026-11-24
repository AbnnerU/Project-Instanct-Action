using Scripts.Abilities;
using UnityEngine;

namespace Scripts.Sound
{
    public class OnAbilityUseRequestSound : MonoBehaviour
    {
        [SerializeField] private AbilityBase _abilityComponent;
        [SerializeField] private ExecutionType _executionType;
        [SerializeField] private AudioChannel _channel;
        [SerializeField] private AudioConfig _audioConfig;
        [SerializeField] private Transform _positionReference;

        enum ExecutionType
        {
            ON_ABILITY_STARTED,
            ON_ABILITY_ENDED
        }

        private void Awake()
        {
            if (_abilityComponent == null)
                _abilityComponent = GetComponent<AbilityBase>();

            if (_executionType == ExecutionType.ON_ABILITY_STARTED)
                _abilityComponent.OnAbilityStarted += StartCooldown;
            else
                _abilityComponent.OnAbilityFinished += StartCooldown;

            _abilityComponent.OnAbilityDeactivating += Ability_Deactivating;
        }

        private void Ability_Deactivating(AbilityBase @base)
        {
            //
        }

        private void StartCooldown(AbilityBase @base)
        {
            if (_channel == null || _audioConfig == null)
                return;

            Vector3 position = _positionReference != null ? _positionReference.position : Vector3.zero;
            _channel.AudioRequest(_audioConfig, position);
        }
    }
}
