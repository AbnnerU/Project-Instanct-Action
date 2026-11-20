using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Scripts.Abilities;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Scripts.Enchantment
{
    public class AbilityEnchantmentSelectionUI : MonoBehaviour, IPanelHandler
    {
        public event Action<bool> OnPanelOpen;

        [SerializeField] private GameObject _abilitySlotPanel;
        [SerializeField] private Image _abilityToChooseImage;
        [SerializeField] private Sprite _defaulfSlotImage;
        [SerializeField] private List<ButtonConfig> _list_AbilitySlotButtons = new List<ButtonConfig>();
        private MN_Enchantments _enchantmentsManager;
        private MN_Abilities _abilitiesManager;
        private readonly List<UnityAction> _list_SlotCallbacks = new List<UnityAction>();
        private readonly List<AbilitySlotView> _subscribedViews = new List<AbilitySlotView>();

        private AsyncOperationHandle<Sprite> _abilityIconHandle;
        private AbilityEnchantmentSO _iconAbility;

        public bool IsPanelOpen { get; set; }

        private bool _isSubscribed;

        private void Awake()
        {
            ClosePanel();
            for (int i = 0; i < _list_AbilitySlotButtons.Count; i++)
            {
                int index = i;

                UnityAction callback = () => SelectAbilitySlot(index);

                _list_SlotCallbacks.Add(callback);

                if (_list_AbilitySlotButtons[i] != null && _list_AbilitySlotButtons[i].ButtonComponent != null)
                    _list_AbilitySlotButtons[i].ButtonComponent.onClick.AddListener(callback);
            }
        }

        private void Start() => TrySubscribe();

        private void LoadAbilityIcon()
        {
            var ability = _enchantmentsManager != null ? _enchantmentsManager.PendingAbility : null;
            if (ability == _iconAbility) return;
            ReleaseAbilityIcon();
            _iconAbility = ability;
            if (_abilityToChooseImage == null || ability == null || ability.IconReference == null ||
                !ability.IconReference.RuntimeKeyIsValid()) return;

            _abilityIconHandle = Addressables.LoadAssetAsync<Sprite>(ability.IconReference.RuntimeKey);
            _abilityIconHandle.Completed += AbilityIconLoaded;
        }

        private void AbilityIconLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (!IsPanelOpen || !_abilityIconHandle.IsValid() || !handle.Equals(_abilityIconHandle)) return;
            SetAbilityToChooseIcon(handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : null);
        }

        private void ReleaseAbilityIcon()
        {
            SetAbilityToChooseIcon(null);
            _iconAbility = null;

            if (!_abilityIconHandle.IsValid()) return;

            _abilityIconHandle.Completed -= AbilityIconLoaded;

            Addressables.Release(_abilityIconHandle);

            _abilityIconHandle = default;
        }

        private void SetAbilityToChooseIcon(Sprite sprite)
        {
            if (_abilityToChooseImage == null) return;
            _abilityToChooseImage.sprite = sprite;
            _abilityToChooseImage.enabled = sprite != null;
        }

        private void UnsubscribeSlotIcons()
        {
            foreach (var view in _subscribedViews)
                if (view != null) view.OnIconChanged -= RefreshSlotIcons;
            _subscribedViews.Clear();
        }

        private void SubscribeSlotIcons()
        {
            UnsubscribeSlotIcons();

            if (_abilitiesManager == null) _abilitiesManager = MN_Abilities.Instance;
            if (_abilitiesManager == null) return;

            foreach (var slot in _abilitiesManager.List_ActiveAbilitySlots)
            {
                var view = slot?.SlotView;
                if (view == null || _subscribedViews.Contains(view)) continue;
                view.OnIconChanged += RefreshSlotIcons;
                _subscribedViews.Add(view);
            }
        }

        private void RefreshSlotIcons()
        {
            if (_abilitiesManager == null)
                _abilitiesManager = MN_Abilities.Instance;

            if (_abilitiesManager == null) return;

            var slots = _abilitiesManager.List_ActiveAbilitySlots;

            for (int i = 0; i < _list_AbilitySlotButtons.Count; i++)
            {
                var image = _list_AbilitySlotButtons[i]?.AbilityIcon;
                if (image == null) continue;
                image.sprite = slots != null && i < slots.Count && slots[i].IconSprite != null ? slots[i].IconSprite : _defaulfSlotImage;
                image.enabled = image.sprite != null;
            }
        }

        private void TrySubscribe()
        {
            if (_isSubscribed || MN_Enchantments.Instance == null)
                return;

            _enchantmentsManager = MN_Enchantments.Instance;

            _enchantmentsManager.OnSelectionStateChanged += RefreshSelectionControls;
            _isSubscribed = true;
            RefreshSelectionControls(_enchantmentsManager.CurrentState);
        }

        private void OnDestroy()
        {
            ReleaseAbilityIcon();
            UnsubscribeSlotIcons();

            if (_isSubscribed && _enchantmentsManager != null)
                _enchantmentsManager.OnSelectionStateChanged -= RefreshSelectionControls;

            for (int i = 0; i < _list_SlotCallbacks.Count; i++)
                if (_list_AbilitySlotButtons[i] != null && _list_AbilitySlotButtons[i].ButtonComponent != null)
                    _list_AbilitySlotButtons[i].ButtonComponent.onClick.RemoveListener(_list_SlotCallbacks[i]);
        }

        private void RefreshSelectionControls(EnchantmentSelectionState state)
        {
            if (_enchantmentsManager == null) return;

            foreach (ButtonConfig button in _list_AbilitySlotButtons)
                if (button != null && button.ButtonComponent != null)
                    button.ButtonComponent.interactable = state == EnchantmentSelectionState.WAITING_FOR_ABILITY_SELECTION;

            if (state == EnchantmentSelectionState.WAITING_FOR_ABILITY_SELECTION ||
                state == EnchantmentSelectionState.APPLYING_ABILITY)
                OpenPanel();
            else
                ClosePanel();
        }

        public void SelectAbilitySlot(int slotIndex)
        {
            if (_enchantmentsManager != null)
                _enchantmentsManager.TrySelectAbilitySlot(slotIndex);
        }

        public void ClosePanel()
        {
            UnsubscribeSlotIcons();
            ReleaseAbilityIcon();
            bool wasOpen = IsPanelOpen;

            IsPanelOpen = false;

            if (_abilitySlotPanel != null)
                _abilitySlotPanel.SetActive(IsPanelOpen);

            if (wasOpen)
                OnPanelOpen?.Invoke(IsPanelOpen);
        }

        public void OpenPanel()
        {
            if (IsPanelOpen) return;

            SubscribeSlotIcons();
            RefreshSlotIcons();

            IsPanelOpen = true;
            LoadAbilityIcon();

            if (_abilitySlotPanel != null)
                _abilitySlotPanel.SetActive(IsPanelOpen);

            OnPanelOpen?.Invoke(IsPanelOpen);
        }

        [Serializable]
        public class ButtonConfig
        {
            public Button ButtonComponent;
            public Image AbilityIcon;
        }
    }
}
