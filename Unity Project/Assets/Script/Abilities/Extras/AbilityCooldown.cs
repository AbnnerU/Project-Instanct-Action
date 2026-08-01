using Scripts.Timer;
using System;
using System.Collections;
using UnityEngine;

namespace Scripts.Abilities.Extras
{
    public class AbilityCooldown : MonoBehaviour, IHasCooldown
    {
        public event Action<float> OnCooldownStarted;
        public event Action<float> OnCooldownCompleted;

        [SerializeField] private AbilityBase _abilityComponent;
        [SerializeField] private float _cooldown;
        [SerializeField] private CooldownType _cooldownType;

        private WaitForSeconds _delay;
        private float _currentTime;

        public float Cooldown { get => _cooldown; set => _cooldown = value; }
        public float CooldownProgression => _currentTime;

        enum CooldownType
        {
            ON_ABILITY_STARTED,
            ON_ABILITY_ENDED
        }

        protected void Awake()
        {
            if (_abilityComponent == null)
               _abilityComponent = GetComponent<AbilityBase>();

            if (_cooldownType == CooldownType.ON_ABILITY_STARTED)
                _abilityComponent.OnAbilityStarted += StartCooldown;
            else
                _abilityComponent.OnAbilityFinished += StartCooldown;
        }

        protected void StartCooldown(AbilityBase abilityBase)
        {
            StopAllCoroutines();
            _currentTime = 0f;

            StartCoroutine(CooldownRoutine());
        }

        IEnumerator CooldownRoutine()
        {
            OnCooldownStarted?.Invoke(_cooldown);

            _abilityComponent.Disable();

            do
            {
                yield return null;
                _currentTime += Time.deltaTime;

            } while (_currentTime < _cooldown);

            _abilityComponent.Enable();

            OnCooldownCompleted?.Invoke(_cooldown);
        }
    }
}
