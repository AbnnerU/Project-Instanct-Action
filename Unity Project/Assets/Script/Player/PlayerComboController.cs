using System.Collections.Generic;
using Scripts.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Player
{
    public class PlayerComboController : MonoBehaviour, IHasActiveState
    {
        [SerializeField] private InputActionReference _rightInput;
        [SerializeField] private InputActionReference _leftInput;
        [SerializeField] private Animator _animator;
        [SerializeField] private PlayerDirection _playerDirection;
        [SerializeField] private PlayerAttackHitbox _attackHitbox;
        [SerializeField] private List<ComboAttackData> _listComboAttackData = new List<ComboAttackData>();
        [SerializeField] private string _idleAnimationStateName = "Idle";
        [SerializeField] private float _idleTransitionDuration = 0.05f;
        [SerializeField] private DirectionSide _fallbackInputDirection = DirectionSide.RIGHT;
        [SerializeField] private bool _active = true;

        private int _currentComboIndex;
        private bool _isAttacking;
        private bool _isComboWindowOpen;
        private bool _hasQueuedNextAttack;
        private DirectionSide _queuedAttackDirection;

        private void Awake()
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();

            if (_playerDirection == null)
                TryGetComponent(out _playerDirection);

            if (_attackHitbox == null)
                TryGetComponent(out _attackHitbox);
        }

        private void OnEnable()
        {
            if (_rightInput == null) return;

            _rightInput.action.Enable();
            _rightInput.action.performed += AttackRight_Performed;

            _leftInput.action.Enable();
            _leftInput.action.performed += AttackLeft_Performed;

        }

        private void OnDisable()
        {
            if (_rightInput == null) return;

            _rightInput.action.performed -= AttackRight_Performed;
            _leftInput.action.performed -= AttackLeft_Performed;
        }

        private void AttackRight_Performed(InputAction.CallbackContext obj) => RequestAttackRight();

        private void AttackLeft_Performed(InputAction.CallbackContext obj) => RequestAttackLeft();

        public void Enable()
        {
            _active = true;
        }

        public void Disable()
        {
            _active = false;
            CancelAttack();
        }

        public void RequestAttackRight()
        {
            RequestAttack(DirectionSide.RIGHT);
        }

        public void RequestAttackLeft()
        {
            RequestAttack(DirectionSide.LEFT);
        }

        public void RequestAttack(DirectionSide directionSide)
        {
            if (!_active) return;
            if (_listComboAttackData.Count == 0) return;

            if (!_isAttacking)
            {
                _currentComboIndex = 0;
                PlayCurrentAttack(directionSide);
                return;
            }

            if (_isComboWindowOpen || _hasQueuedNextAttack)
            {
                _hasQueuedNextAttack = true;
                _queuedAttackDirection = directionSide;
            }
        }

        public void CancelAttack()
        {
            _isAttacking = false;
            _isComboWindowOpen = false;
            _hasQueuedNextAttack = false;
            _queuedAttackDirection = _fallbackInputDirection;
            _currentComboIndex = 0;

            PlayIdle();
        }

        public void AnimationEvent_ExecuteHitbox()
        {
            if (!_active) return;
            if (!CanUseCurrentAttack()) return;

            _attackHitbox.ExecuteHitbox(_listComboAttackData[_currentComboIndex]);
        }

        public void AnimationEvent_OpenComboWindow()
        {
            if (!_active) return;

            _isComboWindowOpen = true;
        }

        public void AnimationEvent_CloseComboWindow()
        {
            _isComboWindowOpen = false;
        }

        public void AnimationEvent_FinishAttack()
        {
            if (!_active) return;

            _isComboWindowOpen = false;

            if (_hasQueuedNextAttack && _currentComboIndex < _listComboAttackData.Count - 1)
            {
                _currentComboIndex++;
                PlayCurrentAttack(_queuedAttackDirection);
                return;
            }

            CancelAttack();
        }

        private void PlayCurrentAttack(DirectionSide directionSide)
        {
            if (!_active) return;
            if (!CanUseCurrentAttack()) return;

            ComboAttackData attackData = _listComboAttackData[_currentComboIndex];

            if (_animator == null || string.IsNullOrEmpty(attackData.AnimationStateName))
            {
                Debug.LogWarning("Animator or animation state name is missing.", this);
                return;
            }

            _playerDirection?.SetDirection(directionSide);

            _isAttacking = true;
            _isComboWindowOpen = false;
            _hasQueuedNextAttack = false;
            _queuedAttackDirection = directionSide;

            _animator.CrossFade(attackData.AnimationStateName, attackData.TransitionDuration, 0, 0f);
        }

        private void PlayIdle()
        {
            if (_animator == null || string.IsNullOrEmpty(_idleAnimationStateName)) return;

            _animator.CrossFade(_idleAnimationStateName, _idleTransitionDuration, 0, 0f);
        }

        private bool CanUseCurrentAttack()
        {
            return _attackHitbox != null &&
                   _currentComboIndex >= 0 &&
                   _currentComboIndex < _listComboAttackData.Count;
        }
    }
}
