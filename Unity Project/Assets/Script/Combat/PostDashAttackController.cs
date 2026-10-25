using Scripts.DashHandler;
using Scripts.Direction;
using Scripts.Weapon;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Combat
{
    [DisallowMultipleComponent]
    public class PostDashAttackController : ComboControllerBase
    {
        #region Fields

        [Header("Input")]
        [SerializeField] private InputActionReference _leftAttackInput;
        [SerializeField] private InputActionReference _rightAttackInput;
        [Header("Post-Dash")]
        [SerializeField] private DashBase _dash;
        [SerializeField] private MN_PlayerWeapon _weaponManager;
        [SerializeField] private DirectionHandler _directionHandler;
        [SerializeField] private GameObject _hittableReference;
        [SerializeField] private bool _maintainComboIndexOnDash = true;

        private IHittable _hittable;
        private ComboAttackData _postDashAttack;
        private bool _hasQueuedDashAttack;
        private bool _isPostDashAttack;
        private int _retainedComboIndex;
        private float _retainedComboTime;
        private DirectionSide _attackDirection;

        public bool HasQueuedAttack => _hasQueuedDashAttack;
        public bool IsPostDashAttack => _isPostDashAttack;
        public override float ComboRetentionRemaining =>
            _isPostDashAttack ? _retainedComboTime : base.ComboRetentionRemaining;
        protected override ComboAttackData CurrentAttack =>
            _isPostDashAttack ? _postDashAttack : base.CurrentAttack;

        #endregion

        #region Lifecycle

        protected override void GetReferences()
        {
            base.GetReferences();
            if (_dash == null) TryGetComponent(out _dash);
            if (_directionHandler == null) TryGetComponent(out _directionHandler);
            if (_weaponManager == null) _weaponManager = FindFirstObjectByType<MN_PlayerWeapon>();
            if (_hittableReference == null) _hittableReference = gameObject;
            _hittable = _hittableReference.GetComponent<IHittable>();
        }

        private void OnEnable()
        {
            if (_leftAttackInput != null && _leftAttackInput.action != null)
            {
                _leftAttackInput.action.performed += LeftAttack_Performed;
                _leftAttackInput.action.Enable();
            }
            if (_rightAttackInput != null && _rightAttackInput.action != null)
            {
                _rightAttackInput.action.performed += RightAttack_Performed;
                _rightAttackInput.action.Enable();
            }
            if (_dash != null)
            {
                _dash.OnDashStarted += Dash_OnStarted;
                _dash.OnDashFinished += Dash_OnFinished;
            }
            if (_weaponManager != null)
            {
                _weaponManager.OnWeaponLoadoutEquipped += ApplyWeaponLoadout;
                ApplyWeaponLoadout(_weaponManager.CurrentLoadout);
            }
            if (_hittable != null) _hittable.OnHitted += Hittable_OnHitted;
        }

        protected override void OnDisable()
        {
            if (_leftAttackInput != null && _leftAttackInput.action != null)
                _leftAttackInput.action.performed -= LeftAttack_Performed;
            if (_rightAttackInput != null && _rightAttackInput.action != null)
                _rightAttackInput.action.performed -= RightAttack_Performed;
            if (_dash != null)
            {
                _dash.OnDashStarted -= Dash_OnStarted;
                _dash.OnDashFinished -= Dash_OnFinished;
            }
            if (_weaponManager != null) _weaponManager.OnWeaponLoadoutEquipped -= ApplyWeaponLoadout;
            if (_hittable != null) _hittable.OnHitted -= Hittable_OnHitted;
            base.OnDisable();
        }

        protected override void LateUpdate()
        {
            // All OnDashFinished subscribers have released their locks before starting the attack.
            if (_hasQueuedDashAttack && _dash != null && !_dash.IsDashing)
                BeginPostDashAttack();
            base.LateUpdate();
        }

        #endregion

        #region Weapon

        private void ApplyWeaponLoadout(WeaponLoadoutSO loadout)
        {
            CancelAttack(playIdle: false);
            _postDashAttack = loadout != null && loadout.HasPostDashAttack ? loadout.PostDashAttack : null;
            // The normal combo keeps its own list. The special only overrides CurrentAttack.
            if (loadout != null) SetComboAttackSet(loadout.ComboAttackSet);
        }

        #endregion

        #region Input And Dash

        private void LeftAttack_Performed(InputAction.CallbackContext context) => RequestAttack(DirectionSide.LEFT);
        private void RightAttack_Performed(InputAction.CallbackContext context) => RequestAttack(DirectionSide.RIGHT);

        public override void RequestAttack() =>
            RequestAttack(_directionHandler != null ? _directionHandler.CurrentDirection : DirectionSide.RIGHT);

        public void RequestAttack(DirectionSide direction)
        {
            if (Time.timeScale <= 0f || !Active || !isActiveAndEnabled || IsAttackInputBlocked ||
                (_hittable != null && !_hittable.IsAlive())) return;

            if (_dash != null && (_dash.IsDashing || _hasQueuedDashAttack))
            {
                _hasQueuedDashAttack = _postDashAttack != null;
                if (_hasQueuedDashAttack) _attackDirection = direction;
                return;
            }

            if (_isPostDashAttack || (IsAttacking && !IsComboWindowOpen) || _list_ComboAttackData.Count == 0) return;

            _attackDirection = direction;
            base.RequestAttack();
        }

        private void Dash_OnStarted(float directionDot)
        {
            CancelAttack(playIdle: false, maintainComboIndex: _maintainComboIndexOnDash);
        }

        private void Dash_OnFinished(float directionDot)
        {
            if (_dash.LastDashCanceled) _hasQueuedDashAttack = false;
        }

        private void BeginPostDashAttack()
        {
            _hasQueuedDashAttack = false;
            if (_dash.LastDashCanceled || !Active || !isActiveAndEnabled ||
                IsAttackInputBlocked || _postDashAttack == null ||
                (_hittable != null && !_hittable.IsAlive())) return;

            _retainedComboIndex = _currentComboIndex;
            _retainedComboTime = base.ComboRetentionRemaining;
            _isPostDashAttack = true;
            PlayCurrentAttack();
            if (IsAttacking) AnimationEvent_CloseComboWindow();
        }

        private void Hittable_OnHitted(int damage) => CancelAttack(playIdle: false);

        #endregion

        #region Finish And Cancellation

        protected override bool PrepareAttack()
        {
            if (_directionHandler == null) return false;
            _directionHandler.UnblockDirection(this);
            if (!_directionHandler.Active) return false;
            _directionHandler.SetDirection(_attackDirection);
            _directionHandler.BlockDirection(this);
            return true;
        }

        protected override void FinishCombo()
        {
            bool wasPostDashAttack = _isPostDashAttack;
            if (_directionHandler != null) _directionHandler.UnblockDirection(this);
            base.FinishCombo();
            if (wasPostDashAttack) EndPostDashAttack(restoreCombo: true);
        }

        public override void CancelAttack(bool playIdle = true, bool maintainComboIndex = false)
        {
            _hasQueuedDashAttack = false;
            bool wasPostDashAttack = _isPostDashAttack;
            if (_directionHandler != null) _directionHandler.UnblockDirection(this);
            base.CancelAttack(playIdle, !wasPostDashAttack && maintainComboIndex);
            if (wasPostDashAttack) EndPostDashAttack(maintainComboIndex);
        }

        private void EndPostDashAttack(bool restoreCombo)
        {
            _isPostDashAttack = false;
            if (restoreCombo && _retainedComboTime > 0f)
            {
                _currentComboIndex = _retainedComboIndex;
                _phase = AttackPhase.HOLDING_ATTACK_INDEX;
                _comboRetentionExpiresAt = Time.time + _retainedComboTime;
            }
            _retainedComboTime = 0f;
        }

        #endregion
    }
}
