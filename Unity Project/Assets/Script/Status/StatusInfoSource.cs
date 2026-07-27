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
    }
}