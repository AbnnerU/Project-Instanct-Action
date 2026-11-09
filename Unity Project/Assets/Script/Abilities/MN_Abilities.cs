using System;
using System.Collections.Generic;
using Scripts.Combat;
using Scripts.Enchantment;
using Scripts.Managers;
using Scripts.DashHandler;
using Scripts.Status;
using Scripts.Timer;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Scripts.Abilities
{
    public class MN_Abilities : SceneSingleton<MN_Abilities>
    {     

        #region Fields

        [SerializeField] private GameObject _ownerObject;
        [SerializeField] private Transform _passiveAbilitiesContainer;
        [SerializeField] private Transform _activeAbilitiesContainer;
        [SerializeField] private MN_PlayerStatus _playerStatus;
        [SerializeField] private ComboControllerBase _comboController;
        [SerializeField] private AttackHitbox _attackHitbox;
        [SerializeField] private DashHandler.DashBase _playerDash;
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private List<ActiveAbilitySlot> _list_ActiveAbilitySlots = new List<ActiveAbilitySlot>();

        private readonly List<AbilityBase> _list_PassiveAbilities = new List<AbilityBase>();
        private readonly List<AbilityBase> _list_ActiveAbilities = new List<AbilityBase>();
        private readonly List<AsyncOperationHandle<GameObject>> _list_AbilityHandles = new List<AsyncOperationHandle<GameObject>>();
        private bool _subscribedToDashEvents;
        private bool _isDashBlockingAbilitySlots;
        private bool _isLoadingAbility;
        private bool _isDestroying;
        private readonly Dictionary<AbilityBase, AbilityInstance> _dcInstances = new Dictionary<AbilityBase, AbilityInstance>();
        private readonly HashSet<AbilityBase> _hashSet_ExecutingAbilities = new HashSet<AbilityBase>();

        private class AbilityInstance
        {
            public string Key;
            public AsyncOperationHandle<GameObject> Handle;
            public AbilityBase[] Abilities;
        }

        #endregion

        #region Properties

        public IReadOnlyList<AbilityBase> List_PassiveAbilities => _list_PassiveAbilities;
        public IReadOnlyList<AbilityBase> List_ActiveAbilities => _list_ActiveAbilities;
        public IReadOnlyList<ActiveAbilitySlot> List_ActiveAbilitySlots => _list_ActiveAbilitySlots;
        //public event Action<int, ActiveAbilitySlot> OnActiveAbilitySlotUpdated;

        #endregion

        #region Unity

        protected override void Awake()
        {
            base.Awake();
            GetReferences();
        }

        private void OnEnable()
        {
            SubscribeDashEvents();
            EnableSlotInputs();
        }

        private void OnDisable()
        {
            UnsubscribeDashEvents();
            DisableSlotInputs();
        }

        private void OnDestroy()
        {
            _isDestroying = true;
            ReleaseAbilities();
            DisableSlotInputs();
        }

        #endregion

        #region Public

        public void AddAbility(AssetReferenceGameObject abilityPrefabReference, AbilityKind abilityKind, IReadOnlyList<DamageData> list_DamageDatas, IReadOnlyList<BuffData> list_BuffDatas)
        {
            AddAbility(abilityPrefabReference, abilityKind, list_DamageDatas, list_BuffDatas, null);
        }

        public void AddAbility(AssetReferenceGameObject abilityPrefabReference, AbilityKind abilityKind, IReadOnlyList<DamageData> list_DamageDatas, IReadOnlyList<BuffData> list_BuffDatas, AssetReferenceSprite iconReference)
        {
            int slotIndex = GetSlotIndex(GetFirstEmptySlot());
            BeginAddAbility(abilityPrefabReference, abilityKind, list_DamageDatas, list_BuffDatas, iconReference,
                slotIndex >= 0 ? slotIndex : 0, null);
        }

        public bool HasAbility(AssetReferenceGameObject prefab)
        {
            if (prefab == null || !prefab.RuntimeKeyIsValid()) return false;
            string key = prefab.RuntimeKey.ToString();
            foreach (AbilityInstance instance in _dcInstances.Values)
                if (instance.Key == key) return true;
            return false;
        }

        public bool TryAddAbility(AbilityEnchantmentSO enchantment, int slotIndex, Action<bool> onCompleted)
        {
            if (enchantment == null) return false;
            return BeginAddAbility(enchantment.AbilityPrefabReference, enchantment.AbilityKind,
                enchantment.List_DamageDatas, enchantment.List_BuffDatas, enchantment.IconReference, slotIndex, onCompleted);
        }

        private bool BeginAddAbility(AssetReferenceGameObject prefab, AbilityKind kind,
            IReadOnlyList<DamageData> damage, IReadOnlyList<BuffData> buffs, AssetReferenceSprite icon,
            int slotIndex, Action<bool> onCompleted)
        {
            if (_isDestroying || _isLoadingAbility || prefab == null || !prefab.RuntimeKeyIsValid() || HasAbility(prefab)) return false;
            if (kind == AbilityKind.ACTIVE && (slotIndex < 0 || slotIndex >= _list_ActiveAbilitySlots.Count || _list_ActiveAbilitySlots[slotIndex] == null)) return false;
            _isLoadingAbility = true;
            try
            {
                var handle = Addressables.InstantiateAsync(prefab.RuntimeKey, GetAbilityContainer(kind));
                _list_AbilityHandles.Add(handle);
                handle.Completed += result => CompleteAbilityLoad(result, prefab.RuntimeKey.ToString(), kind, damage, buffs, icon, slotIndex, onCompleted);
                return true;
            }
            catch (Exception exception)
            {
                _isLoadingAbility = false;
                Debug.LogException(exception, this);
                return false;
            }
        }

        private void CompleteAbilityLoad(AsyncOperationHandle<GameObject> handle, string key, AbilityKind kind,
            IReadOnlyList<DamageData> damage, IReadOnlyList<BuffData> buffs, AssetReferenceSprite icon,
            int slotIndex, Action<bool> onCompleted)
        {
            if (this == null || _isDestroying) return;
            AbilityBase[] abilities = null;
            bool succeeded = false;
            try
            {
                if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
                    throw new InvalidOperationException("Ability prefab could not be loaded.");
                abilities = handle.Result.GetComponentsInChildren<AbilityBase>(true);
                if (abilities.Length == 0 || (kind == AbilityKind.ACTIVE && abilities.Length != 1))
                    throw new InvalidOperationException("An active ability prefab must contain exactly one AbilityBase.");
                AbilityContext context = CreateContext(damage, buffs);
                foreach (AbilityAutoSetupBase setup in handle.Result.GetComponentsInChildren<AbilityAutoSetupBase>(true))
                    setup.Setup(context);
                foreach (AbilityBase ability in abilities) ability.Initialize();

                // Commit only after loading and setup have succeeded.
                if (kind == AbilityKind.ACTIVE) RemoveAbility(_list_ActiveAbilitySlots[slotIndex].Ability);
                var instance = new AbilityInstance { Key = key, Handle = handle, Abilities = abilities };
                foreach (AbilityBase ability in abilities)
                {
                    _dcInstances.Add(ability, instance);
                    if (kind == AbilityKind.ACTIVE)
                    {
                        _list_ActiveAbilities.Add(ability);
                        SubscribeAbilityEvents(ability);
                    }
                    else _list_PassiveAbilities.Add(ability);
                }
                if (kind == AbilityKind.ACTIVE)
                {
                    ActiveAbilitySlot slot = _list_ActiveAbilitySlots[slotIndex];

                    slot.SetAbility(abilities[0], icon);

                    if (_isDashBlockingAbilitySlots) slot.SetBlocked(true);

                    foreach (AbilityBase executing in _hashSet_ExecutingAbilities)
                        if (executing != abilities[0]) slot.SetBlocked(true);

                    slot.AbilitySlotUpdated();
                }
                succeeded = true;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                if (abilities != null)
                    foreach (AbilityBase ability in abilities)
                        if (ability != null) ability.Deactivate();
                _list_AbilityHandles.Remove(handle);
                if (handle.IsValid())
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded) Addressables.ReleaseInstance(handle);
                    else Addressables.Release(handle);
                }
            }
            _isLoadingAbility = false;
            onCompleted?.Invoke(succeeded);
        }

        public void RemoveAbility(AbilityBase ability)
        {
            if (ability == null || !_dcInstances.TryGetValue(ability, out AbilityInstance instance)) return;
            foreach (AbilityBase component in instance.Abilities) _dcInstances.Remove(component);
            foreach (AbilityBase component in instance.Abilities)
            {
                if (component == null) continue;
                component.Deactivate();
                UnsubscribeAbilityEvents(component);
                ActiveAbilitySlot slot = GetSlotByAbility(component);
                if (slot != null)
                {
                    slot.SetAbility(null);
                    slot.AbilitySlotUpdated();
                }
                _list_ActiveAbilities.Remove(component);
                _list_PassiveAbilities.Remove(component);
                _dcInstances.Remove(component);
            }
            _list_AbilityHandles.Remove(instance.Handle);
            if (instance.Handle.IsValid()) Addressables.ReleaseInstance(instance.Handle);
        }

        public bool TryExecuteActiveAbility(int activeAbilityIndex)
        {
            if (activeAbilityIndex < 0 || activeAbilityIndex >= _list_ActiveAbilities.Count)
                return false;

            AbilityBase ability = _list_ActiveAbilities[activeAbilityIndex];

            if (ability == null || !ability.Initialized)
                return false;

            ActiveAbilitySlot slot = GetSlotByAbility(ability);

            if (slot != null)
                return slot.TryExecuteAbility();

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
                _playerDash = _ownerObject.GetComponentInChildren<Scripts.DashHandler.DashBase>();

            if (_statusSource == null)
                _statusSource = _ownerObject.GetComponentInChildren<StatusInfoSourceBase>();
        }

        private AbilityContext CreateContext(IReadOnlyList<DamageData> list_DamageDatas, IReadOnlyList<BuffData> list_BuffDatas)
        {
            GetReferences();

            return new AbilityContext(
                _ownerObject,
                _playerStatus,
                _comboController,
                _attackHitbox,
                _playerDash,
                _statusSource,
                list_DamageDatas,
                list_BuffDatas);
        }

        private Transform GetAbilityContainer(AbilityKind abilityKind)
        {
            if (abilityKind == AbilityKind.ACTIVE)
                return _activeAbilitiesContainer != null ? _activeAbilitiesContainer : transform;

            return _passiveAbilitiesContainer != null ? _passiveAbilitiesContainer : transform;
        }

        #endregion

        #region Spawn

        private void ReleaseAbilities()
        {
            foreach (AbilityBase ability in new List<AbilityBase>(_dcInstances.Keys)) RemoveAbility(ability);
            for (int i = 0; i < _list_ActiveAbilities.Count; i++)
                UnsubscribeAbilityEvents(_list_ActiveAbilities[i]);

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

        private ActiveAbilitySlot GetFirstEmptySlot()
        {
            for (int i = 0; i < _list_ActiveAbilitySlots.Count; i++)
            {
                if (_list_ActiveAbilitySlots[i] != null && _list_ActiveAbilitySlots[i].IsEmpty)
                    return _list_ActiveAbilitySlots[i];
            }

            return null;
        }

        private int GetSlotIndex(ActiveAbilitySlot slot)
        {
            if (slot == null || _list_ActiveAbilitySlots == null)
                return -1;

            for (int i = 0; i < _list_ActiveAbilitySlots.Count; i++)
            {
                if (_list_ActiveAbilitySlots[i] == slot)
                    return i;
            }

            return -1;
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

        private void SubscribeDashEvents()
        {
            GetReferences();

            if (_subscribedToDashEvents || _playerDash == null)
                return;

            _playerDash.OnDashStarted += PlayerDash_OnDashStarted;
            _playerDash.OnDashFinished += PlayerDash_OnDashFinished;
            _subscribedToDashEvents = true;
        }

        private void UnsubscribeDashEvents()
        {
            if (!_subscribedToDashEvents || _playerDash == null)
                return;

            _playerDash.OnDashStarted -= PlayerDash_OnDashStarted;
            _playerDash.OnDashFinished -= PlayerDash_OnDashFinished;
            _subscribedToDashEvents = false;
        }

        private void PlayerDash_OnDashStarted(float directionDot)
        {
            _isDashBlockingAbilitySlots = true;
            SetAllSlotsBlocked(true);
        }

        private void PlayerDash_OnDashFinished(float directionDot)
        {
            _isDashBlockingAbilitySlots = false;
            SetAllSlotsBlocked(false);
        }

        private void SubscribeAbilityEvents(AbilityBase ability)
        {
            if (ability == null)
                return;

            ability.OnAbilityStarted -= Ability_OnAbilityStarted;
            ability.OnAbilityFinished -= Ability_OnAbilityFinished;

            ability.OnAbilityStarted += Ability_OnAbilityStarted;
            ability.OnAbilityFinished += Ability_OnAbilityFinished;
        }

        private void UnsubscribeAbilityEvents(AbilityBase ability)
        {
            if (ability == null)
                return;

            ability.OnAbilityStarted -= Ability_OnAbilityStarted;
            ability.OnAbilityFinished -= Ability_OnAbilityFinished;
        }

        private void Ability_OnAbilityStarted(AbilityBase ability)
        {
            ActiveAbilitySlot sourceSlot = GetSlotByAbility(ability);

            if (sourceSlot == null || !_hashSet_ExecutingAbilities.Add(ability))
                return;

            SetSlotsBlockedExcept(sourceSlot, true);
        }

        private void Ability_OnAbilityFinished(AbilityBase ability)
        {
            ActiveAbilitySlot sourceSlot = GetSlotByAbility(ability);

            if (sourceSlot == null || !_hashSet_ExecutingAbilities.Remove(ability))
                return;

            SetSlotsBlockedExcept(sourceSlot, false);
        }

        private ActiveAbilitySlot GetSlotByAbility(AbilityBase ability)
        {
            if (ability == null || _list_ActiveAbilitySlots == null)
                return null;

            for (int i = 0; i < _list_ActiveAbilitySlots.Count; i++)
            {
                if (_list_ActiveAbilitySlots[i] != null && _list_ActiveAbilitySlots[i].Ability == ability)
                    return _list_ActiveAbilitySlots[i];
            }

            return null;
        }

        private void SetSlotsBlockedExcept(ActiveAbilitySlot sourceSlot, bool blocked)
        {
            if (_list_ActiveAbilitySlots == null)
                return;

            for (int i = 0; i < _list_ActiveAbilitySlots.Count; i++)
            {
                ActiveAbilitySlot slot = _list_ActiveAbilitySlots[i];

                if (slot == null || slot == sourceSlot)
                    continue;

                slot.SetBlocked(blocked);
            }
        }

        private void SetAllSlotsBlocked(bool blocked)
        {
            if (_list_ActiveAbilitySlots == null)
                return;

            for (int i = 0; i < _list_ActiveAbilitySlots.Count; i++)
                _list_ActiveAbilitySlots[i]?.SetBlocked(blocked);
        }

        #endregion

        #region Classes

        [Serializable]
        public class ActiveAbilitySlot
        {
            public Action<float> OnAbilityCooldownStarted;
            public Action<float> OnAbilityCooldownCompleted;
            public Action<AbilitySlotState> OnAbilitySlotStateUpdated;
            public Action<ActiveAbilitySlot> OnAbilityUpdated;

            [SerializeField] private AbilitySlotState _slotState;
            [SerializeField] private AbilitySlotView _slotView;
            [SerializeField] private InputActionReference _inputAction;
            [SerializeField] private AbilityBase _ability;

            private Action<InputAction.CallbackContext> _onPerformedCallback;
            private int _blockCount;
            private bool _isInCooldown;

            public AbilityBase Ability => _ability;
            public AssetReferenceSprite IconReference { get; private set; }
            public Sprite IconSprite => !IsEmpty && _slotView != null ? _slotView.IconSprite : null;
            public bool IsEmpty => _ability == null;
            public AbilitySlotState SlotState => _slotState;
            public IHasCooldown CooldownHandler => _cooldownHandler;

            public IHasCooldown _cooldownHandler;

            public enum AbilitySlotState
            {
                NONE,
                ABILITY_IN_COOLDOWN,
                BLOCKED
            }

            public void SetAbility(AbilityBase ability)
            {
                SetAbility(ability, null);
            }

            public void SetAbility(AbilityBase ability, AssetReferenceSprite iconReference)
            {
                if (_cooldownHandler!=null)
                {
                    _cooldownHandler.OnCooldownStarted -= StartedCooldown;
                    _cooldownHandler.OnCooldownCompleted -= CompletedCooldown;
                }

                _ability = ability;
                IconReference = iconReference;
                _cooldownHandler = null;
                _blockCount = 0;
                _isInCooldown = false;
                UpdateSlotState();

                if (_ability == null)
                {
                    OnAbilityUpdated?.Invoke(this);
                    return;
                }

                if(_ability.gameObject.TryGetComponent<IHasCooldown>(out _cooldownHandler))
                {
                    _cooldownHandler.OnCooldownStarted += StartedCooldown;
                    _cooldownHandler.OnCooldownCompleted += CompletedCooldown;
                }

                OnAbilityUpdated?.Invoke(this);
            }

            public void SetBlocked(bool blocked)
            {
                int previousBlockCount = _blockCount;

                if (blocked)
                    _blockCount++;
                else
                    _blockCount = Mathf.Max(0, _blockCount - 1);

                if (previousBlockCount == _blockCount)
                    return;

                UpdateSlotState();
            }

            public bool TryExecuteAbility()
            {
                if (_blockCount > 0)
                    return false;

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

                _onPerformedCallback = _ => onPerformed?.Invoke(this);
                _inputAction.action.Enable();
                _inputAction.action.performed += _onPerformedCallback;
            }

            public void DisableInput()
            {
                if (_inputAction == null || _inputAction.action == null || _onPerformedCallback == null)
                    return;

                _inputAction.action.performed -= _onPerformedCallback;
                _onPerformedCallback = null;
            }

            public void StartedCooldown(float cooldownValue)
            {
                _isInCooldown = true;
                UpdateSlotState();
                OnAbilityCooldownStarted?.Invoke(cooldownValue);
            }

            public void CompletedCooldown(float cooldownValue)
            {
                _isInCooldown = false;
                UpdateSlotState();
                OnAbilityCooldownCompleted?.Invoke(cooldownValue);
            }

            private void UpdateSlotState()
            {
                AbilitySlotState newSlotState = GetSlotState();

                if (_slotState == newSlotState)
                    return;

                _slotState = newSlotState;
                OnAbilitySlotStateUpdated?.Invoke(_slotState);
            }

            private AbilitySlotState GetSlotState()
            {
                if (_blockCount > 0)
                    return AbilitySlotState.BLOCKED;

                if (_isInCooldown)
                    return AbilitySlotState.ABILITY_IN_COOLDOWN;

                return AbilitySlotState.NONE;
            }

            public void AbilitySlotUpdated()
            {
                if (_slotView == null)
                    return;

                _slotView.Setup(this);
            }
        }

        #endregion
    }
}
