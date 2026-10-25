using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Scripts.Combat
{
    public class ComboController : ComboControllerBase
    {
        #region Fields

        [Header("Input")]
        [FormerlySerializedAs("_rightInput")]
        [SerializeField] private InputActionReference _attackInput;

        #endregion

        #region Unity

        private void OnEnable()
        {
            if (_attackInput == null || _attackInput.action == null)
                return;
            _attackInput.action.performed += Attack_Performed;
            _attackInput.action.Enable();
        }

        protected override void OnDisable()
        {
            if (_attackInput != null && _attackInput.action != null)
                _attackInput.action.performed -= Attack_Performed;
            base.OnDisable();
        }

        #endregion

        #region Attack

        private void Attack_Performed(InputAction.CallbackContext context)
        {
            if (Time.timeScale <= 0f) return;
            RequestAttack();
        }

        #endregion
    }
}
