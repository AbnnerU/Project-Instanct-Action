using System;
using System.Collections;
using Scripts.Combat;
using Scripts.ObjectPooling;
using Scripts.Player;
using Scripts.Status;
using UnityEngine;
using Scripts.Projectile;

namespace Scripts.Abilities
{
    public class LauchProjectileAbility : AbilityBase, IHasCastTime
    {
        public event Action<float> OnCastTimeStarted;
        public event Action<float> OnCastTimeFinished;

        #region Fields

        [Header("References")]
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private GameObject _attackerObject;
        [SerializeField] private Animator _animator;
        [Header("Damage")]
        [SerializeField] private DamageData _damageData = new DamageData();
        [Header("Animation")]
        [SerializeField] private string _animationStateName;
        [SerializeField] private float _animationTransitionDuration = 0.05f;
        [SerializeField] private int _animationLayerIndex;
        [Header("Projectile")]
        [SerializeField] private float _castTime;
        [SerializeField] private float _projectileSpeed = 8f;
        [SerializeField] private Vector3 _gravityValue = Vector3.zero;
        [SerializeField] private float _projectileRadius = 0.25f;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private bool _useGroundLayer;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private string _targetTag;
        [Header("Container")]
        [SerializeField] private Transform _projectileContainer;
        [SerializeField] private string _containerTag;
        [Header("Combat Lock")]
        [SerializeField] private ComboControllerBase _comboController;
        [SerializeField] private bool _cancelAttackOnExecute = true;
        [SerializeField] private bool _disableComboOnExecute = true;
        [SerializeField] private float _comboDisableDuration = 0.1f;
        [Header("Dash Lock")]
        [SerializeField] private PlayerDash _dashComponent;
        [SerializeField] private bool _disableDashOnExecute = true;
        [SerializeField] private float _disableDashDuration = 0.1f;

        private Coroutine _dashDisableRoutine;
        private Coroutine _comboDisableRoutine;
        private Coroutine _castRoutine;
        public float CastTime { get => _castTime; set => _castTime = value; }

        #endregion

        #region Public

        public void Setup(StatusInfoSourceBase statusSource, ComboControllerBase comboController, GameObject attackerObject, PlayerDash dash)
        {
            _statusSource = statusSource;
            _comboController = comboController;
            _attackerObject = attackerObject;
            _dashComponent = dash;

            if (_animator == null && _attackerObject != null)
                _animator = _attackerObject.GetComponentInChildren<Animator>();

            if (_containerTag != string.Empty)
                _projectileContainer = GameObject.FindGameObjectWithTag(_containerTag).transform;
        }

        public override void Execute()
        {
            if (!CanExecute())
                return;

            if (_projectilePrefab == null)
            {
                Debug.LogWarning("Projectile prefab is missing.", this);
                return;
            }

            HandleComboOnExecute();
            HandleDashOnExecute();
            PlayAnimation();
            StartCastRoutine();
        }

        public override void Cancel()
        {
            bool wasExecuting = _castRoutine != null;

            StopCastRoutine();
            StopComboDisableRoutine(true);
            StopDashDisableRoutine(true);

            if (wasExecuting)
                OnAbilityFinished?.Invoke(this);
        }

        public override void Deactivate()
        {
            Cancel();
            base.Deactivate();
        }

        #endregion

        #region Execute
        protected override bool CanExecute()
        {
            return _castRoutine == null && _active;
        }

        private void PlayAnimation()
        {
            if (_animator == null || string.IsNullOrEmpty(_animationStateName))
                return;

            _animator.CrossFade(_animationStateName, _animationTransitionDuration, _animationLayerIndex, 0f);
        }

        private void StartCastRoutine()
        {
            StopCastRoutine();
            _castRoutine = StartCoroutine(CastRoutine());
        }

        private IEnumerator CastRoutine()
        {
            OnAbilityStarted?.Invoke(this);

            if (_castTime > 0f)
            {
                OnCastTimeStarted?.Invoke(_castTime);
                yield return new WaitForSeconds(_castTime);
                OnCastTimeFinished?.Invoke(_castTime);
            }

            SpawnProjectile();
            _castRoutine = null;

            OnAbilityFinished?.Invoke(this);
        }

        private void HandleDashOnExecute()
        {
            if(_dashComponent == null) return;

            if (!_disableDashOnExecute) return;

            StopDashDisableRoutine(false);

            bool wasDashActiveBeforeDisable = _dashComponent.Active;
            _dashComponent.Disable();

            if(_disableDashDuration <= 0)
            {
                if (wasDashActiveBeforeDisable)
                    _dashComponent.Enable();

                return;
            }

            _dashDisableRoutine = StartCoroutine(DashDisableRoutine());
        }

        private IEnumerator DashDisableRoutine()
        {
            yield return new WaitForSeconds(_disableDashDuration);

            if (_dashComponent != null)
                _dashComponent.Enable();

            _dashDisableRoutine = null;
        }

        private void HandleComboOnExecute()
        {
            if (_comboController == null)
                return;

            if (_cancelAttackOnExecute)
                _comboController.CancelAttack(false);

            if (!_disableComboOnExecute)
                return;

            StopComboDisableRoutine(false);

            bool wasComboActiveBeforeDisable = _comboController.Active;
            _comboController.Disable();

            if (_comboDisableDuration <= 0f)
            {
                if (wasComboActiveBeforeDisable)
                    _comboController.Enable();

                return;
            }

            _comboDisableRoutine = StartCoroutine(ComboDisableRoutine());
        }

        private IEnumerator ComboDisableRoutine()
        {
            yield return new WaitForSeconds(_comboDisableDuration);

            if (_comboController != null)
                _comboController.Enable();

            _comboDisableRoutine = null;
        }

        private void SpawnProjectile()
        {
            Vector3 direction = _spawnPoint.forward;
            Vector3 spawnPosition = _spawnPoint != null ? _spawnPoint.position : transform.position;
            Quaternion rotation = _spawnPoint != null ? _spawnPoint.rotation : Quaternion.LookRotation(direction, Vector3.up);

            GameObject projectileObject = PoolManager.SpawnObject(_projectilePrefab, spawnPosition, rotation);

            projectileObject.transform.SetParent(_projectileContainer);

            if (!projectileObject.TryGetComponent(out DamageRequestProjectile abilityProjectile))
            {
                Debug.LogWarning("Projectile prefab needs an AbilityProjectile component.", projectileObject);
                return;
            }

            DamageRequest damageRequest = new DamageRequest(
                _damageData,
                _statusSource != null ? _statusSource.GetStatusInfo() : null,
                _attackerObject != null ? _attackerObject : gameObject,
                null,
                null,
                spawnPosition);

            abilityProjectile.SetDamageRequest(damageRequest);
            abilityProjectile.EnableProjectile(
                spawnPosition,
                direction * _projectileSpeed,
                _gravityValue,
                _layerMask,
                _useGroundLayer,
                _groundLayer,
                _projectileRadius,
                _targetTag);
        }

        private void StopCastRoutine()
        {
            if (_castRoutine == null)
                return;

            StopCoroutine(_castRoutine);
            _castRoutine = null;
        }

        private void StopComboDisableRoutine(bool activeState)
        {
            if (_comboDisableRoutine == null)
                return;

            StopCoroutine(_comboDisableRoutine);
            _comboDisableRoutine = null;

            if (activeState && _comboController != null)
                _comboController.Enable();
        }

        private void StopDashDisableRoutine(bool activeState)
        {
            if (_dashDisableRoutine == null)
                return;

            StopCoroutine(_dashDisableRoutine);
            _dashDisableRoutine = null;

            if (activeState && _dashComponent != null)
                _dashComponent.Enable();
        }

        #endregion
    }
}
