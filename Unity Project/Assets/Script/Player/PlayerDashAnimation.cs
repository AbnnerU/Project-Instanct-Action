using Scripts.Animation;
using UnityEngine;

namespace Scripts.Player
{
    public class PlayerDashAnimation : AnimationModuleBase
    {
        [SerializeField] private PlayerDash _playerDash;
        [SerializeField] private string _dashTriggerParameter = "Dash";

        protected override void Awake()
        {
            base.Awake();

            if (_playerDash == null)
                TryGetComponent(out _playerDash);
        }

        private void OnEnable()
        {
            if (_playerDash != null)
                _playerDash.OnDashStarted += PlayerDash_OnDashStarted;
        }

        private void OnDisable()
        {
            if (_playerDash != null)
                _playerDash.OnDashStarted -= PlayerDash_OnDashStarted;
        }

        private void PlayerDash_OnDashStarted(DirectionSide directionSide)
        {
            if (_animationControllerComponent == null) return;

            _animationControllerComponent.SetTrigger(_dashTriggerParameter);
        }
    }
}
