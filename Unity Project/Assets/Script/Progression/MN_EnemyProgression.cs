using System;
using System.Collections.Generic;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Progression
{
    public class MN_EnemyProgression : SceneSingleton<MN_EnemyProgression>
    {
        #region Fields

        [Serializable]
        public class StatusProgression
        {
            public StatusType StatusType;
            [Min(0f)] public float GrowthPercentPerLevel;
            [SerializeField] private float _currentMultiplierPercent = 100f;

            public float CurrentMultiplierPercent => _currentMultiplierPercent;

            public void UpdateMultiplier(int level)
            {
                _currentMultiplierPercent = 100f + Mathf.Max(0, level - 1) *
                    Mathf.Max(0f, GrowthPercentPerLevel);
            }
        }

        [SerializeField] private List<StatusProgression> _list_StatusProgressions = new List<StatusProgression>
        {
            new StatusProgression { StatusType = StatusType.HEALTH },
            new StatusProgression { StatusType = StatusType.PHYSIC_FORCE },
            new StatusProgression { StatusType = StatusType.MAGIC_FORCE }
        };

        [SerializeField] private int _currentLevel = 1;
        private MN_Xp _xpManager;

        #endregion

        #region Unity

        private void OnEnable()
        {
            if (_xpManager != null)
                _xpManager.OnLevelUp -= Xp_OnLevelUp;

            _xpManager = null;
            RefreshProgression();
        }

        private void Start()
        {
            RefreshProgression();
        }

        private void OnDisable()
        {
            if (_xpManager != null)
                _xpManager.OnLevelUp -= Xp_OnLevelUp;

            _xpManager = null;
        }

        #endregion

        #region Progression

        public float GetMultiplierPercent(StatusType statusType)
        {
            RefreshProgression();

            for (int i = 0; i < _list_StatusProgressions.Count; i++)
            {
                StatusProgression progression = _list_StatusProgressions[i];
                if (progression != null && progression.StatusType == statusType)
                    return progression.CurrentMultiplierPercent;
            }

            return 100f;
        }

        private void RefreshProgression()
        {
            if (_xpManager != MN_Xp.Instance)
            {
                if (_xpManager != null)
                    _xpManager.OnLevelUp -= Xp_OnLevelUp;

                _xpManager = MN_Xp.Instance;

                if (_xpManager != null && isActiveAndEnabled)
                    _xpManager.OnLevelUp += Xp_OnLevelUp;
            }

            UpdateMultipliers(_xpManager != null ? _xpManager.GetCurrentLevel() : 1);
        }

        private void Xp_OnLevelUp(int level)
        {
            UpdateMultipliers(level);
        }

        private void UpdateMultipliers(int level)
        {
            _currentLevel = Mathf.Max(1, level);

            for (int i = 0; i < _list_StatusProgressions.Count; i++)
                _list_StatusProgressions[i]?.UpdateMultiplier(_currentLevel);
        }

        #endregion
    }
}
