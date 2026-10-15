using System;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Animation;

namespace Scripts.Combat
{
    public abstract class ComboControllerBase : MonoBehaviour, IHasActiveState
    {
        #region Fields
        public event Action OnComboStarted;
        public event Action OnComboStopped;
        public event Action OnEntireComboSequenceExecuted;
        public event Action OnAttackStarted;
        public event Action<ComboAttackData> OnBeforeAttack;
        public event Action OnAttackFinished;
        public event Action OnAttackCanceled;
        public event Action<float> OnComboHoldingStarted;
        public event Action<AttackExecutionData> OnAttackHitboxExecutionStarted;
        public event Action<AttackExecutionData> OnAttackHitboxExecutionFinished;

        [SerializeField] protected bool _active = true;
        [SerializeField] protected AttackHitbox _attackHitbox;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected CharacterAnimationController _characterAnimationController;
        [SerializeField] protected ComboAttackSetSO _comboAttackSet;
        [SerializeField] protected List<ComboAttackData> _list_ComboAttackData = new List<ComboAttackData>();
        [SerializeField] protected bool _triggerIdle = true;
        [SerializeField] protected string _backToIdleTriggerParameter = "Idle";
        [Header("Maintain Combo Index")]
        [SerializeField] protected float _comboRetentionDuration = 0.87f;
        [Header("Gizmos")]
        [SerializeField] protected bool _drawGizmos = true;
        [SerializeField] protected int _debugAttackIndex;
        [SerializeField] protected Color _debugHitboxColor = Color.yellow;

        protected float _comboRetentionExpiresAt;

        protected enum AttackStatus { IDLE, ACCEPTING_INPUT, RECOVERY, FINISHING }
        protected enum AttackPhase { NONE, STARTED, HITBOX_EXECUTED, HOLDING_ATTACK_INDEX, FINISHING }

        protected AttackStatus _status;
        protected AttackPhase _phase;

        protected int _currentComboIndex;
        protected bool _hasQueuedNextAttack;

        protected int _attackStartedFrame;

        protected int _executionVersion;

        public bool Active => _active;
        public bool IsAttacking => _status != AttackStatus.IDLE;
        public bool IsComboWindowOpen => _status == AttackStatus.ACCEPTING_INPUT;
        public bool HasQueuedNextAttack => _hasQueuedNextAttack;
        public int CurrentComboIndex => _currentComboIndex;
        public IReadOnlyList<ComboAttackData> List_ComboAttackData => _list_ComboAttackData;
        protected ComboAttackData CurrentAttack => _currentComboIndex >= 0 && _currentComboIndex < _list_ComboAttackData.Count
            ? _list_ComboAttackData[_currentComboIndex] : null;
        private bool CanProcessAttackEvents => _active && isActiveAndEnabled &&
            (_status == AttackStatus.ACCEPTING_INPUT || _status == AttackStatus.RECOVERY);

        #endregion

        #region Lifecycle

        protected virtual void Awake()
        {
            GetReferences();
            ApplyInitialComboAttackSet();
        }

        protected virtual void OnDisable() => CancelAttack(playIdle: false);

        protected virtual void LateUpdate()
        {
            if (!IsAttacking || Time.timeScale <= 0f || Time.frameCount == _attackStartedFrame)
                return;

            if (!_active || _animator == null || !_animator.isActiveAndEnabled ||
                _animator.runtimeAnimatorController == null)
            {
                CancelAttack(playIdle: false);
                return;
            }

            if (!TryGetAttackState(out AnimatorStateInfo state))
            {
                CancelAttack(playIdle: false);
                return;
            }

            if (!_animator.IsInTransition(0) && state.normalizedTime >= 1f)
                CancelAttack();
        }

        #endregion

        #region Setup

        public virtual void Enable() => _active = true;

        public virtual void Disable() => _active = false;

        public virtual void SetComboAttackSet(ComboAttackSetSO comboAttackSet)
        {
            _comboAttackSet = comboAttackSet;

            CancelAttack(playIdle: false);

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

        #endregion

        #region Attack And Cancellation

        public virtual void RequestAttack()
        {
            if (!_active || !isActiveAndEnabled || _status == AttackStatus.FINISHING) return;
            if (_list_ComboAttackData.Count == 0) return;

            if (!IsAttacking)
            {
                if(_phase == AttackPhase.HOLDING_ATTACK_INDEX)
                {
                    if (Time.time >= _comboRetentionExpiresAt) 
                        _currentComboIndex = 0;                    
                }

                PlayCurrentAttack();

                if (IsAttacking)
                    OnComboStarted?.Invoke();

                return;
            }

            if (IsComboWindowOpen && !_hasQueuedNextAttack)
                _hasQueuedNextAttack = true;
        }

        public virtual void CancelAttack(bool playIdle = true, bool maintainComboIndex = false)
        {
            bool wasAttacking = IsAttacking;

            ResetExecution();

            if (maintainComboIndex)
                MaintainComboHandler();
            else
                _currentComboIndex = 0;

            if (playIdle) PlayIdle();
            if (wasAttacking)
                OnAttackCanceled?.Invoke();
        }

        private void ResetExecution()
        {
            _executionVersion++;
            _status = AttackStatus.IDLE;
            _hasQueuedNextAttack = false;
        }

        private bool IsCurrentExecution(int version) =>
            _active && isActiveAndEnabled && IsAttacking && version == _executionVersion;

       
        protected virtual void MaintainComboHandler()
        {
            _comboRetentionExpiresAt = Time.time + _comboRetentionDuration;

            if (_phase == AttackPhase.HITBOX_EXECUTED)
            {
                _currentComboIndex++;

                if (_currentComboIndex > _list_ComboAttackData.Count - 1)
                    OnEntireComboSequenceExecuted?.Invoke();
            }

            _phase = AttackPhase.HOLDING_ATTACK_INDEX;

            OnComboHoldingStarted?.Invoke(_comboRetentionDuration);
        }

        #endregion

        #region Animation Events

        public bool CanAcceptAnimationEvent(Animator source, AnimationEvent animationEvent)
        {
            if (!CanProcessAttackEvents ||
                source == null || source != _animator || animationEvent == null ||
                !animationEvent.isFiredByAnimator)
                return false;

            if (!TryGetAttackState(out AnimatorStateInfo state)) return false;
            AnimatorStateInfo eventState = animationEvent.animatorStateInfo;
            return eventState.fullPathHash == state.fullPathHash &&
                eventState.normalizedTime <= state.normalizedTime + 0.01f;
        }

        private bool TryGetAttackState(out AnimatorStateInfo state)
        {
            state = default;
            ComboAttackData attack = CurrentAttack;
            if (_animator == null || _animator.runtimeAnimatorController == null ||
                _animator.layerCount == 0 || attack == null || string.IsNullOrEmpty(attack.AnimationStateName))
                return false;

            // During a blend only the destination may control the new attack.
            state = _animator.IsInTransition(0)
                ? _animator.GetNextAnimatorStateInfo(0)
                : _animator.GetCurrentAnimatorStateInfo(0);
            return state.IsName(attack.AnimationStateName);
        }

        public virtual void AnimationEvent_ExecuteHitbox()
        {
            if (!CanProcessAttackEvents) return;
            if (!CanUseCurrentAttack()) return;

            ComboAttackData attackData = CurrentAttack;
            AttackExecutionData executionData = new AttackExecutionData(Guid.NewGuid(), _currentComboIndex, attackData, gameObject);

            int version = _executionVersion;
            OnAttackHitboxExecutionStarted?.Invoke(executionData);

            if (!IsCurrentExecution(version)) return;

            _phase = AttackPhase.HITBOX_EXECUTED;

            _attackHitbox.ExecuteHitbox(attackData, executionData);
            OnAttackHitboxExecutionFinished?.Invoke(executionData);
        }

        public virtual void AnimationEvent_OpenComboWindow()
        {
            // Legacy clip compatibility: the window now opens when PlayCurrentAttack starts.
        }

        public virtual void AnimationEvent_CloseComboWindow()
        {
            if (!CanProcessAttackEvents) return;
            _status = AttackStatus.RECOVERY;
        }

        public virtual void AnimationEvent_FinishAttack()
        {
            if (!CanProcessAttackEvents) return;

            int version = _executionVersion;
            _status = AttackStatus.FINISHING;
            OnAttackFinished?.Invoke();
            if (!IsCurrentExecution(version)) return;

            _phase = AttackPhase.FINISHING;

            if (_hasQueuedNextAttack)
            { 
                _currentComboIndex++;

                if (_currentComboIndex > _list_ComboAttackData.Count - 1)
                    OnEntireComboSequenceExecuted?.Invoke();

                PlayCurrentAttack();
                return;
            }
            else
                FinishCombo();
        }

        #endregion

        #region Execution

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

        protected virtual void PlayCurrentAttack()
        {
            if (!_active || !CanUseCurrentAttack())
            {
                CancelAttack(playIdle: false);
                return;
            }

            if (_currentComboIndex > _list_ComboAttackData.Count - 1)        
                _currentComboIndex = 0;
            
            ComboAttackData attackData = CurrentAttack;

            if (_animator == null || !_animator.isActiveAndEnabled ||
                _animator.runtimeAnimatorController == null || _animator.layerCount == 0 ||
                string.IsNullOrEmpty(attackData.AnimationStateName) ||
                !_animator.HasState(0, Animator.StringToHash(attackData.AnimationStateName)))
            {
                Debug.LogWarning("[ComboControllerBase] Cannot play attack: Animator or state is unavailable.", this);
                CancelAttack(playIdle: false);
                return;
            }

            int version = ++_executionVersion;
            _attackStartedFrame = Time.frameCount;
            _status = AttackStatus.ACCEPTING_INPUT;
            
            _hasQueuedNextAttack = false;

            if (!string.IsNullOrEmpty(_backToIdleTriggerParameter))
                _animator.ResetTrigger(_backToIdleTriggerParameter);

            OnBeforeAttack?.Invoke(attackData);

            if (!IsCurrentExecution(version)) return;

            _phase = AttackPhase.STARTED;
            _animator.CrossFade(attackData.AnimationStateName, attackData.TransitionDuration, 0, 0f);
            OnAttackStarted?.Invoke();
        }

        protected virtual void FinishCombo()
        {
            ResetExecution();
            _currentComboIndex = 0;
            PlayIdle();

            _phase = AttackPhase.NONE;

            OnComboStopped?.Invoke();
        }

        protected virtual void PlayIdle()
        {
            if (!_triggerIdle || _animator == null || string.IsNullOrEmpty(_backToIdleTriggerParameter)) return;

            if (_characterAnimationController != null)
                _characterAnimationController.SetTrigger(_backToIdleTriggerParameter);
            else
                _animator.SetTrigger(_backToIdleTriggerParameter);
        }

        protected virtual bool CanUseCurrentAttack()
        {
            return _attackHitbox != null && CurrentAttack != null;
        }

        #endregion

        #region Gizmos
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
            if (IsAttacking && _currentComboIndex >= 0 && _currentComboIndex < attackCount)
                return _currentComboIndex;

            return Mathf.Clamp(_debugAttackIndex, 0, attackCount - 1);
        }
        #endregion
    }
}
