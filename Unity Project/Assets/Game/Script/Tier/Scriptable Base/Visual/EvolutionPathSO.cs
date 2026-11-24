using System;
using System.Collections.Generic;
using UnityEngine;


namespace Scripts.TierManipulation.Visual
{
    [CreateAssetMenu(fileName = "EvolutionPathSO", menuName = "Scriptable Objects/Evolution/EvolutionPathSO")]
    public class EvolutionPathSO : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        public CreatureEvolutionDataSO StartingEvolution;
        public List<EvolutionConnection> List_EvolutionConnections = new List<EvolutionConnection>();

        [System.Serializable]
        public class EvolutionConnection
        {
            //public CreatureEvolutionDataSO fromEvolution;
            public Tier TierReference;
            public CreatureEvolutionDataSO NewModelEvolution;
        }

        public EvolutionConnection TryFindEvolutionByTier(Tier tier)
        {
            return List_EvolutionConnections.Find(x => x.TierReference == tier);
        }

#if UNITY_EDITOR
        private void CreateID()
        {
            Guid guid = Guid.NewGuid();
            Id = guid.ToString();

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}