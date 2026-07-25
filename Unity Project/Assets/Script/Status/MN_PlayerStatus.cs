using System;
using System.Collections.Generic;
using UnityEngine;
using StatusType = Scripts.Status.StatusType;

namespace Scripts.Managers
{
    public class MN_PlayerStatus : global::SceneSingleton<MN_PlayerStatus>
    {
        public event Action<StatusType, float> OnCurrentValueUpdated;
        public event Action<StatusType, float> OnBaseValueUpdated;

        [SerializeField] private List<PlayerStatusData> _list_StatusData = new List<PlayerStatusData>();

        private readonly Dictionary<StatusType, PlayerStatusData> _dc_StatusData = new Dictionary<StatusType, PlayerStatusData>();

        protected override void Awake()
        {
            base.Awake();
            BuildStatusDataDictionary();
        }

        public bool TryGetStatusData(StatusType statusType, out PlayerStatusData statusData)
        {
            return _dc_StatusData.TryGetValue(statusType, out statusData);
        }

        public float GetBaseValue(StatusType statusType)
        {
            return TryGetStatusData(statusType, out PlayerStatusData statusData) ? statusData.BaseValue : 0f;
        }

        public float GetCurrentValue(StatusType statusType)
        {
            return TryGetStatusData(statusType, out PlayerStatusData statusData) ? statusData.CurrentValue : 0f;
        }

        public void SetBaseValue(StatusType statusType, float value, bool updateCurrentValue)
        {
            if (!TryGetStatusData(statusType, out PlayerStatusData statusData)) return;

            statusData.BaseValue = value;
            OnBaseValueUpdated?.Invoke(statusType, statusData.BaseValue);

            if (updateCurrentValue)
                SetCurrentValue(statusType, value);
        }

        public void SetCurrentValue(StatusType statusType, float value)
        {
            if (!TryGetStatusData(statusType, out PlayerStatusData statusData)) return;

            statusData.CurrentValue = value;
            OnCurrentValueUpdated?.Invoke(statusType, statusData.CurrentValue);
        }

        public void AddCurrentValue(StatusType statusType, float value)
        {
            if (!TryGetStatusData(statusType, out PlayerStatusData statusData)) return;

            SetCurrentValue(statusType, statusData.CurrentValue + value);
        }

        public void ResetCurrentValue(StatusType statusType)
        {
            if (!TryGetStatusData(statusType, out PlayerStatusData statusData)) return;

            SetCurrentValue(statusType, statusData.BaseValue);
        }

        public void ResetAllCurrentValues()
        {
            for (int i = 0; i < _list_StatusData.Count; i++)
                SetCurrentValue(_list_StatusData[i].StatusType, _list_StatusData[i].BaseValue);
        }

        private void BuildStatusDataDictionary()
        {
            _dc_StatusData.Clear();

            for (int i = 0; i < _list_StatusData.Count; i++)
            {
                PlayerStatusData statusData = _list_StatusData[i];

                if (_dc_StatusData.ContainsKey(statusData.StatusType))
                {
                    Debug.LogWarning($"Duplicate player status ignored: {statusData.StatusType}", this);
                    continue;
                }

                _dc_StatusData.Add(statusData.StatusType, statusData);
            }
        }
    }

    [Serializable]
    public class PlayerStatusData
    {
        public StatusType StatusType;
        public float BaseValue;
        public float CurrentValue;
    }
}
