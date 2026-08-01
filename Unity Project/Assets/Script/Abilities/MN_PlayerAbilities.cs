using System;
using System.Collections.Generic;
using Scripts.Combat;
using Scripts.Managers;
using Scripts.Player;
using Scripts.Status;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Scripts.Abilities
{
    public class MN_PlayerAbilities : SceneSingleton<MN_PlayerAbilities>
    {
       

        #region Fields

        [SerializeField] private GameObject _ownerObject;
        [SerializeField] private Transform _passiveAbilitiesContainer;
        [SerializeField] private Transform _activeAbilitiesContainer;
        [SerializeField] private MN_PlayerStatus _playerStatus;
        [SerializeField] private ComboControllerBase _comboController;
        [SerializeField] private AttackHitbox _attackHitbox;
        [SerializeField] private PlayerDash _playerDash;
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private List<ActiveAbilitySlot> _list_ActiveAbilitySlots = new List<ActiveAbilitySlot>();

        private readonly List<AbilityBase> _list_PassiveAbilities = new List<AbilityBase>();
        private readonly List<AbilityBase> _list_ActiveAbilities = new List<AbilityBase>();
        private readonly List<AsyncOperationHandle<GameObject>> _list_AbilityHandles = new List<AsyncOperationHandle<GameObject>>();

        #endregion

        #region Properties

        public IReadOnlyList<AbilityBase> List_PassiveAbilities => _list_PassiveAbilities;
        public IReadOnlyList<AbilityBase> List_ActiveAbilities => _list_ActiveAbilities;

        #endregion

        #region Unity

        protected override void Awake()
        {
            base.Awake();
            GetReferences();
        }

        private void OnEnable()
        {
            EnableSlotInputs();
        }

        private void OnDisable()
        {
            DisableSlotInputs();
        }

        private void OnDestroy()
        {
            ReleaseAbilities();
        }

        #endregion

        #region Public

        public void AddAbility(AssetReferenceGameObject abilityPrefabReference, AbilityKind abilityKind)
        {
            if (abilityPrefabReference == null || !abilityPrefabReference.RuntimeKeyIsValid())
                return;

            Transform parent = GetAbilityContainer(abilityKind);
            AsyncOperationHandle<GameObject> loadHandle = abilityPrefabReference.InstantiateAsync(parent);

            _list_AbilityHandles.Add(loadHandle);
            loadHandle.Completed += handle => AbilityLoadHandle_Completed(handle, abilityKind);
        }

        public bool TryExecuteActiveAbility(int activeAbilityIndex)
        {
            if (activeAbilityIndex < 0 || activeAbilityIndex >= _list_ActiveAbilities.Count)
                return false;

            AbilityBase ability = _list_ActiveAbilities[activeAbilityIndex];

            if (ability == null || !ability.Initialized)
                return false;

            ability.Execute();
            return true;
        }

        public void CancelActiveAbilities()
        {
            for (int i = 0; i < _list_ActiveAbilities.Count; i++)
                _list_ActiveAbilities[i]?.Cancel();
        }

        #endregion

        #region Setup

        private void GetReferences()
        {
            if (_ownerObject == null)
                _ownerObject = gameObject;

            if (_playerStatus == null)
                _playerStatus = MN_PlayerStatus.Instance;

            if (_comboController == null)
                _comboController = _ownerObject.GetComponentInChildren<ComboControllerBase>();

            if (_attackHitbox == null)
                _attackHitbox = _ownerObject.GetComponentInChildren<AttackHitbox>();

            if (_playerDash == null)
                _playerDash = _ownerObject.GetComponentInChildren<PlayerDash>();

            if (_statusSource == null)
                _statusSource = _ownerObject.GetComponentInChildren<StatusInfoSourceBase>();
        }

        private AbilityContext CreateContext()
        {
            GetReferences();

            return new AbilityContext(
                _ownerObject,
                _playerStatus,
                _comboController,
                _attackHitbox,
                _playerDash,
                _statusSource);
        }

        private Transform GetAbilityContainer(AbilityKind abilityKind)
        {
            if (abilityKind == AbilityKind.ACTIVE)
                return _activeAbilitiesContainer != null ? _activeAbilitiesContainer : transform;

            return _passiveAbilitiesContainer != null ? _passiveAbilitiesContainer : transform;
        }

        #endregion

        #region Spawn

        private void AbilityLoadHandle_Completed(AsyncOperationHandle<GameObject> loadHandle, AbilityKind abilityKind)
        {
            if (loadHandle.Status != AsyncOperationStatus.Succeeded || loadHandle.Result == null)
                return;

            SetupAbilityInstance(loadHandle.Result, abilityKind);
        }

        private void SetupAbilityInstance(GameObject abilityInstance, AbilityKind abilityKind)
        {
            AbilityContext context = CreateContext();
            AbilityAutoSetupBase[] autoSetups = abilityInstance.GetComponentsInChildren<AbilityAutoSetupBase>(true);

            for (int i = 0; i < autoSetups.Length; i++)
                autoSetups[i].Setup(context);

            AbilityBase[] abilities = abilityInstance.GetComponentsInChildren<AbilityBase>(true);

            for (int i = 0; i < abilities.Length; i++)
            {
                abilities[i].Initialize();
                AddToList(abilities[i], abilityKind);
            }
        }

        private void AddToList(AbilityBase ability, AbilityKind abilityKind)
        {
            if (ability == null)
                return;

            if (abilityKind == AbilityKind.ACTIVE)
            {
                _list_ActiveAbilities.Add(ability);
                AssignAbilityToSlot(ability);
            }
            else
            {
                _list_PassiveAbilities.Add(ability);
            }
        }

        private void ReleaseAbilities()
        {
            for (int i = 0; i < _list_AbilityHandles.Count; i++)
            {
                if (_list_AbilityHandles[i].IsValid())
                    Addressables.ReleaseInstance(_list_AbilityHandles[i]);
            }

            _list_AbilityHandles.Clear();
            _list_PassiveAbilities.Clear();
            _list_ActiveAbilities.Clear();
        }

        #endregion

        #region Active Slots

        private void AssignAbilityToSlot(AbilityBase ability)
        {
            if (ability == null || _list_ActiveAbilitySlots == null || _list_ActiveAbilitySlots.Count == 0)
                return;

            ActiveAbilitySlot slot = GetFirstEmptySlot();

            if (slot == null)
                slot = _list_ActiveAbilitySlots[0];

            slot?.SetAbility(ability);
        }

        private ActiveAbilitySlot GetFirstEmptySlot()
        {
            for (int i = 0; i < _list_ActiveAbilitySlots.Count; i++)
            {
                if (_list_ActiveAbilitySlots[i] != null && _list_ActiveAbilitySlots[i].IsEmpty)
                    return _list_ActiveAbilitySlots[i];
            }

            return null;
        }

        private void EnableSlotInputs()
        {
            if (_list_ActiveAbilitySlots == null)
                return;

            for (int i = 0; i < _list_ActiveAbilitySlots.Count; i++)
                _list_ActiveAbilitySlots[i]?.EnableInput(SlotInput_Performed);
        }

        private void DisableSlotInputs()
        {
            if (_list_ActiveAbilitySlots == null)
                return;

            for (int i = 0; i < _list_ActiveAbilitySlots.Count; i++)
                _list_ActiveAbilitySlots[i]?.DisableInput();
        }

        private void SlotInput_Performed(ActiveAbilitySlot slot)
        {
            slot?.TryExecuteAbility();
        }

        #endregion

        #region Classes

        [Serializable]
        public class ActiveAbilitySlot
        {
            [SerializeField] private InputActionReference _inputAction;
            [SerializeField] private AbilityBase _ability;

            private Action<InputAction.CallbackContext> _performedCallback;

            public AbilityBase Ability => _ability;
            public bool IsEmpty => _ability == null;

            public void SetAbility(AbilityBase ability)
            {
                _ability = ability;
            }

            public bool TryExecuteAbility()
            {
                if (_ability == null || !_ability.Initialized)
                    return false;

                _ability.Execute();
                return true;
            }

            public void EnableInput(Action<ActiveAbilitySlot> onPerformed)
            {
                if (_inputAction == null || _inputAction.action == null)
                    return;

                DisableInput();

                _performedCallback = _ => onPerformed?.Invoke(this);
                _inputAction.action.Enable();
                _inputAction.action.performed += _performedCallback;
            }

            public void DisableInput()
            {
                if (_inputAction == null || _inputAction.action == null || _performedCallback == null)
                    return;

                _inputAction.action.performed -= _performedCallback;
                _performedCallback = null;
            }
        }

        #endregion
    }
}
