using UnityEngine;


namespace Scripts.Status
{
    public abstract class StatusInfoSourceBase : MonoBehaviour
    {
        private StatusInfo _statusInfo;

        public abstract StatusInfo GetStatusInfo();

        public abstract bool TryGetStatusData(StatusType statusType, out StatusData statusData);
    }
}