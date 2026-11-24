using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Combat
{
    [CreateAssetMenu(fileName = "ComboAttackSetSO", menuName = "Scriptable Objects/Combat/Combo Attack Set")]
    public class ComboAttackSetSO : ScriptableObject
    {
        [SerializeField] private List<ComboAttackData> _list_ComboAttackData = new List<ComboAttackData>();

        public IReadOnlyList<ComboAttackData> List_ComboAttackData => _list_ComboAttackData;
    }
}
