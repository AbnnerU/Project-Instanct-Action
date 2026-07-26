using System;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Animation;

namespace Scripts.Combat
{
    public abstract class ComboControllerBase : MonoBehaviour, IHasActiveState
    {
        public event Action OnComboStarted;
        public event Action OnComboFinished;
        public event Action OnAttackStarted;
        public event Action OnAttackFinished;
        public event Action OnAttackCanceled;

        [SerializeField] protected bool _active = true;
        [SerializeField] protected AttackHitbox _attackHitbox;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected CharacterAnimationController _characterAnimationController;
        [SerializeField] protected List<ComboAttackData> _list_ComboAttackData = new List<ComboAttackData>();
        [SerializeField] protected bool _triggerIdle=true;
        [SerializeField] protected string _backToIdleTriggerParameter = "Idle";


        protected int _currentComboIndex;
        protected bool _isAttacking;
        protected bool _isComboWindowOpen;
        protected bool _hasQueuedNextAttack;

        public bool Active => _active;
        public bool IsAttacking => _isAttacking;
        public bool IsComboWindowOpen => _isComboWindowOpen;
        public bool HasQueuedNextAttack => _hasQueuedNextAttack;

        protected virtual void Awake()
        {
            GetReferences();
        }

        public virtual void Enable()
        {
            _active = true;
        }

        public virtual void Disable()
        {
            _active = false;
            CancelAttack();
        }

        public virtual void RequestAttack()
        {
            if (!_active) return;
            if (_list_ComboAttackData.Count == 0) return;

            if (!_isAttacking)
            {
                _currentComboIndex = 0;
                PlayCurrentAttack();
                OnComboStarted?.Invoke();
                return;
            }

            if (_isComboWindowOpen || _hasQueuedNextAttack)
                QueueNextAttack();
        }

        public virtual void CancelAttack()
        {
            bool wasAttacking = _isAttacking;

            _isAttacking = false;
            _isComboWindowOpen = false;
            _hasQueuedNextAttack = false;
            _currentComboIndex = 0;

            PlayIdle();

            if (wasAttacking)
                OnAttackCanceled?.Invoke();
        }

        public virtual void AnimationEvent_ExecuteHitbox()
        {
            if (!_active) return;
            if (!CanUseCurrentAttack()) return;

            _attackHitbox.ExecuteHitbox(_list_ComboAttackData[_currentComboIndex]);
        }

        public virtual void AnimationEvent_OpenComboWindow()
        {
            if (!_active) return;

            _isComboWindowOpen = true;
        }

        public virtual void AnimationEvent_CloseComboWindow()
        {
            _isComboWindowOpen = false;
        }

        public virtual void AnimationEvent_FinishAttack()
        {
            if (!_active) return;

            _isComboWindowOpen = false;
            OnAttackFinished?.Invoke();

            if (_hasQueuedNextAttack && _currentComboIndex < _list_ComboAttackData.Count - 1)
            {
                _currentComboIndex++;
                PlayCurrentAttack();
                return;
            }

            FinishCombo();
        }

        protected virtual void GetReferences()
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();

            if (_attackHitbox == null)
                TryGetComponent(out _attackHitbox);
        }

        protected virtual void QueueNextAttack()
        {
            _hasQueuedNextAttack = true;
        }

        protected virtual void PlayCurrentAttack()
        {
            if (!_active) return;
            if (!CanUseCurrentAttack()) return;

            ComboAttackData attackData = _list_ComboAttackData[_currentComboIndex];

            if (_animator == null || string.IsNullOrEmpty(attackData.AnimationStateName))
            {
                Debug.LogWarning("Animator or animation state name is missing.", this);
                return;
            }

            _isAttacking = true;
            _isComboWindowOpen = false;
            _hasQueuedNextAttack = false;

            _animator.CrossFade(attackData.AnimationStateName, attackData.TransitionDuration, 0, 0f);
            OnAttackStarted?.Invoke();
        }

        protected virtual void FinishCombo()
        {
            _isAttacking = false;
            _isComboWindowOpen = false;
            _hasQueuedNextAttack = false;
            _currentComboIndex = 0;

            PlayIdle();
            OnComboFinished?.Invoke();
        }

        protected virtual void PlayIdle()
        {
            if (!_triggerIdle || _animator == null || string.IsNullOrEmpty(_backToIdleTriggerParameter)) return;

            _characterAnimationController.SetTrigger(_backToIdleTriggerParameter);
        }

        protected virtual bool CanUseCurrentAttack()
        {
            return _attackHitbox != null &&
                   _currentComboIndex >= 0 &&
                   _currentComboIndex < _list_ComboAttackData.Count;
        }
    }
}
