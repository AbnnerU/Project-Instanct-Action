using System;
using System.Collections.Generic;
using Scripts.Progression;
using UnityEngine;

namespace Scripts.Waves
{
    // Apply initial settings before MN_Waves.Start, after all scene Awake calls.
    [DefaultExecutionOrder(-50)]
    public class WaveProgressionByPlayerLevel : MonoBehaviour
    {
        [SerializeField] private MN_Xp _xp;
        [SerializeField] private MN_Waves _waves;
        [SerializeField] private List<WaveProgressionEntry> _list_Progression = new List<WaveProgressionEntry>
        {
            new WaveProgressionEntry()
        };

        private bool _started;

        [Serializable]
        public class WaveProgressionEntry
        {
            [Min(1)] public int Level = 1;
            [Min(0.01f)] public float SpawnInterval = 2f;
            [Min(0)] public int MaxAliveEnemies = 10;
        }

        private void Start()
        {
            _started = true;
            Bind();
        }

        private void OnEnable()
        {
            if (_started) Bind();
        }

        private void OnDisable()
        {
            if (_xp != null) _xp.OnLevelUp -= ApplyLevel;
        }

        private void Bind()
        {
            if (_xp == null) _xp = MN_Xp.Instance;
            if (_waves == null) _waves = MN_Waves.Instance;
            if (_xp == null || _waves == null)
            {
                Debug.LogWarning("[WaveProgressionByPlayerLevel] Missing XP or wave manager.", this);
                return;
            }

            _xp.OnLevelUp += ApplyLevel;
            ApplyLevel(_xp.GetCurrentLevel());
        }

        private void ApplyLevel(int level)
        {
            if (_waves == null) return;

            WaveProgressionEntry selected = null;
            foreach (WaveProgressionEntry entry in _list_Progression)
            {
                if (entry == null || entry.Level > level) continue;
                if (selected == null || entry.Level > selected.Level) selected = entry;
            }

            if (selected == null)
            {
                Debug.LogWarning($"[WaveProgressionByPlayerLevel] No spawn settings for level {level}. Add an initial milestone.", this);
                return;
            }

            _waves.SetSpawnSettings(selected.SpawnInterval, selected.MaxAliveEnemies);
        }

        private void OnValidate()
        {
            var levels = new HashSet<int>();
            foreach (WaveProgressionEntry entry in _list_Progression)
            {
                if (entry != null && !levels.Add(entry.Level))
                    Debug.LogWarning($"[WaveProgressionByPlayerLevel] Duplicate level {entry.Level}; the first entry will be used.", this);
            }
        }
    }
}
