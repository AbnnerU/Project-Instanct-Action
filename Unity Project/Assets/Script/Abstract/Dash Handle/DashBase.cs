using System;
using System.Collections;
using UnityEngine;

namespace Scripts.DashHandler
{
    public abstract class DashBase : MonoBehaviour, IMovementHandler
    {
        #region Fields

        public event Action<float> OnDashStarted;
        // Also raised on cancellation so listeners always release their locks.
        public event Action<float> OnDashFinished;

        [SerializeField] private bool _active = true;
        [Header("References")]
        [SerializeField] protected Transform _targetTransform;
        [SerializeField] private Transform _directionReference;
        [SerializeField] private DashReferenceAxis _referenceAxis = DashReferenceAxis.FORWARD;
        [Header("Dash")]
        [SerializeField, Min(0f)] protected float _dashDistance = 2f;
        [SerializeField, Min(0f)] protected float _dashDuration = 0.15f;
        [SerializeField] protected AnimationCurve _movementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private Coroutine _dashRoutine;
        private int _executionId;
        private bool _isFinishing;

        public bool Active => _active;
        public bool IsDashing { get; private set; }
        public bool LastDashCanceled { get; private set; }
        public float DirectionDot { get; private set; }
        public bool LastActiveState { get; set; }
        protected bool CanStartDash => _active && isActiveAndEnabled && !IsDashing && !_isFinishing && Time.timeScale > 0f;

        #endregion

        #region Unity

        protected virtual void Awake()
        {
            if (_targetTransform == null)
                _targetTransform = transform;
            if (_directionReference == null)
                _directionReference = _targetTransform;
            LastActiveState = _active;
        }

        protected virtual void OnDisable() => CancelDash();

        #endregion

        #region Execution

        protected bool TryStartDash(Vector3 direction)
        {
            if (!CanStartDash || direction.sqrMagnitude < 0.000001f)
                return false;

            direction.Normalize();
            DirectionDot = GetDirectionDot(direction);
            LastDashCanceled = false;
            IsDashing = true;
            int executionId = ++_executionId;
            OnDashStarted?.Invoke(DirectionDot);
            if (!IsDashing || executionId != _executionId)
                return true;

            Coroutine routine = StartCoroutine(ExecuteDash(direction, executionId));
            if (IsDashing && executionId == _executionId)
                _dashRoutine = routine;
            return true;
        }

        private IEnumerator ExecuteDash(Vector3 direction, int executionId)
        {
            yield return DashRoutine(direction);
            if (IsDashing && executionId == _executionId)
                FinishDash(false);
        }

        protected abstract IEnumerator DashRoutine(Vector3 direction);

        public void CancelDash()
        {
            if (!IsDashing)
                return;
            if (_dashRoutine != null)
                StopCoroutine(_dashRoutine);
            FinishDash(true);
        }

        private void FinishDash(bool canceled)
        {
            _dashRoutine = null;
            IsDashing = false;
            LastDashCanceled = canceled;
            float directionDot = DirectionDot;
            _isFinishing = true;
            try
            {
                OnDashEnded(canceled);
                OnDashFinished?.Invoke(directionDot);
            }
            finally
            {
                _isFinishing = false;
            }
        }

        protected virtual void OnDashEnded(bool canceled) { }

        public float GetDirectionDot(Vector3 direction)
        {
            Transform reference = _directionReference != null ? _directionReference : transform;
            Vector3 referenceDirection = _referenceAxis switch
            {
                DashReferenceAxis.RIGHT => reference.right,
                DashReferenceAxis.UP => reference.up,
                _ => reference.forward
            };
            return Mathf.Clamp(Vector3.Dot(referenceDirection.normalized, direction.normalized), -1f, 1f);
        }

        #endregion

        #region Active State

        public void BlockMovement() => _active = false;
        public void ResetLastState() => _active = LastActiveState;

        public void Disable()
        {
            _active = false;
            LastActiveState = false;
        }

        public void Enable()
        {
            _active = true;
            LastActiveState = true;
        }

        #endregion
    }

    public enum DashReferenceAxis { FORWARD, RIGHT, UP }
}
