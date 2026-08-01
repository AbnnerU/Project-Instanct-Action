using System.Collections;
using Scripts.Combat;
using Scripts.ObjectPooling;
using Scripts.Player;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Abilities
{
    public class FireBallAbility : AbilityBase
    {
        #region Fields

        [Header("References")]
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private PlayerDirection _playerDirection;
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private ComboControllerBase _comboController;
        [SerializeField] private GameObject _attackerObject;
        [SerializeField] private Animator _animator;

        [Header("Damage")]
        [SerializeField] private DamageData _damageData = new DamageData();

        [Header("Animation")]
        [SerializeField] private string _animationStateName;
        [SerializeField] private float _animationTransitionDuration = 0.05f;
        [SerializeField] private int _animationLayerIndex;

        [Header("Cooldown")]
        [SerializeField] private float _cooldownDuration = 1f;

        [Header("Projectile")]
        [SerializeField] private float _projectileSpeed = 8f;
        [SerializeField] private float _spawnDelay;
        [SerializeField] private Vector3 _gravityValue = Vector3.zero;
        [SerializeField] private float _projectileRadius = 0.25f;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private bool _useGroundLayer;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private string _targetTag;

        [Header("Combat Lock")]
        [SerializeField] private bool _cancelAttackOnExecute = true;
        [SerializeField] private bool _disableComboOnExecute = true;
        [SerializeField] private float _comboDisableDuration = 0.1f;

        private Coroutine _comboDisableRoutine;
        private Coroutine _castRoutine;
        private bool _wasComboActiveBeforeDisable;
        private float _cooldownEndTime;

        #endregion

        #region Public

        public void Setup(
            PlayerDirection playerDirection,
            StatusInfoSourceBase statusSource,
            ComboControllerBase comboController,
            GameObject attackerObject)
        {
            _playerDirection = playerDirection;
            _statusSource = statusSource;
            _comboController = comboController;
            _attackerObject = attackerObject;

            if (_animator == null && _attackerObject != null)
                _animator = _attackerObject.GetComponentInChildren<Animator>();
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

            StartCooldown();
            HandleComboOnExecute();
            PlayAnimation();
            StartCastRoutine();
        }

        public override void Cancel()
        {
            StopCastRoutine();
            StopComboDisableRoutine(true);
        }

        public override void Deactivate()
        {
            Cancel();
            base.Deactivate();
        }

        #endregion

        #region Execute

        private bool CanExecute()
        {
            return _castRoutine == null && Time.time >= _cooldownEndTime;
        }

        private void StartCooldown()
        {
            _cooldownEndTime = Time.time + Mathf.Max(0f, _cooldownDuration);
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
            if (_spawnDelay > 0f)
                yield return new WaitForSeconds(_spawnDelay);

            SpawnProjectile();
            _castRoutine = null;
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

            _wasComboActiveBeforeDisable = _comboController.Active;
            _comboController.Disable();

            if (_comboDisableDuration <= 0f)
            {
                if (_wasComboActiveBeforeDisable)
                    _comboController.Enable();

                return;
            }

            _comboDisableRoutine = StartCoroutine(ComboDisableRoutine(_wasComboActiveBeforeDisable));
        }

        private IEnumerator ComboDisableRoutine(bool wasComboActive)
        {
            yield return new WaitForSeconds(_comboDisableDuration);

            if (wasComboActive && _comboController != null)
                _comboController.Enable();

            _comboDisableRoutine = null;
        }

        private void SpawnProjectile()
        {
            Vector3 direction = GetDirection();
            Vector3 spawnPosition = _spawnPoint != null ? _spawnPoint.position : transform.position;
            Quaternion rotation = _spawnPoint != null ? _spawnPoint.rotation : Quaternion.LookRotation(direction, Vector3.up);

            GameObject projectileObject = PoolManager.SpawnObject(_projectilePrefab, spawnPosition, rotation);

            if (!projectileObject.TryGetComponent(out AbilityProjectile abilityProjectile))
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

        private Vector3 GetDirection()
        {
            Vector3 direction = _playerDirection != null ? _playerDirection.DirectionVector : transform.right;
            direction.y = 0f;
            direction.z = 0f;

            return direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.right;
        }

        private void StopCastRoutine()
        {
            if (_castRoutine == null)
                return;

            StopCoroutine(_castRoutine);
            _castRoutine = null;
        }

        private void StopComboDisableRoutine(bool restoreComboState)
        {
            if (_comboDisableRoutine == null)
                return;

            StopCoroutine(_comboDisableRoutine);
            _comboDisableRoutine = null;

            if (restoreComboState && _wasComboActiveBeforeDisable && _comboController != null)
                _comboController.Enable();
        }

        #endregion
    }
}
