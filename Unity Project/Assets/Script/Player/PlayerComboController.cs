using Scripts.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Player
{
    public class PlayerComboController : ComboControllerBase
    {
        [Header("Input")]
        [SerializeField] private InputActionReference _rightInput;
        [SerializeField] private InputActionReference _leftInput;

        [Header("Player")]
        [SerializeField] private PlayerDirection _playerDirection;
        [SerializeField] private DirectionSide _fallbackInputDirection = DirectionSide.RIGHT;

        private DirectionSide _queuedAttackDirection;

        protected override void Awake()
        {
            base.Awake();

            if (_playerDirection == null)
                TryGetComponent(out _playerDirection);
        }

        private void OnEnable()
        {
            if (_rightInput != null)
            {
                _rightInput.action.Enable();
                _rightInput.action.performed += AttackRight_Performed;
            }

            if (_leftInput != null)
            {
                _leftInput.action.Enable();
                _leftInput.action.performed += AttackLeft_Performed;
            }
        }

        private void OnDisable()
        {
            if (_rightInput != null)
                _rightInput.action.performed -= AttackRight_Performed;

            if (_leftInput != null)
                _leftInput.action.performed -= AttackLeft_Performed;
        }

        public override void CancelAttack()
        {
            _queuedAttackDirection = _fallbackInputDirection;
            base.CancelAttack();
        }

        public void RequestAttackRight()
        {
            RequestAttack(DirectionSide.RIGHT);
        }

        public void RequestAttackLeft()
        {
            RequestAttack(DirectionSide.LEFT);
        }

        public void RequestAttack(DirectionSide directionSide)
        {
            if (!_active) return;

            _queuedAttackDirection = directionSide;
            RequestAttack();
        }

        protected override void QueueNextAttack()
        {
            _hasQueuedNextAttack = true;
        }

        protected override void PlayCurrentAttack()
        {
            _playerDirection?.SetDirection(_queuedAttackDirection);
            base.PlayCurrentAttack();
        }

        private void AttackRight_Performed(InputAction.CallbackContext context)
        {
            RequestAttackRight();
        }

        private void AttackLeft_Performed(InputAction.CallbackContext context)
        {
            RequestAttackLeft();
        }
    }
}
