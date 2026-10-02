using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

namespace Scripts.DashHandler
{
    [MovedFrom(true, "Scripts.DashHandler", null, "Dash")]
    public class Dash2D : DashBase
    {
        #region Fields

        public event Action<DashInputSide, float> OnDashCooldownStarted;
        public event Action<DashInputSide> OnDashCooldownFinished;

        [Header("Input")]
        [SerializeField] private InputActionReference _rightInput;
        [SerializeField] private InputActionReference _leftInput;

        [Header("2D Movement")]
        [SerializeField] private DashMovementAxis _movementAxis = DashMovementAxis.X;
        [Header("Cooldown Per Input Side")]
        [SerializeField, Min(0f)] private float _rightCooldown = 0.75f;
        [SerializeField, Min(0f)] private float _leftCooldown = 0.75f;

        private float _rightCooldownTimer;
        private float _leftCooldownTimer;
        private DashInputSide _currentInputSide;

        public float RightCooldownTimer => _rightCooldownTimer;
        public float LeftCooldownTimer => _leftCooldownTimer;

        #endregion

        #region Input

        private void OnEnable()
        {
            if (_rightInput != null && _rightInput.action != null)
            {
                _rightInput.action.performed += RightInput_Performed;
                _rightInput.action.Enable();
            }
            if (_leftInput != null && _leftInput.action != null)
            {
                _leftInput.action.performed += LeftInput_Performed;
                _leftInput.action.Enable();
            }
        }

        protected override void OnDisable()
        {
            if (_rightInput != null && _rightInput.action != null)
                _rightInput.action.performed -= RightInput_Performed;
            if (_leftInput != null && _leftInput.action != null)
                _leftInput.action.performed -= LeftInput_Performed;
            base.OnDisable();
        }

        private void RightInput_Performed(InputAction.CallbackContext context)
        {
            TryDash(DashInputSide.RIGHT);
        }

        private void LeftInput_Performed(InputAction.CallbackContext context)
        {
            TryDash(DashInputSide.LEFT);
        }

        #endregion

        #region Execution

        public bool TryDash(DashInputSide inputSide)
        {
            if (!CanStartDash || IsOnCooldown(inputSide))
                return false;

            _currentInputSide = inputSide;
            return TryStartDash(GetDirectionVector(inputSide));
        }

        public Vector3 GetDirectionVector(DashInputSide inputSide)
        {
            Vector3 axis = _movementAxis switch
            {
                DashMovementAxis.Y => Vector3.up,
                DashMovementAxis.Z => Vector3.forward,
                _ => Vector3.right
            };
            return inputSide == DashInputSide.RIGHT ? axis : -axis;
        }

        protected override IEnumerator DashRoutine(Vector3 direction)
        {
            Vector3 startPosition = _targetTransform.position;
            Vector3 endPosition = startPosition + direction * _dashDistance;
            float elapsedTime = 0f;

            while (elapsedTime < _dashDuration)
            {
                elapsedTime += Time.deltaTime;
                float percentage = Mathf.Clamp01(elapsedTime / _dashDuration);
                float curveValue = _movementCurve != null ? _movementCurve.Evaluate(percentage) : percentage;
                _targetTransform.position = Vector3.LerpUnclamped(startPosition, endPosition, curveValue);
                yield return null;
            }

            _targetTransform.position = endPosition;
        }

        protected override void OnDashEnded(bool canceled)
        {
            StartCooldown(_currentInputSide);
        }

        #endregion

        #region Cooldown

        protected virtual void Update()
        {
            UpdateCooldownTimer(ref _rightCooldownTimer, DashInputSide.RIGHT);
            UpdateCooldownTimer(ref _leftCooldownTimer, DashInputSide.LEFT);
        }

        public virtual bool IsOnCooldown(DashInputSide inputSide) => GetCooldownTimer(inputSide) > 0f;

        public float GetCooldownFillAmount(DashInputSide inputSide)
        {
            float cooldown = GetCooldown(inputSide);
            return cooldown <= 0f ? 0f : Mathf.Clamp01(GetCooldownTimer(inputSide) / cooldown);
        }

        protected virtual void StartCooldown(DashInputSide inputSide)
        {
            float cooldown = Mathf.Max(0f, GetCooldown(inputSide));
            if (inputSide == DashInputSide.RIGHT)
                _rightCooldownTimer = cooldown;
            else
                _leftCooldownTimer = cooldown;
            OnDashCooldownStarted?.Invoke(inputSide, cooldown);
            if (cooldown == 0f)
                OnDashCooldownFinished?.Invoke(inputSide);
        }

        protected virtual void UpdateCooldownTimer(ref float timer, DashInputSide inputSide)
        {
            if (timer <= 0f)
                return;
            timer = Mathf.Max(0f, timer - Time.deltaTime);
            if (timer == 0f)
                OnDashCooldownFinished?.Invoke(inputSide);
        }

        protected virtual float GetCooldown(DashInputSide inputSide) =>
            inputSide == DashInputSide.RIGHT ? _rightCooldown : _leftCooldown;

        private float GetCooldownTimer(DashInputSide inputSide) =>
            inputSide == DashInputSide.RIGHT ? _rightCooldownTimer : _leftCooldownTimer;

        #endregion
    }

    public enum DashInputSide { LEFT, RIGHT }
    public enum DashMovementAxis { X, Y, Z }
}
