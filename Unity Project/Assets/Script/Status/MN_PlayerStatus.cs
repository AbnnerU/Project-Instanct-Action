using System;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Status;

namespace Scripts.Managers
{
    public class MN_PlayerStatus : SceneSingleton<MN_PlayerStatus>
    {
        public event Action<StatusType, float> OnCurrentValueUpdated;
        public event Action<StatusType, float> OnBaseValueUpdated;

        [SerializeField] private StatusInfo _statusInfo;

        private readonly Dictionary<StatusType, StatusData> _dc_StatusData = new Dictionary<StatusType, StatusData>();

        protected override void Awake()
        {
            base.Awake();
            BuildStatusDataDictionary();
        }

        public bool TryGetStatusData(StatusType statusType, out StatusData statusData)
        {
            return _dc_StatusData.TryGetValue(statusType, out statusData);
        }

        public StatusInfo GetStatusInfo()
        {
            return _statusInfo;
        }

        public float GetBaseValue(StatusType statusType)
        {
            return TryGetStatusData(statusType, out StatusData statusData) ? statusData.BaseValue : 0f;
        }

        public float GetCurrentValue(StatusType statusType)
        {
            return TryGetStatusData(statusType, out StatusData statusData) ? statusData.CurrentValue : 0f;
        }

        public void SetBaseValue(StatusType statusType, float value, bool updateCurrentValue)
        {
            if (!TryGetStatusData(statusType, out StatusData statusData)) return;

            statusData.BaseValue = value;
            OnBaseValueUpdated?.Invoke(statusType, statusData.BaseValue);

            if (updateCurrentValue)
                SetCurrentValue(statusType, value);
        }

        public void SetCurrentValue(StatusType statusType, float value)
        {
            if (!TryGetStatusData(statusType, out StatusData statusData)) return;

            statusData.CurrentValue = value;
            statusData.OnCurrentValueUpdated?.Invoke(statusData.CurrentValue);

            OnCurrentValueUpdated?.Invoke(statusType, statusData.CurrentValue);
        }

        public void AddCurrentValue(StatusType statusType, float value)
        {
            if (!TryGetStatusData(statusType, out StatusData statusData)) return;

            SetCurrentValue(statusType, statusData.CurrentValue + value);
        }

        public void ResetCurrentValue(StatusType statusType)
        {
            if (!TryGetStatusData(statusType, out StatusData statusData)) return;

            SetCurrentValue(statusType, statusData.BaseValue);
        }

        public void ResetAllCurrentValues()
        {
            for (int i = 0; i < _statusInfo.List_StatusData.Count; i++)
                SetCurrentValue(_statusInfo.List_StatusData[i].StatusType, _statusInfo.List_StatusData[i].BaseValue);
        }

        private void BuildStatusDataDictionary()
        {
            _statusInfo.Initialize();

            _dc_StatusData.Clear();

            for (int i = 0; i < _statusInfo.List_StatusData.Count; i++)
            {
                StatusData statusData = _statusInfo.List_StatusData[i];

                if (_dc_StatusData.ContainsKey(statusData.StatusType))
                {
                    Debug.LogWarning($"Duplicate player status ignored: {statusData.StatusType}", this);
                    continue;
                }

                _dc_StatusData.Add(statusData.StatusType, statusData);
            }
        }
    }
}