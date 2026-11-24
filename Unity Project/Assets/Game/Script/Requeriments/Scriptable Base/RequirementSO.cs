using System;
using UnityEngine;

namespace Scripts.Progression
{
    [CreateAssetMenu(fileName = "RequirementSO", menuName = "Scriptable Objects/Progression/Requirement")]
    public class RequirementSO : ScriptableObject
    {
        public string ID;
        public string DisplayName;
        [TextArea] public string Description;

#if UNITY_EDITOR
        private void CreateID()
        {
            ID = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
