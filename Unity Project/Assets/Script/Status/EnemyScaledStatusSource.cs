using Scripts.Progression;
using UnityEngine;

namespace Scripts.Status
{
    [DefaultExecutionOrder(100)]
    public class EnemyScaledStatusSource : StatusInfoSourceBase
    {
        #region Fields

        [SerializeField] private StatusInfo _statusInfo = new StatusInfo();

        #endregion

        #region Unity

        private void OnEnable()
        {
            MN_EnemyProgression progression = MN_EnemyProgression.Instance;

            if (progression == null)
                Debug.LogWarning($"[{nameof(EnemyScaledStatusSource)}] No enemy progression manager; using base values.", this);

            for (int i = 0; i < _statusInfo.List_StatusData.Count; i++)
            {
                StatusData statusData = _statusInfo.List_StatusData[i];
                if (statusData == null)
                    continue;

                float multiplierPercent = progression != null
                    ? progression.GetMultiplierPercent(statusData.StatusType)
                    : 100f;

                statusData.CurrentValue = statusData.BaseValue * multiplierPercent / 100f;
                statusData.OnCurrentValueUpdated?.Invoke(statusData.CurrentValue);
            }
        }

        #endregion

        #region Status

        public override StatusInfo GetStatusInfo()
        {
            return _statusInfo;
        }

        public override bool TryGetStatusData(StatusType statusType, out StatusData statusData)
        {
            statusData = _statusInfo.GetStatusData(statusType);
            return statusData != null;
        }

        #endregion
    }
}
