using System;
using UnityEngine;

namespace Scripts.Progression
{
    public class MN_Xp : SceneSingleton<MN_Xp>
    {
        public event Action<float, float> OnXpUpdated;
        public event Action<int> OnLevelUp;

        [SerializeField] private int _initialLevel = 1;
        [SerializeField] private int _currentLevel;
        [SerializeField] private int _currentXp;
        [SerializeField] private int _xpToNextLevel = 100;
        [SerializeField] private int _xpIncreasePerLevel = 50;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this) return;

            Initialize();
        }

        private void Start()
        {
            NotifyXpUpdated();
        }

        public int GetCurrentLevel()
        {
            return _currentLevel;
        }

        public int GetCurrentXp()
        {
            return _currentXp;
        }

        public int GetXpToNextLevel()
        {
            return _xpToNextLevel;
        }

        public float GetCurrentXpPercentage()
        {
            if (_xpToNextLevel <= 0) return 1f;

            return Mathf.Clamp01((float)_currentXp / _xpToNextLevel);
        }

        public void AddXp(int xpValue)
        {
            if (xpValue <= 0) return;

            _currentXp += xpValue;

            while (_xpToNextLevel > 0 && _currentXp >= _xpToNextLevel)
                LevelUp();

            NotifyXpUpdated();
        }

        private void Initialize()
        {
            _currentLevel = Mathf.Max(1, _initialLevel);
            _currentXp = Mathf.Max(0, _currentXp);
            _xpToNextLevel = Mathf.Max(1, _xpToNextLevel);
            _xpIncreasePerLevel = Mathf.Max(0, _xpIncreasePerLevel);
        }

        private void LevelUp()
        {
            _currentXp = 0;
            _currentLevel++;
            _xpToNextLevel += _xpIncreasePerLevel;

            OnLevelUp?.Invoke(_currentLevel);
        }

        private void NotifyXpUpdated()
        {
            OnXpUpdated?.Invoke(GetCurrentXpPercentage(), _currentXp);
        }
    }
}
