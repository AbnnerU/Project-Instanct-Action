using UnityEngine;

namespace Scripts.Animation
{
    public class OnDeath_TriggerParameter : OnDeathActionBase
    {
        [SerializeField] private CharacterAnimationController _animationControllerComponent;
        [SerializeField] private string _deathParameter = "Death";

        protected override void Awake()
        {
            if (_animationControllerComponent == null)
                TryGetComponent(out _animationControllerComponent);

            base.Awake();
        }

        protected override void HealthEvents_OnDeath()
        {
            if (_animationControllerComponent == null || string.IsNullOrEmpty(_deathParameter))
                return;

            _animationControllerComponent.SetTrigger(_deathParameter);
        }
    }
}
