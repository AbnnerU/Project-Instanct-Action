using Scripts.Animation;
using UnityEngine;

namespace Scripts.DashHandler
{
    public class DashAnimation : AnimationModuleBase
    {
        #region Fields

        [SerializeField] private DashBase _playerDash;
        [SerializeField] private string _dashDirectionParameter = "DashDirection";
        [SerializeField] private bool _useDirectionSign;
        [SerializeField] private string _dashTriggerParameter = "Dash";

        #endregion

        #region Unity

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

        #endregion

        #region Animation

        private void PlayerDash_OnDashStarted(float directionDot)
        {
            if (_animationControllerComponent == null) return;

            float value = _useDirectionSign
                ? (Mathf.Abs(directionDot) < 0.0001f ? 0f : Mathf.Sign(directionDot))
                : directionDot;
            _animationControllerComponent.SetFloat(_dashDirectionParameter, value);
            _animationControllerComponent.SetTrigger(_dashTriggerParameter);
        }

        #endregion
    }
}
