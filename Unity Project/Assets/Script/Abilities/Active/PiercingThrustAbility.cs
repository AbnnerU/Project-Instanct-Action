using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Combat;
using Scripts.Player;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Abilities
{
    public class PiercingThrustAbility : AbilityBase, IHasCastTime, IDamageDataAbility
    {
        public event Action<float> OnCastTimeStarted;
        public event Action<float> OnCastTimeFinished;

        [Header("References")]
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private GameObject _attackerObject;
        [SerializeField] private Animator _animator;
       
        [SerializeField] private float _castTime;
        [Header("Damage")]
        [SerializeField] private DamageData _damageData = new DamageData();
        [Header("Animation")]
        [SerializeField] private string _animationStateName;
        [SerializeField] private float _animationTransitionDuration = 0.05f;
        [SerializeField] private int _animationLayerIndex;
        [Header("Movement")]
        [SerializeField] private float _distance = 2f;
        [SerializeField] private float _duration = 0.15f;
        [SerializeField] private AnimationCurve _movementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private bool _setEndPosition;
        [Header("Hitbox")]
        [SerializeField] private Vector3 _hitboxCenterOffset = new Vector3(0.75f, 0.75f, 0f);
        [SerializeField] private Vector3 _hitboxHalfExtents = new Vector3(0.75f, 0.75f, 0.75f);
        [SerializeField] private LayerMask _targetLayerMask;
        [SerializeField] private QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
        [SerializeField] private int _maxTargets = 8;
        [Header("Collider")]
        [SerializeField] private Collider _colliderComponent;
        [SerializeField] private bool _disableColliderDuringExecution = true;
        [Header("Combat Lock")]
        [SerializeField] private ComboControllerBase _comboController;
        [SerializeField] private bool _cancelAttackOnExecute = true;
        [SerializeField] private bool _disableComboDuringExecution = true;
        [Header("Dash Lock")]
        [SerializeField] private PlayerDash _dashComponent;
        [SerializeField] private bool _disableDashDuringExecution = true;
        [Header("Gizmos")]
        [SerializeField] private bool _drawGizmos = true;
        [SerializeField] private Color _gizmosColor = Color.cyan;

        private Collider[] _hitResults;
        private Coroutine _executionRoutine;
        private readonly HashSet<IDamageReceiver> _hashSet_DamageReceivers = new HashSet<IDamageReceiver>();
        public float CastTime { get => _castTime; set => _castTime = value; }


        #region IDamageDataAbility
        public void SetDamageData(IReadOnlyList<DamageData> list_Damage)
        {
            if (list_Damage != null && list_Damage.Count > 0 && list_Damage[0] != null)
                _damageData = list_Damage[0];
        }
        #endregion


        #region Public

        public void Setup(Transform targetTransform, StatusInfoSourceBase statusSource, ComboControllerBase comboController, GameObject attackerObject, PlayerDash dash)
        {
            _targetTransform = targetTransform;
            _statusSource = statusSource;
            _comboController = comboController;
            _attackerObject = attackerObject;
            _dashComponent = dash;

            if(_disableColliderDuringExecution)
                _colliderComponent = targetTransform.GetComponent<Collider>();

            if (_animator == null && _attackerObject != null)
                _animator = _attackerObject.GetComponentInChildren<Animator>();
        }

        public override void Execute()
        {
            if (!CanExecute())
                return;

            if (_targetTransform == null)
                _targetTransform = transform;

            PlayAnimation();

            if (_cancelAttackOnExecute)
                _comboController?.CancelAttack(false);

            if (_disableComboDuringExecution && _comboController != null)
                _comboController.Disable();

            if (_disableDashDuringExecution && _dashComponent != null)        
                _dashComponent.Disable();

            if(_disableColliderDuringExecution && _colliderComponent != null)
                _colliderComponent.enabled = false;
          
            _executionRoutine = StartCoroutine(ExecutionRoutine());
        }

        public override void Cancel()
        {
            bool wasExecuting = _executionRoutine != null;

            if (_executionRoutine != null)
            {
                StopCoroutine(_executionRoutine);
                _executionRoutine = null;
            }

            if (_disableComboDuringExecution && _comboController != null)
                _comboController.Enable();

            if (_disableDashDuringExecution && _dashComponent != null)
                _dashComponent.Enable();

            if (_disableColliderDuringExecution && _colliderComponent != null)
                _colliderComponent.enabled = true;

            _hashSet_DamageReceivers.Clear();

            if (wasExecuting)
                OnAbilityFinished?.Invoke(this);
        }

        public override void Deactivate()
        {
            Cancel();
            base.Deactivate();
        }

        #endregion

        #region Execution

        protected override bool CanExecute()
        {
            return _executionRoutine == null && _active;
        }

        private void PlayAnimation()
        {
            if (_animator == null || string.IsNullOrEmpty(_animationStateName))
                return;

            _animator.CrossFade(_animationStateName, _animationTransitionDuration, _animationLayerIndex, 0f);
        }

        private IEnumerator ExecutionRoutine()
        {
            OnAbilityStarted?.Invoke(this);

            if (_castTime > 0)
            {
                OnCastTimeStarted?.Invoke(_castTime);
                yield return new WaitForSeconds(_castTime);
                OnCastTimeFinished?.Invoke(_castTime);
            }

            EnsureHitResultsCapacity();
            _hashSet_DamageReceivers.Clear();

            Vector3 direction = _targetTransform.forward;
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

            if(_setEndPosition)
                _targetTransform.position = endPosition;

            ExecuteHitbox(direction);

            if (_disableComboDuringExecution)
                _comboController.Enable();

            if(_disableDashDuringExecution)
                _dashComponent.Enable();

            if (_disableColliderDuringExecution)
                _colliderComponent.enabled = true;

            _hashSet_DamageReceivers.Clear();
            _executionRoutine = null;

            OnAbilityFinished?.Invoke(this);
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

        #endregion

        #region Helpers
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

            if (_targetTransform)
            {

                Vector3 direction = _targetTransform.forward;
                Vector3 center = GetHitboxCenter(direction);

                Gizmos.color = _gizmosColor;
                Gizmos.DrawWireCube(center, _hitboxHalfExtents * 2f);
            }
        }

        #endregion
    }
}
