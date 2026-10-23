using Scripts.Combat;
using UnityEngine;

namespace Scripts.DashHandler
{
    public class Dash_ComboControllerHandler : MonoBehaviour
    {
        #region Fields

        [SerializeField] private DashBase _dashHandler;
        [SerializeField] private ComboControllerBase _comboController;
        [SerializeField] private bool _maintainComboIndex;

        private bool _isBlocking;

        #endregion

        #region Unity

        private void Awake()
        {
            if (_dashHandler == null)
                TryGetComponent(out _dashHandler);
            if (_comboController == null)
                TryGetComponent(out _comboController);
        }

        private void OnEnable()
        {
            if (_dashHandler == null)
                return;
            _dashHandler.OnDashStarted += OnDashStarted;
            _dashHandler.OnDashFinished += OnDashFinished;
            if (_dashHandler.IsDashing)
                OnDashStarted(_dashHandler.DirectionDot);
        }

        private void OnDisable()
        {
            if (_dashHandler != null)
            {
                _dashHandler.OnDashStarted -= OnDashStarted;
                _dashHandler.OnDashFinished -= OnDashFinished;
            }
            OnDashFinished(0f);
        }

        #endregion

        #region Events

        private void OnDashFinished(float directionDot)
        {
            if (!_isBlocking)
                return;
            _isBlocking = false;
            if (_comboController != null)
                _comboController.UnblockAttackInput(this);
        }

        private void OnDashStarted(float directionDot)
        {
            if (_isBlocking || _comboController == null)
                return;
            _isBlocking = true;
            _comboController.BlockAttackInput(this);
            _comboController.CancelAttack(false, _maintainComboIndex);
        }

        #endregion
    }
}
