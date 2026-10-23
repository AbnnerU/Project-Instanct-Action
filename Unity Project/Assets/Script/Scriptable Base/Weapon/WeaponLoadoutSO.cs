using System.Collections.Generic;
using Scripts.Combat;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scripts.Weapon
{
    [CreateAssetMenu(fileName = "WeaponLoadoutSO", menuName = "Scriptable Objects/Weapon/Weapon Loadout")]
    public class WeaponLoadoutSO : ScriptableObject
    {
        public string ID;
        public string DisplayName;
        public ComboAttackSetSO ComboAttackSet;
        public bool HasPostDashAttack;
        public ComboAttackData PostDashAttack = new ComboAttackData();
        public RuntimeAnimatorController AnimatorController;
        public List<WeaponModelSpawnData> List_WeaponModels = new List<WeaponModelSpawnData>();
    }

    [System.Serializable]
    public class WeaponModelSpawnData
    {
        public AssetReferenceGameObject WeaponPrefabReference;
        public string SocketID;
        public Vector3 LocalPositionOffset;
        public Vector3 LocalEulerAngles;
        public Vector3 LocalScale = Vector3.one;
    }
}
