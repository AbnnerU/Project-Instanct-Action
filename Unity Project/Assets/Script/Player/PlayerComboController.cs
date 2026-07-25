using System.Collections.Generic;
using Scripts.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Player
{
    public class PlayerComboController : MonoBehaviour
    {
        [SerializeField] private InputActionReference _input;
        [SerializeField] private Animator _animator;
        [SerializeField] private PlayerAttackHitbox _attackHitbox;
        [SerializeField] private List<ComboAttackData> _listComboAttackData = new List<ComboAttackData>();

        private int _currentComboIndex;
        private bool _isAttacking;
        private bool _isComboWindowOpen;
        private bool _hasQueuedNextAttack;

        private void Awake()
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();

            if (_attackHitbox == null)
                TryGetComponent(out _attackHitbox);
        }

        private void OnEnable()
        {
            _input.action.Enable();
            _input.action.performed += Action_performed;
        }

        private void Action_performed(InputAction.CallbackContext obj)
        {
            if (_listComboAttackData.Count == 0) return;

            if (!_isAttacking)
            {
                _currentComboIndex = 0;
                PlayCurrentAttack();
                return;
            }

            if (_isComboWindowOpen)
                _hasQueuedNextAttack = true;
        }

        public void CancelAttack()
        {
            _isAttacking = false;
            _isComboWindowOpen = false;
            _hasQueuedNextAttack = false;
            _currentComboIndex = 0;
        }

        public void AnimationEvent_ExecuteHitbox()
        {
            if (!CanUseCurrentAttack()) return;

            _attackHitbox.ExecuteHitbox(_listComboAttackData[_currentComboIndex]);
        }

        public void AnimationEvent_OpenComboWindow()
        {
            _isComboWindowOpen = true;
        }

        public void AnimationEvent_CloseComboWindow()
        {
            _isComboWindowOpen = false;
        }

        public void AnimationEvent_FinishAttack()
        {
            _isComboWindowOpen = false;

            if (_hasQueuedNextAttack && _currentComboIndex < _listComboAttackData.Count - 1)
            {
                _currentComboIndex++;
                PlayCurrentAttack();
                return;
            }

            CancelAttack();
        }

        private void PlayCurrentAttack()
        {
            if (!CanUseCurrentAttack()) return;

            ComboAttackData attackData = _listComboAttackData[_currentComboIndex];

            if (_animator == null || string.IsNullOrEmpty(attackData.AnimationStateName))
            {
                Debug.LogWarning("Animator or animation state name is missing.", this);
                return;
            }

            _isAttacking = true;
            _isComboWindowOpen = false;
            _hasQueuedNextAttack = false;

            _animator.CrossFade(attackData.AnimationStateName, attackData.TransitionDuration, 0, 0f);
        }

        private bool CanUseCurrentAttack()
        {
            return _attackHitbox != null &&
                   _currentComboIndex >= 0 &&
                   _currentComboIndex < _listComboAttackData.Count;
        }
    }
}
