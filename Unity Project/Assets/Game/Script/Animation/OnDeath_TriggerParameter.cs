using UnityEngine;

namespace Scripts.Animation
{
    public class OnDeath_TriggerParameter : LifeCycleActionsBase
    {
        [SerializeField] private CharacterAnimationController _animationControllerComponent;
        [SerializeField] private bool _boolParameter = true;
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

            if (_boolParameter)
                _animationControllerComponent.SetBool(_deathParameter, true);
            else
                _animationControllerComponent.SetTrigger(_deathParameter);
        }

        protected override void HealthEvents_OnRespawn()
        {
            if (_animationControllerComponent == null || string.IsNullOrEmpty(_deathParameter))
                return;

            if (_boolParameter)
                _animationControllerComponent.SetBool(_deathParameter, false);
        }
    }
}
