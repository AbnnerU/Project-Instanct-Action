using System;
using System.Collections.Generic;

namespace Scripts.Status
{
    [Serializable]
    public class StatusInfo 
    {
        public List<StatusData> List_StatusData = new List<StatusData>();

        public float GetCurrentValueOf(StatusType statusType)
        {
            StatusData statusData = List_StatusData.Find(x=> x.StatusType == statusType);

            if(statusData!=null)
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


        public void Initialize()
        {
            foreach(StatusData statusData in List_StatusData) 
                statusData.CurrentValue = statusData.BaseValue;
        }
    }

    [Serializable]
    public class StatusData
    {
        public Action<float> OnCurrentValueUpdated;

        public StatusType StatusType;
        public float BaseValue;
        public float CurrentValue;
    }
}