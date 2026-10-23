using System;
using Scripts.DashHandler;
using Scripts.Direction;
using Scripts.Weapon;
using UnityEngine;

namespace Scripts.Combat
{
    [DisallowMultipleComponent]
    public class PostDashAttackController : MonoBehaviour
    {
        #region Fields

        public event Action OnAttackStarted;
        public event Action OnAttackFinished;
        public event Action OnAttackCanceled;

        [SerializeField] private DashBase _dash;
        [SerializeField] private ComboControllerBase _comboController;
        [SerializeField] private MN_PlayerWeapon _weaponManager;
        [SerializeField] private AttackHitbox _attackHitbox;
        [SerializeField] private Animator _animator;
        [SerializeField] private DirectionHandler _directionHandler;
        [SerializeField] private GameObject _hittableReference;
        [SerializeField] private string _idleStateName = "Base Layer.Idle";

        private IHittable _hittable;
        private ComboAttackData _attackData;
        private bool _hasQueuedAttack;
        private int _startedFrame;
        private Guid _executionId;

        public bool IsExecuting { get; private set; }
        public bool HasQueuedAttack => _hasQueuedAttack;

        #endregion

        #region Lifecycle

        private void Awake()
        {
            if (_dash == null) TryGetComponent(out _dash);
            if (_comboController == null) TryGetComponent(out _comboController);
            if (_attackHitbox == null) TryGetComponent(out _attackHitbox);
            if (_animator == null) _animator = GetComponentInChildren<Animator>();
            if (_directionHandler == null) TryGetComponent(out _directionHandler);
            if (_weaponManager == null) _weaponManager = FindFirstObjectByType<MN_PlayerWeapon>();
            if (_hittableReference == null) _hittableReference = gameObject;
            _hittable = _hittableReference.GetComponent<IHittable>();
        }

        private void OnEnable()
        {
            if (_dash != null)
            {
                _dash.OnDashStarted += Dash_OnStarted;
                _dash.OnDashFinished += Dash_OnFinished;
            }
            if (_weaponManager != null) _weaponManager.OnWeaponLoadoutEquipped += Weapon_OnEquipped;
            if (_hittable != null) _hittable.OnHitted += Hittable_OnHitted;
        }

        private void OnDisable()
        {
            if (_dash != null)
            {
                _dash.OnDashStarted -= Dash_OnStarted;
                _dash.OnDashFinished -= Dash_OnFinished;
            }
            if (_weaponManager != null) _weaponManager.OnWeaponLoadoutEquipped -= Weapon_OnEquipped;
            if (_hittable != null) _hittable.OnHitted -= Hittable_OnHitted;
            CancelAttack();
        }

        private void LateUpdate()
        {
            if (!IsExecuting || Time.timeScale <= 0f || Time.frameCount == _startedFrame) return;
            if (_comboController == null || !_comboController.Active || !_comboController.isActiveAndEnabled ||
                !TryGetAttackState(out AnimatorStateInfo state))
            {
                CancelAttack();
                return;
            }
            // Release the locks even if a clip is missing its Finish event.
            if (!_animator.IsInTransition(0) && state.normalizedTime >= 1f) FinishAttack();
        }

        #endregion

        #region Dash And Input

        public bool TryQueueAttack()
        {
            if (!isActiveAndEnabled || _dash == null || !_dash.IsDashing ||
                _comboController == null || !_comboController.Active) return false;
            _hasQueuedAttack = true;
            return true;
        }

        private void Dash_OnStarted(float directionDot) => CancelAttack();

        private void Dash_OnFinished(float directionDot)
        {
            bool requested = _hasQueuedAttack;
            _hasQueuedAttack = false;
            if (!requested || _dash.LastDashCanceled || (_hittable != null && !_hittable.IsAlive())) return;
            BeginAttack();
        }

        private void Weapon_OnEquipped(WeaponLoadoutSO loadout) => CancelAttack();

        private void Hittable_OnHitted(int damage)
        {
            if ((IsExecuting || _hasQueuedAttack) && _comboController != null)
                _comboController.CancelAttack(playIdle: false);
            CancelAttack();
        }

        #endregion

        #region Execution

        private void BeginAttack()
        {
            WeaponLoadoutSO loadout = _weaponManager != null ? _weaponManager.CurrentLoadout : null;
            if (loadout == null || !loadout.HasPostDashAttack || loadout.PostDashAttack == null) return;
            if (_comboController == null || !_comboController.Active || !_comboController.isActiveAndEnabled ||
                _comboController.IsAttacking || _attackHitbox == null || _animator == null ||
                !_animator.isActiveAndEnabled || _animator.runtimeAnimatorController == null || _animator.layerCount == 0)
                return;

            _attackData = loadout.PostDashAttack;
            if (string.IsNullOrWhiteSpace(_attackData.AnimationStateName) ||
                !_animator.HasState(0, Animator.StringToHash(_attackData.AnimationStateName)))
            {
                Debug.LogWarning("[PostDashAttackController] The equipped weapon has no valid post-dash Animator state.", this);
                _attackData = null;
                return;
            }

            IsExecuting = true;
            _startedFrame = Time.frameCount;
            _executionId = Guid.NewGuid();
            _comboController.BlockAttackInput(this);
            _comboController.PauseComboRetention();
            if (_directionHandler != null) _directionHandler.BlockDirection(this);
            _animator.CrossFade(_attackData.AnimationStateName, _attackData.TransitionDuration, 0, 0f);
            OnAttackStarted?.Invoke();
        }

        public void ExecuteHitbox()
        {
            if (!IsExecuting || _attackData == null || _attackHitbox == null) return;
            // Index -1 identifies an attack outside the normal combo sequence.
            var execution = new AttackExecutionData(_executionId, -1, _attackData, gameObject);
            _attackHitbox.ExecuteHitbox(_attackData, execution);
        }

        public void FinishAttack()
        {
            if (!IsExecuting) return;
            ReleaseAttack();
            if (_animator != null && _animator.isActiveAndEnabled && !string.IsNullOrEmpty(_idleStateName) &&
                _animator.HasState(0, Animator.StringToHash(_idleStateName)))
                _animator.CrossFadeInFixedTime(_idleStateName, 0.05f, 0, 0f);
            OnAttackFinished?.Invoke();
        }

        public void CancelAttack()
        {
            _hasQueuedAttack = false;
            if (!IsExecuting) return;
            ReleaseAttack();
            OnAttackCanceled?.Invoke();
        }

        private void ReleaseAttack()
        {
            IsExecuting = false;
            _attackData = null;
            if (_comboController != null)
            {
                _comboController.ResumeComboRetention();
                _comboController.UnblockAttackInput(this);
            }
            if (_directionHandler != null) _directionHandler.UnblockDirection(this);
        }

        #endregion

        #region Animation Events

        public bool CanAcceptAnimationEvent(Animator source, AnimationEvent animationEvent)
        {
            if (!isActiveAndEnabled || !IsExecuting || source != _animator || animationEvent == null ||
                !animationEvent.isFiredByAnimator || !TryGetAttackState(out AnimatorStateInfo state)) return false;
            AnimatorStateInfo eventState = animationEvent.animatorStateInfo;
            return eventState.fullPathHash == state.fullPathHash && eventState.normalizedTime <= state.normalizedTime + 0.01f;
        }

        private bool TryGetAttackState(out AnimatorStateInfo state)
        {
            state = default;
            if (_attackData == null || _animator == null || !_animator.isActiveAndEnabled ||
                _animator.runtimeAnimatorController == null || _animator.layerCount == 0) return false;
            state = _animator.IsInTransition(0) ? _animator.GetNextAnimatorStateInfo(0) : _animator.GetCurrentAnimatorStateInfo(0);
            return state.IsName(_attackData.AnimationStateName);
        }

        #endregion
    }
}
