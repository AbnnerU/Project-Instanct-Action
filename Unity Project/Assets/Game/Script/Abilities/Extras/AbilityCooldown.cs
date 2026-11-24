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

        private float _currentTime;
        private bool _isCoolingDown;

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
            _abilityComponent.OnAbilityDeactivating += Ability_Deactivating;
        }

        private void Ability_Deactivating(AbilityBase ability)
        {
            StopAllCoroutines();
            _isCoolingDown = false;
            _abilityComponent.OnAbilityStarted -= StartCooldown;
            _abilityComponent.OnAbilityFinished -= StartCooldown;
            _abilityComponent.OnAbilityDeactivating -= Ability_Deactivating;
        }

        private void OnDestroy()
        {
            if (_abilityComponent != null) Ability_Deactivating(_abilityComponent);
        }

        protected void StartCooldown(AbilityBase abilityBase)
        {
            if (!abilityBase.Initialized) return;
            StopAllCoroutines();
            _currentTime = 0f;
            _isCoolingDown = true;

            StartCoroutine(CooldownRoutine());
        }

        IEnumerator CooldownRoutine()
        {
            _abilityComponent.Disable();
            OnCooldownStarted?.Invoke(_cooldown);

            do
            {
                yield return null;
                _currentTime += Time.deltaTime;

            } while (_currentTime < _cooldown);

            CompleteCooldown();
        }

        public void ReduceRemainingCooldown(float seconds)
        {
            if (!_isCoolingDown || seconds <= 0f) return;

            _currentTime = Mathf.Min(_currentTime + seconds, _cooldown);
            if (_currentTime < _cooldown) return;

            StopAllCoroutines();
            CompleteCooldown();
        }

        private void CompleteCooldown()
        {
            if (!_isCoolingDown) return;

            _isCoolingDown = false;
            _currentTime = _cooldown;
            _abilityComponent.Enable();
            OnCooldownCompleted?.Invoke(_cooldown);
        }
    }
}
