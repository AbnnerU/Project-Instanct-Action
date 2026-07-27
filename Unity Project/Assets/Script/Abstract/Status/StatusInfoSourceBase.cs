using UnityEngine;


namespace Scripts.Status
{
    public abstract class StatusInfoSourceBase : MonoBehaviour
    {
        public abstract StatusInfo GetStatusInfo();
    }
}