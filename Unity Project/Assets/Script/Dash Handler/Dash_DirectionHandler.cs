using Scripts.Direction;
using UnityEngine;

namespace Scripts.DashHandler
{
    public class Dash_DirectionHandler : MonoBehaviour
    {
        #region Fields

        [SerializeField] private DashBase _dash;
        [SerializeField] private DirectionHandler _directionHandler;

        #endregion

        #region Unity

        private void Awake()
        {
            if (_dash == null)
                TryGetComponent(out _dash);
            if (_directionHandler == null)
                TryGetComponent(out _directionHandler);
        }

        private void OnEnable()
        {
            if (_dash == null)
                return;
            _dash.OnDashStarted += Dash_OnStarted;
            _dash.OnDashFinished += Dash_OnFinished;
            if (_dash.IsDashing)
                Dash_OnStarted(_dash.DirectionDot);
        }

        private void OnDisable()
        {
            if (_dash != null)
            {
                _dash.OnDashStarted -= Dash_OnStarted;
                _dash.OnDashFinished -= Dash_OnFinished;
            }
            Dash_OnFinished(0f);
        }

        #endregion

        #region Events

        private void Dash_OnStarted(float directionDot)
        {
            if (_directionHandler != null)
                _directionHandler.BlockDirection(this);
        }

        private void Dash_OnFinished(float directionDot)
        {
            if (_directionHandler != null)
                _directionHandler.UnblockDirection(this);
        }

        #endregion
    }
}
