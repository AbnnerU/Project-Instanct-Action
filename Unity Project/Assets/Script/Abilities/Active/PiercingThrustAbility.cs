using System.Collections;
using System.Collections.Generic;
using Scripts.Combat;
using Scripts.Player;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Abilities
{
    public class PiercingThrustAbility : AbilityBase
    {
        #region Fields

        [Header("References")]
        [SerializeField] private Transform _targetTransform;
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

        [Header("Movement")]
        [SerializeField] private float _distance = 2f;
        [SerializeField] private float _duration = 0.15f;
        [SerializeField] private AnimationCurve _movementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Hitbox")]
        [SerializeField] private Vector3 _hitboxCenterOffset = new Vector3(0.75f, 0.75f, 0f);
        [SerializeField] private Vector3 _hitboxHalfExtents = new Vector3(0.75f, 0.75f, 0.75f);
        [SerializeField] private LayerMask _targetLayerMask;
        [SerializeField] private QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
        [SerializeField] private int _maxTargets = 8;

        [Header("Combat Lock")]
        [SerializeField] private bool _cancelAttackOnExecute = true;
        [SerializeField] private bool _disableComboDuringExecution = true;

        [Header("Gizmos")]
        [SerializeField] private bool _drawGizmos = true;
        [SerializeField] private Color _gizmosColor = Color.cyan;

        private Collider[] _hitResults;
        private Coroutine _executionRoutine;
        private readonly HashSet<IDamageReceiver> _hashSet_DamageReceivers = new HashSet<IDamageReceiver>();
        private bool _wasComboActiveBeforeExecution;
        private float _cooldownEndTime;

        #endregion

        #region Public

        public void Setup(
            Transform targetTransform,
            PlayerDirection playerDirection,
            StatusInfoSourceBase statusSource,
            ComboControllerBase comboController,
            GameObject attackerObject)
        {
            _targetTransform = targetTransform;
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

            if (_targetTransform == null)
                _targetTransform = transform;

            StartCooldown();
            PlayAnimation();

            if (_cancelAttackOnExecute)
                _comboController?.CancelAttack(false);

            if (_disableComboDuringExecution && _comboController != null)
            {
                _wasComboActiveBeforeExecution = _comboController.Active;
                _comboController.Disable();
            }

            _executionRoutine = StartCoroutine(ExecutionRoutine());
        }

        public override void Cancel()
        {
            if (_executionRoutine != null)
            {
                StopCoroutine(_executionRoutine);
                _executionRoutine = null;
            }

            RestoreComboState();
            _hashSet_DamageReceivers.Clear();
        }

        public override void Deactivate()
        {
            Cancel();
            base.Deactivate();
        }

        #endregion

        #region Execution

        private bool CanExecute()
        {
            return _executionRoutine == null && Time.time >= _cooldownEndTime;
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

        private IEnumerator ExecutionRoutine()
        {
            EnsureHitResultsCapacity();
            _hashSet_DamageReceivers.Clear();

            Vector3 direction = GetDirection();
            Vector3 startPosition = _targetTransform.position;
            Vector3 endPosition = startPosition + direction * _distance;
            float elapsedTime = 0f;

            while (elapsedTime < _duration)
            {
                elapsedTime += Time.deltaTime;

                float percentage = _duration <= 0f ? 1f : Mathf.Clamp01(elapsedTime / _duration);
                float curveValue = _movementCurve != null ? _movementCurve.Evaluate(percentage) : percentage;

                _targetTransform.position = Vector3.LerpUnclamped(startPosition, endPosition, curveValue);
                ExecuteHitbox(direction);

                yield return null;
            }

            _targetTransform.position = endPosition;
            ExecuteHitbox(direction);

            RestoreComboState();
            _hashSet_DamageReceivers.Clear();
            _executionRoutine = null;
        }

        private void ExecuteHitbox(Vector3 direction)
        {
            Vector3 center = GetHitboxCenter(direction);

            int hits = Physics.OverlapBoxNonAlloc(
                center,
                _hitboxHalfExtents,
                _hitResults,
                Quaternion.identity,
                _targetLayerMask,
                _queryTriggerInteraction);

            for (int i = 0; i < hits; i++)
            {
                Collider hitCollider = _hitResults[i];

                if (hitCollider == null || !TryGetDamageReceiver(hitCollider, out IDamageReceiver damageReceiver))
                    continue;

                if (!_hashSet_DamageReceivers.Add(damageReceiver))
                    continue;

                GameObject receiverObject = damageReceiver is Component component ? component.gameObject : hitCollider.gameObject;
                DamageRequest damageRequest = new DamageRequest(
                    _damageData,
                    _statusSource != null ? _statusSource.GetStatusInfo() : null,
                    _attackerObject != null ? _attackerObject : gameObject,
                    receiverObject,
                    hitCollider,
                    hitCollider.ClosestPoint(center));

                damageReceiver.ReceiveDamage(damageRequest);
            }
        }

        private void RestoreComboState()
        {
            if (!_disableComboDuringExecution || _comboController == null)
                return;

            if (_wasComboActiveBeforeExecution)
                _comboController.Enable();
        }

        #endregion

        #region Helpers

        private Vector3 GetDirection()
        {
            Vector3 direction = _playerDirection != null ? _playerDirection.DirectionVector : transform.right;
            direction.y = 0f;
            direction.z = 0f;

            return direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.right;
        }

        private Vector3 GetHitboxCenter(Vector3 direction)
        {
            Vector3 offset = _hitboxCenterOffset;
            offset.x = Mathf.Abs(offset.x) * Mathf.Sign(direction.x);

            return (_targetTransform != null ? _targetTransform.position : transform.position) + offset;
        }

        private bool TryGetDamageReceiver(Collider hitCollider, out IDamageReceiver damageReceiver)
        {
            damageReceiver = hitCollider.GetComponentInParent<IDamageReceiver>();

            if (damageReceiver != null)
                return true;

            damageReceiver = hitCollider.GetComponentInChildren<IDamageReceiver>();
            return damageReceiver != null;
        }

        private void EnsureHitResultsCapacity()
        {
            int capacity = Mathf.Max(1, _maxTargets);

            if (_hitResults != null && _hitResults.Length >= capacity)
                return;

            _hitResults = new Collider[capacity];
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmos)
                return;

            Vector3 direction = Application.isPlaying ? GetDirection() : Vector3.right;
            Vector3 center = GetHitboxCenter(direction);

            Gizmos.color = _gizmosColor;
            Gizmos.DrawWireCube(center, _hitboxHalfExtents * 2f);
        }

        #endregion
    }
}
