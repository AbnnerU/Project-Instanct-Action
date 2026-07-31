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
        public event Action<AttackExecutionData> OnAttackHitboxExecuted;
        public event Action<AttackExecutionData> OnAttackHitboxFinished;

        [SerializeField] protected bool _active = true;
        [SerializeField] protected AttackHitbox _attackHitbox;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected CharacterAnimationController _characterAnimationController;
        [SerializeField] protected ComboAttackSetSO _comboAttackSet;
        [SerializeField] protected List<ComboAttackData> _list_ComboAttackData = new List<ComboAttackData>();
        [SerializeField] protected bool _triggerIdle = true;
        [SerializeField] protected string _backToIdleTriggerParameter = "Idle";
        [Header("Gizmos")]
        [SerializeField] protected bool _drawGizmos = true;
        [SerializeField] protected int _debugAttackIndex;
        [SerializeField] protected Color _debugHitboxColor = Color.yellow;


        protected int _currentComboIndex;
        protected int _attackExecutionId;
        protected bool _isAttacking;
        protected bool _isComboWindowOpen;
        protected bool _hasQueuedNextAttack;

        public bool Active => _active;
        public bool IsAttacking => _isAttacking;
        public bool IsComboWindowOpen => _isComboWindowOpen;
        public bool HasQueuedNextAttack => _hasQueuedNextAttack;
        public IReadOnlyList<ComboAttackData> List_ComboAttackData => _list_ComboAttackData;

        protected virtual void Awake()
        {
            GetReferences();
            ApplyInitialComboAttackSet();
        }


        public virtual void Enable() => _active = true;

        public virtual void Disable() => _active = false;

        public virtual void SetComboAttackSet(ComboAttackSetSO comboAttackSet)
        {
            _comboAttackSet = comboAttackSet;

            if (_isAttacking)
                CancelAttack(false);

            _list_ComboAttackData.Clear();

            if (_comboAttackSet == null)
                return;

            IReadOnlyList<ComboAttackData> list_ComboAttackData = _comboAttackSet.List_ComboAttackData;

            for (int i = 0; i < list_ComboAttackData.Count; i++)
            {
                if (list_ComboAttackData[i] != null)
                    _list_ComboAttackData.Add(list_ComboAttackData[i]);
            }
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

        public virtual void CancelAttack(bool playIdle)
        {
            bool wasAttacking = _isAttacking;

            _isAttacking = false;
            _isComboWindowOpen = false;
            _hasQueuedNextAttack = false;
            _currentComboIndex = 0;

            if (playIdle)
                PlayIdle();

            if (wasAttacking)
                OnAttackCanceled?.Invoke();
        }

        public virtual void AnimationEvent_ExecuteHitbox()
        {
            if (!_active) return;
            if (!CanUseCurrentAttack()) return;

            ComboAttackData attackData = _list_ComboAttackData[_currentComboIndex];
            AttackExecutionData executionData = new AttackExecutionData(++_attackExecutionId, _currentComboIndex, attackData, gameObject);

            OnAttackHitboxExecuted?.Invoke(executionData);
            _attackHitbox.ExecuteHitbox(attackData, executionData);
            OnAttackHitboxFinished?.Invoke(executionData);
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

        protected virtual void ApplyInitialComboAttackSet()
        {
            if (_comboAttackSet == null)
                return;

            SetComboAttackSet(_comboAttackSet);
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

        protected virtual void OnDrawGizmos()
        {
            if (!_drawGizmos)
                return;

            if (_attackHitbox == null)
                TryGetComponent(out _attackHitbox);

            IReadOnlyList<ComboAttackData> list_ComboAttackData = GetGizmoComboAttackData();

            if (_attackHitbox == null || list_ComboAttackData == null || list_ComboAttackData.Count == 0)
                return;

            int attackIndex = GetDebugAttackIndex(list_ComboAttackData.Count);
            ComboAttackData attackData = list_ComboAttackData[attackIndex];

            if (attackData == null)
                return;

            Gizmos.color = _debugHitboxColor;
            Gizmos.matrix = Matrix4x4.TRS(_attackHitbox.GetHitboxCenter(attackData), _attackHitbox.GetHitboxRotation(), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, attackData.HitboxHalfExtents * 2f);
            Gizmos.matrix = Matrix4x4.identity;
        }

        private IReadOnlyList<ComboAttackData> GetGizmoComboAttackData()
        {
            if (_list_ComboAttackData != null && _list_ComboAttackData.Count > 0)
                return _list_ComboAttackData;

            return _comboAttackSet != null ? _comboAttackSet.List_ComboAttackData : null;
        }

        private int GetDebugAttackIndex(int attackCount)
        {
            if (_isAttacking && _currentComboIndex >= 0 && _currentComboIndex < attackCount)
                return _currentComboIndex;

            return Mathf.Clamp(_debugAttackIndex, 0, attackCount - 1);
        }
    }
}
