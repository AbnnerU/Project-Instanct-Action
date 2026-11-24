using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Progression
{
    public class MN_Requirements : SceneSingleton<MN_Requirements>
    {
        public event Action<string> OnRequirementUnlocked;

        private HashSet<string> _hashSet_UnlockedRequirements = new HashSet<string>();

        public void UnlockRequirement(RequirementSO requirement)
        {
            if (requirement != null)
                UnlockRequirement(requirement.ID);
        }

        public void UnlockRequirement(string requirementID)
        {
            if (string.IsNullOrEmpty(requirementID)) return;

            if (_hashSet_UnlockedRequirements.Add(requirementID))
            {
                Debug.Log($"[MN_Requirements] New requeriment unlocked: {requirementID}");
                OnRequirementUnlocked?.Invoke(requirementID);
            }
        }

        public bool IsRequirementMet(RequirementSO requirement)
        {
            if (requirement == null) return true;
            return _hashSet_UnlockedRequirements.Contains(requirement.ID);
        }

        public bool AreAllRequirementsMet(List<RequirementSO> requirements)
        {
            if (requirements == null || requirements.Count == 0) return true;

            foreach (var req in requirements)
            {
                if (req != null && !_hashSet_UnlockedRequirements.Contains(req.ID))
                    return false;
            }
            return true;
        }


        #region Save/Load
        public List<string> GetUnlockedRequirementsList()
        {
            return new List<string>(_hashSet_UnlockedRequirements);
        }

        public void LoadSavedRequirements(List<string> savedRequirements)
        {
            _hashSet_UnlockedRequirements.Clear();

            if (savedRequirements != null)
            {
                foreach (var id in savedRequirements)
                    _hashSet_UnlockedRequirements.Add(id);
            }

            Debug.Log($"[MN_Requirements] Carregados {_hashSet_UnlockedRequirements.Count} requerimentos salvos.");
        }
        #endregion
    }
}