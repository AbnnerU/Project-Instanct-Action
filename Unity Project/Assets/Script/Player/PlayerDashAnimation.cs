using UnityEngine;

namespace Scripts.Player
{
    public class PlayerDashAnimation : MonoBehaviour
    {
        [SerializeField] private PlayerDash _playerDash;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _dashAnimationStateName = "Dash";
        [SerializeField] private float _transitionDuration = 0.05f;

        private void Awake()
        {
            if (_playerDash == null)
                TryGetComponent(out _playerDash);

            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
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
            if (_animator == null || string.IsNullOrEmpty(_dashAnimationStateName)) return;

            _animator.CrossFade(_dashAnimationStateName, _transitionDuration, 0, 0f);
        }
    }
}
