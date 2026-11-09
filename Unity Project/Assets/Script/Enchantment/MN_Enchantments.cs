using System;
using System.Collections.Generic;
using Scripts.Abilities;
using Scripts.Managers;
using Scripts.Progression;
using UnityEngine;

namespace Scripts.Enchantment
{
    public class MN_Enchantments : SceneSingleton<MN_Enchantments>
    {
        public event Action<EnchantmentSOBase> OnEnchantmentApplied;
        public event Action<IReadOnlyList<EnchantmentSOBase>> OnEnchantmentsOffered;
        public event Action<EnchantmentSelectionState> OnSelectionStateChanged;

        private EnchantmentSelectionState _state = EnchantmentSelectionState.NONE;
        [SerializeField] private GameObject _playerObject;
        [SerializeField] private bool _offerEnchantmentsOnLevelUp = true;
        [SerializeField, Min(1)] private int _optionsPerLevelUp = 3;
        [SerializeField] private EnchantmentsDeckConfig _defaultDeckConfig = new EnchantmentsDeckConfig();
        [SerializeField] private EnchantmentOfferBuilderSOBase _deckBuildRules;
        [SerializeField, Min(0)] private int _skipAdditionalRolls = 1;
        [SerializeField] private List<EnchantmentSOBase> _list_AvailableEnchantments = new List<EnchantmentSOBase>();
        [SerializeField] private List<EnchantmentSOBase> _list_AppliedEnchantments = new List<EnchantmentSOBase>();

        private readonly List<EnchantmentSOBase> _list_CurrentOfferedEnchantments = new List<EnchantmentSOBase>();
        private readonly Queue<EnchantmentsDeckConfig> _queueOffers = new Queue<EnchantmentsDeckConfig>();
        private EnchantmentsDeckConfig _currentDeckConfig;
        private AbilityEnchantmentSO _pendingAbility;

        public IReadOnlyList<EnchantmentSOBase> List_AvailableEnchantments => _list_AvailableEnchantments;
        public IReadOnlyList<EnchantmentSOBase> List_AppliedEnchantments => _list_AppliedEnchantments;
        public IReadOnlyList<EnchantmentSOBase> List_CurrentOfferedEnchantments => _list_CurrentOfferedEnchantments;
        public EnchantmentSelectionState CurrentState => _state;
        public bool CanSkip => _state == EnchantmentSelectionState.ENCHANTMENTS_OFFERED &&
            _currentDeckConfig != null && _currentDeckConfig.AllowSkip;
        public int CurrentOfferRolls { get; private set; }


        private void Start()
        {
            if (_offerEnchantmentsOnLevelUp && MN_Xp.Instance != null) MN_Xp.Instance.OnLevelUp += Xp_OnLevelUp;
        }

        private void OnDestroy()
        {
            if (MN_Xp.Instance != null) MN_Xp.Instance.OnLevelUp -= Xp_OnLevelUp;
        }

        private void Update()
        {
            // Wait until the previous HUD has finished closing before showing the next reward.
            if (_state == EnchantmentSelectionState.NONE && _queueOffers.Count > 0) TryOfferEnchantments(_queueOffers.Dequeue());
        }

        public void OfferEnchantments() => OfferEnchantments(_defaultDeckConfig);

        public void OfferEnchantments(EnchantmentsDeckConfig config)
        {
            if (config == null) return;
            if (_state != EnchantmentSelectionState.NONE) _queueOffers.Enqueue(config);
            else TryOfferEnchantments(config);
        }

        public bool TryOfferEnchantments(EnchantmentsDeckConfig config)
        {
            if (config == null || _state != EnchantmentSelectionState.NONE) return false;
            BuildOfferedEnchantments(config);
            if (_list_CurrentOfferedEnchantments.Count == 0) return false;

            _currentDeckConfig = config;
            CurrentOfferRolls = config.GetRollsAmount();
            SetState(EnchantmentSelectionState.ENCHANTMENTS_OFFERED);
            OnEnchantmentsOffered?.Invoke(_list_CurrentOfferedEnchantments.AsReadOnly());
            return true;
        }

        public void ApplyEnchantment(EnchantmentSOBase enchantment) => TrySelectEnchantment(enchantment);

        public bool TrySelectEnchantment(EnchantmentSOBase enchantment)
        {
            if (_state != EnchantmentSelectionState.ENCHANTMENTS_OFFERED ||
                !_list_CurrentOfferedEnchantments.Contains(enchantment) || _deckBuildRules == null ||
                !_deckBuildRules.CanAcquire(enchantment, _list_AppliedEnchantments)) return false;
            if (enchantment is AbilityEnchantmentSO ability)
            {
                if (ability.AbilityKind == AbilityKind.ACTIVE)
                {
                    _pendingAbility = ability;
                    SetState(EnchantmentSelectionState.WAITING_FOR_ABILITY_SELECTION);
                    return true;
                }
                return BeginAbilityAcquisition(ability, -1);
            }
            SetState(EnchantmentSelectionState.APPLYING_ENCHANTMENT);
            try { enchantment.Apply(CreateContext()); }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                SetState(EnchantmentSelectionState.ENCHANTMENTS_OFFERED);
                return false;
            }
            CompleteAcquisition(enchantment);
            return true;
        }

        public bool TrySelectAbilitySlot(int slotIndex)
        {
            if (_state != EnchantmentSelectionState.WAITING_FOR_ABILITY_SELECTION ||
                _pendingAbility == null || _deckBuildRules == null ||
                !_deckBuildRules.CanAcquire(_pendingAbility, _list_AppliedEnchantments)) return false;
            return BeginAbilityAcquisition(_pendingAbility, slotIndex);
        }

        private bool BeginAbilityAcquisition(AbilityEnchantmentSO ability, int slotIndex)
        {
            if (MN_Abilities.Instance == null) return false;
            EnchantmentSelectionState previousState = _state;
            SetState(ability.AbilityKind == AbilityKind.ACTIVE
                ? EnchantmentSelectionState.APPLYING_ABILITY
                : EnchantmentSelectionState.APPLYING_ENCHANTMENT);
            bool started = MN_Abilities.Instance.TryAddAbility(ability, slotIndex, success =>
            {
                if (this == null) return;
                if (success) CompleteAcquisition(ability);
                else SetState(previousState);
            });
            if (!started) SetState(previousState);
            return started;
        }

        private void CompleteAcquisition(EnchantmentSOBase enchantment)
        {
            _list_AppliedEnchantments.Add(enchantment);
            try { OnEnchantmentApplied?.Invoke(enchantment); }
            finally { CloseOffer(); }
        }

        public bool TrySkipOffer()
        {
            if (!CanSkip) 
                return false;

            if (_defaultDeckConfig != null) 
                _defaultDeckConfig.AddRoll(_skipAdditionalRolls);

            CloseOffer();
            return true;
        }

        private void CloseOffer()
        {
            _currentDeckConfig = null;
            _pendingAbility = null;
            CurrentOfferRolls = 0;
            _list_CurrentOfferedEnchantments.Clear();
            SetState(EnchantmentSelectionState.NONE);
        }

        private void SetState(EnchantmentSelectionState state)
        {
            if (_state == state) return;
            _state = state;
            OnSelectionStateChanged?.Invoke(state);
        }

        public bool HasEnchantment(string enchantmentID)
        {
            if (string.IsNullOrEmpty(enchantmentID)) return false;
            return _list_AppliedEnchantments.Exists(item => item != null && item.ID == enchantmentID);
        }

        public bool HasEnchantment(EnchantmentSOBase enchantment) => enchantment != null &&
            (_list_AppliedEnchantments.Contains(enchantment) || HasEnchantment(enchantment.ID));

        public EnchantmentContext CreateContext() => new EnchantmentContext(MN_PlayerStatus.Instance, _playerObject);
        private void Xp_OnLevelUp(int currentLevel) => OfferEnchantments(_defaultDeckConfig);

        private void BuildOfferedEnchantments(EnchantmentsDeckConfig config)
        {
            _list_CurrentOfferedEnchantments.Clear();

            if (_deckBuildRules == null)
            {
                Debug.LogWarning("[MN_Enchantments] Assign Deck Build Rules before offering enchantments.", this);
                return;
            }

            _list_CurrentOfferedEnchantments.AddRange(_deckBuildRules.Build(
                config,
                _list_AvailableEnchantments, 
                _list_AppliedEnchantments, 
                Mathf.Max(1, _optionsPerLevelUp)
                ));
        }
    }

    public enum EnchantmentSelectionState
    {
        NONE,
        ENCHANTMENTS_OFFERED,
        WAITING_FOR_ABILITY_SELECTION,
        APPLYING_ABILITY,
        APPLYING_ENCHANTMENT
    }
}
