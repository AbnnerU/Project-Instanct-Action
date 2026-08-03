using System;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts.Combat
{
    public class DamageKnockbackReceiver : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _damageReceiverReference;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private NavMeshAgent _navMeshAgent;

        [Header("Settings")]
        [SerializeField] private bool _active = true;
        [SerializeField] private bool _ignoreIfKilled = true;
        [SerializeField] private bool _useNavMeshAgentMove = true;
        [SerializeField] private bool _stopNavMeshAgentDuringKnockback = true;

        private IDamageReceiver _damageReceiver;
        private Coroutine _knockbackRoutine;
        private bool _agentWasStopped;
        private bool _hasStoredAgentState;

        private void Awake()
        {
            GetReferences();
        }

        private void OnEnable()
        {
            GetReferences();

            if (_damageReceiver != null)
                _damageReceiver.OnDamageReceived += DamageReceiver_OnDamageReceived;
        }

        private void OnDisable()
        {
            if (_damageReceiver != null)
                _damageReceiver.OnDamageReceived -= DamageReceiver_OnDamageReceived;

            StopKnockback(true);
        }

        public void Enable() => _active = true;

        public void Disable()
        {
            _active = false;
            StopKnockback(true);
        }

        private void DamageReceiver_OnDamageReceived(DamageResult damageResult)
        {
            if (!_active) return;
            if (!damageResult.WasApplied) return;
            if (_ignoreIfKilled && damageResult.Killed) return;

            ComboAttackData attackData = damageResult.DamageRequest.ComboAttackData;

            if (attackData == null) return;
            if (Mathf.Approximately(attackData.KnockbackDistance, 0f)) return;

            Vector3 direction = GetKnockbackDirection(damageResult);

            if (direction.sqrMagnitude <= 0.0001f)
                return;

            StopKnockback(true);
            _knockbackRoutine = StartCoroutine(KnockbackRoutine(direction, attackData.KnockbackDistance, attackData.KnockbackDuration, attackData.KnockbackCurve));
        }

        private System.Collections.IEnumerator KnockbackRoutine(Vector3 direction, float distance, float duration, AnimationCurve curve)
        {
            StoreAndStopAgentIfNeeded();

            if (duration <= 0f)
            {
                Move(direction * distance);
                StopKnockback(true);
                yield break;
            }

            float elapsedTime = 0f;
            float previousCurveValue = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                float percentage = Mathf.Clamp01(elapsedTime / duration);
                float curveValue = curve != null ? curve.Evaluate(percentage) : percentage;
                float deltaCurve = curveValue - previousCurveValue;

                Move(direction * (distance * deltaCurve));

                previousCurveValue = curveValue;
                yield return null;
            }

            _knockbackRoutine = null;
            RestoreAgentStateIfNeeded();
        }

        private void GetReferences()
        {
            if (_targetTransform == null)
                _targetTransform = transform;

            if (_damageReceiverReference == null)
                _damageReceiverReference = gameObject;

            _damageReceiver = _damageReceiverReference.GetComponent<IDamageReceiver>();

            if (_damageReceiver == null)
                _damageReceiver = _damageReceiverReference.GetComponentInParent<IDamageReceiver>();

            if (_damageReceiver == null)
                _damageReceiver = _damageReceiverReference.GetComponentInChildren<IDamageReceiver>();

            if (_navMeshAgent == null)
                TryGetComponent(out _navMeshAgent);
        }

        private Vector3 GetKnockbackDirection(DamageResult damageResult)
        {
            Vector3 origin = damageResult.Attacker != null ? damageResult.Attacker.transform.position : damageResult.HitPoint;
            Vector3 direction = _targetTransform.position - origin;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.0001f)
                return direction.normalized;

            direction = -_targetTransform.forward;
            direction.y = 0f;

            return direction.normalized;
        }

        private void Move(Vector3 delta)
        {
            if (_targetTransform == null)
                return;

            if (_useNavMeshAgentMove && _navMeshAgent != null && _navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.Move(delta);
                return;
            }

            _targetTransform.position += delta;
        }

        private void StopKnockback(bool restoreAgentState)
        {
            if (_knockbackRoutine != null)
            {
                StopCoroutine(_knockbackRoutine);
                _knockbackRoutine = null;
            }

            if (restoreAgentState)
                RestoreAgentStateIfNeeded();
        }

        private void StoreAndStopAgentIfNeeded()
        {
            if (!_stopNavMeshAgentDuringKnockback || _navMeshAgent == null || !_navMeshAgent.enabled || !_navMeshAgent.isOnNavMesh)
                return;

            _agentWasStopped = _navMeshAgent.isStopped;
            _hasStoredAgentState = true;
            _navMeshAgent.isStopped = true;
        }

        private void RestoreAgentStateIfNeeded()
        {
            if (!_hasStoredAgentState || _navMeshAgent == null || !_navMeshAgent.enabled)
                return;

            _navMeshAgent.isStopped = _agentWasStopped;
            _hasStoredAgentState = false;
        }
    }
}
