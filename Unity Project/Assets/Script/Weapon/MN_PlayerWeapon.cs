using CustomInspector;
using Scripts.Animation;
using Scripts.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Scripts.Weapon
{
    public class MN_PlayerWeapon : SceneSingleton<MN_PlayerWeapon>
    {
        #region Events

        public event Action<WeaponLoadoutSO> OnWeaponLoadoutEquipped;

        #endregion

        #region Fields

        [SerializeField] private WeaponSocketRegistry _weaponSocketRegistry;
        [SerializeField] private ComboControllerBase _comboController;
        [SerializeField] private Animator _animator;
        [SerializeField] private CharacterAnimationController _characterAnimationController;
        [SerializeField] private WeaponLoadoutSO _initialLoadout;
        [SerializeField] private bool _equipInitialLoadoutOnStart = true;
        [Button(nameof(EquipWeaponLoadout))]

        private readonly List<AsyncOperationHandle<GameObject>> _list_EquippedModelHandles = new List<AsyncOperationHandle<GameObject>>();
        private RuntimeAnimatorController _baseRuntimeAnimatorController;
        private WeaponLoadoutSO _currentLoadout;

        #endregion

        #region Properties

        public WeaponLoadoutSO CurrentLoadout => _currentLoadout;

        #endregion

        #region Unity

        protected override void Awake()
        {
            base.Awake();
            GetReferences();
            CacheBaseAnimatorController();
        }

        private void Start()
        {
            if (_equipInitialLoadoutOnStart && _initialLoadout != null)
                EquipWeaponLoadout(_initialLoadout);
        }

        private void OnDestroy()
        {
            ReleaseEquippedModels();
        }

        #endregion

        #region Public
        public void EquipWeaponLoadout(WeaponLoadoutSO weaponLoadout)
        {
            if (weaponLoadout == null)
                return;

            _currentLoadout = weaponLoadout;

            ReleaseEquippedModels();
            ApplyComboAttackSet(weaponLoadout);
            ApplyAnimatorController(weaponLoadout);
            SpawnWeaponModels(weaponLoadout);

            OnWeaponLoadoutEquipped?.Invoke(weaponLoadout);
        }

        #endregion

        #region References

        private void GetReferences()
        {
            if (_weaponSocketRegistry == null)
                _weaponSocketRegistry = GetComponentInChildren<WeaponSocketRegistry>();

            if (_comboController == null)
                _comboController = GetComponentInChildren<ComboControllerBase>();

            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();

            if (_characterAnimationController == null)
                _characterAnimationController = GetComponentInChildren<CharacterAnimationController>();
        }

        private void CacheBaseAnimatorController()
        {
            if (_animator == null)
                return;

            RuntimeAnimatorController runtimeAnimatorController = _animator.runtimeAnimatorController;

            if (runtimeAnimatorController is AnimatorOverrideController overrideController)
                _baseRuntimeAnimatorController = overrideController.runtimeAnimatorController;
            else
                _baseRuntimeAnimatorController = runtimeAnimatorController;
        }

        #endregion

        #region Combo

        private void ApplyComboAttackSet(WeaponLoadoutSO weaponLoadout)
        {
            if (_comboController == null)
                return;

            _comboController.SetComboAttackSet(weaponLoadout.ComboAttackSet);
        }

        #endregion

        #region Animations

        private void ApplyAnimatorController(WeaponLoadoutSO weaponLoadout)
        {
            if (_animator == null)
                return;

            _animator.runtimeAnimatorController = weaponLoadout.AnimatorController != null
                ? weaponLoadout.AnimatorController
                : _baseRuntimeAnimatorController;

            _characterAnimationController?.RefreshParameterCache();
        }

        #endregion

        #region Models

        private void SpawnWeaponModels(WeaponLoadoutSO weaponLoadout)
        {
            if (_weaponSocketRegistry == null || weaponLoadout.List_WeaponModels == null)
                return;

            for (int i = 0; i < weaponLoadout.List_WeaponModels.Count; i++)
                SpawnWeaponModel(weaponLoadout.List_WeaponModels[i]);
        }

        private void SpawnWeaponModel(WeaponModelSpawnData weaponModelData)
        {
            if (weaponModelData == null || weaponModelData.WeaponPrefabReference == null || !weaponModelData.WeaponPrefabReference.RuntimeKeyIsValid())
                return;

            if (!_weaponSocketRegistry.TryGetSocket(weaponModelData.SocketID, out Transform socketTransform))
            {
                Debug.LogWarning($"Weapon socket '{weaponModelData.SocketID}' was not found.", this);
                return;
            }

            AsyncOperationHandle<GameObject> loadHandle = weaponModelData.WeaponPrefabReference.InstantiateAsync(socketTransform);
            _list_EquippedModelHandles.Add(loadHandle);
            loadHandle.Completed += handle => WeaponModelLoadHandle_Completed(handle, weaponModelData);
        }

        private void WeaponModelLoadHandle_Completed(AsyncOperationHandle<GameObject> loadHandle, WeaponModelSpawnData weaponModelData)
        {
            if (loadHandle.Status != AsyncOperationStatus.Succeeded || loadHandle.Result == null)
                return;

            Transform weaponTransform = loadHandle.Result.transform;
            weaponTransform.localPosition = weaponModelData.LocalPositionOffset;
            weaponTransform.localEulerAngles = weaponModelData.LocalEulerAngles;
            weaponTransform.localScale = weaponModelData.LocalScale;
        }

        private void ReleaseEquippedModels()
        {
            for (int i = 0; i < _list_EquippedModelHandles.Count; i++)
            {
                if (_list_EquippedModelHandles[i].IsValid())
                    Addressables.ReleaseInstance(_list_EquippedModelHandles[i]);
            }

            _list_EquippedModelHandles.Clear();
        }

        #endregion
    }
}
