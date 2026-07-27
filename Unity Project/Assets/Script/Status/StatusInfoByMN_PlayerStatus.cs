using Scripts.Managers;


namespace Scripts.Status
{
    public class StatusInfoByMN_PlayerStatus : StatusInfoSourceBase
    {
        private MN_PlayerStatus _playerStatusManager;

        private void Start() => _playerStatusManager = MN_PlayerStatus.Instance;

        public override StatusInfo GetStatusInfo()
        {
            if(_playerStatusManager)
                return _playerStatusManager.GetStatusInfo();
            else
                return null;
        }
    }
}