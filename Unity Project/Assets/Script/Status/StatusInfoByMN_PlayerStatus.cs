using Scripts.Managers;


namespace Scripts.Status
{
    public class StatusInfoByMN_PlayerStatus : StatusInfoSourceBase
    {
        private MN_PlayerStatus _playerStatusManager;

        private void Start() => _playerStatusManager = MN_PlayerStatus.Instance;

        public override StatusInfo GetStatusInfo()
        {
            if(_playerStatusManager == null)
                _playerStatusManager = MN_PlayerStatus.Instance;

            if (_playerStatusManager)
                return _playerStatusManager.GetStatusInfo();
            else
                return null;
        }

        public override bool TryGetStatusData(StatusType statusType, out StatusData statusData)
        {
            statusData = null;

            if (_playerStatusManager == null)
                _playerStatusManager = MN_PlayerStatus.Instance;

            if (_playerStatusManager == null)
                return false;

            return _playerStatusManager.TryGetStatusData(statusType, out statusData);
        }
    }
}
