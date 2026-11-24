using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Status
{
    [Serializable]
    public class StatusInfo
    {
        public List<StatusData> List_StatusData = new List<StatusData>();

        public StatusData GetStatusData(StatusType statusType)
        {
            return List_StatusData.Find(x => x.StatusType == statusType);
        }

        public float GetCurrentValueOf(StatusType statusType)
        {
            StatusData statusData = List_StatusData.Find(x => x.StatusType == statusType);

            if (statusData != null)
                return statusData.CurrentValue;
            else
                return 0;
        }

        public float GetBaseValueOf(StatusType statusType)
        {
            StatusData statusData = List_StatusData.Find(x => x.StatusType == statusType);

            if (statusData != null)
                return statusData.BaseValue;
            else
                return 0;
        }

        public bool HasStatusType(StatusType statusType)
        {
            return List_StatusData.Find(x => x.StatusType == statusType) != null ? true : false;
        }

        public void Initialize()
        {
            foreach (StatusData statusData in List_StatusData)
                statusData.CurrentValue = statusData.BaseValue;
        }
    }

    [Serializable]
    public class StatusData
    {
        public Action<float> OnCurrentValueUpdated;

        public StatusType StatusType;
        [Tooltip("Percentage stats use 100 = 100%. Attack speed 100 = normal; critical damage 125 = 1.25x total damage. Health and force use absolute units.")]
        public float BaseValue;
        [Tooltip("Uses the same units as BaseValue, including percentage points for percentage stats.")]
        public float CurrentValue;

        public void AddCurrentValue(float valueToAdd)
        {
            CurrentValue += valueToAdd;
            OnCurrentValueUpdated?.Invoke(CurrentValue);
        }

        public void RemoveCurrentValue(float valueToRemove)
        {
            CurrentValue -= valueToRemove;
            OnCurrentValueUpdated?.Invoke(CurrentValue);
        }

        public void ResetCurrentValue()
        {
            CurrentValue = BaseValue;
            OnCurrentValueUpdated?.Invoke(CurrentValue);
        }
    }
}
