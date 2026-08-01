using System;
using System.Collections;
using Scripts.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Player
{
    public class PlayerDash : MonoBehaviour, IMovementHandler
    {
        public event Action<DirectionSide> OnDashStarted;
        public event Action<DirectionSide> OnDashFinished;
        public event Action<DirectionSide, float> OnDashCooldownStarted;
        public event Action<DirectionSide> OnDashCooldownFinished;

        [SerializeField] private bool _active = true;
        [Header("Input")]
        [SerializeField] private InputActionReference _rightInput;
        [SerializeField] private InputActionReference _leftInput;

        [Header("References")]
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private PlayerDirection _playerDirection;
        [SerializeField] private ComboControllerBase _comboController;

        [Header("Dash")]
        [SerializeField] private float _dashDistance = 2f;
        [SerializeField] private float _dashDuration = 0.15f;
        [SerializeField] private AnimationCurve _movementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Cooldown")]
        [SerializeField] private float _rightCooldown = 0.75f;
        [SerializeField] private float _leftCooldown = 0.75f;

        private bool _isDashing;
        private float _rightCooldownTimer;
        private float _leftCooldownTimer;
        private Coroutine _dashRoutine;

        public bool Active => _active;
        public bool IsDashing => _isDashing;
        public float RightCooldownTimer => _rightCooldownTimer;
        public float LeftCooldownTimer => _leftCooldownTimer;

        public bool LastActiveState { get ; set; }

        private void Awake()
        {
            if (_targetTransform == null)
                _targetTransform = transform;

            if (_playerDirection == null)
                TryGetComponent(out _playerDirection);

            if (_comboController == null)
                TryGetComponent(out _comboController);

            LastActiveState = _active;
        }

        private void OnEnable()
        {
            if (_rightInput != null)
            {
                _rightInput.action.Enable();
                _rightInput.action.performed += RightInput_Performed;
            }

            if (_leftInput != null)
            {
                _leftInput.action.Enable();
                _leftInput.action.performed += LeftInput_Performed;
            }
        }

        private void OnDisable()
        {
            if (_rightInput != null)
                _rightInput.action.performed -= RightInput_Performed;

            if (_leftInput != null)
                _leftInput.action.performed -= LeftInput_Performed;
        }

        private void Update()
        {
            UpdateCooldownTimer(ref _rightCooldownTimer, DirectionSide.RIGHT);
            UpdateCooldownTimer(ref _leftCooldownTimer, DirectionSide.LEFT);
        }

        private void RightInput_Performed(InputAction.CallbackContext context)
        {
            if(_active)
            TryDash(DirectionSide.RIGHT);
        }

        private void LeftInput_Performed(InputAction.CallbackContext context)
        {
            if(_active)
            TryDash(DirectionSide.LEFT);
        }

        public bool TryDash(DirectionSide directionSide)
        {
            if (_isDashing || IsOnCooldown(directionSide)) return false;

            _playerDirection?.SetDirection(directionSide);
            _comboController?.Disable();
            _comboController?.CancelAttack(false);

            _dashRoutine = StartCoroutine(DashRoutine(directionSide));
            return true;
        }

        public bool IsOnCooldown(DirectionSide directionSide)
        {
            return directionSide == DirectionSide.RIGHT ? _rightCooldownTimer > 0f : _leftCooldownTimer > 0f;
        }

        private IEnumerator DashRoutine(DirectionSide directionSide)
        {
            _isDashing = true;
            OnDashStarted?.Invoke(directionSide);

            Vector3 startPosition = _targetTransform.position;
            Vector3 endPosition = startPosition + GetDirectionVector(directionSide) * _dashDistance;

            float elapsedTime = 0f;

            while (elapsedTime < _dashDuration)
            {
                elapsedTime += Time.deltaTime;
                float percentage = _dashDuration <= 0f ? 1f : Mathf.Clamp01(elapsedTime / _dashDuration);
                float curveValue = _movementCurve != null ? _movementCurve.Evaluate(percentage) : percentage;

                _targetTransform.position = Vector3.LerpUnclamped(startPosition, endPosition, curveValue);
                yield return null;
            }

            _targetTransform.position = endPosition;
            _isDashing = false;
            _dashRoutine = null;
            _comboController?.Enable();

            StartCooldown(directionSide);
            OnDashFinished?.Invoke(directionSide);
        }

        private void StartCooldown(DirectionSide directionSide)
        {
            float cooldown = GetCooldown(directionSide);

            if (directionSide == DirectionSide.RIGHT)
                _rightCooldownTimer = cooldown;
            else
                _leftCooldownTimer = cooldown;

            OnDashCooldownStarted?.Invoke(directionSide, cooldown);
        }

        private void UpdateCooldownTimer(ref float cooldownTimer, DirectionSide directionSide)
        {
            if (cooldownTimer <= 0f) return;

            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer > 0f) return;

            cooldownTimer = 0f;
            OnDashCooldownFinished?.Invoke(directionSide);
        }

        private float GetCooldown(DirectionSide directionSide)
        {
            return directionSide == DirectionSide.RIGHT ? _rightCooldown : _leftCooldown;
        }

        private Vector3 GetDirectionVector(DirectionSide directionSide)
        {
            return directionSide == DirectionSide.RIGHT ? Vector3.right : Vector3.left;
        }

        public void BlockMovement()
        {
            _active = false;
        }

        public void ResetLastState()
        {
            _active = LastActiveState;
        }

        public void Disable()
        {
            _active = false;
            LastActiveState = _active;
        }

        public void Enable()
        {
            _active = true;
            LastActiveState = _active;
        }
    }
}
