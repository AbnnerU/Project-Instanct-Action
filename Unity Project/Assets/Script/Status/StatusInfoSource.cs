using UnityEngine;

namespace Scripts.Status
{
    public class StatusInfoSource : StatusInfoSourceBase
    {
        [SerializeField] private StatusInfo _statusInfo;

        public override StatusInfo GetStatusInfo()
        {
            return _statusInfo;
        }

        public override bool TryGetStatusData(StatusType statusType, out StatusData statusData)
        {
            statusData = null;

            if (_statusInfo == null || _statusInfo.List_StatusData.Count == 0)
                return false;

            statusData = _statusInfo.List_StatusData.Find(x=>x.StatusType == statusType);

            return statusData != null ? true : false;
        }
    }
}