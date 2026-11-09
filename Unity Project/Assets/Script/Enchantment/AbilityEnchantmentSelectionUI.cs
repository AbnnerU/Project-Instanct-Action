using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Scripts.Abilities;

namespace Scripts.Enchantment
{
    public class AbilityEnchantmentSelectionUI : MonoBehaviour, IPanelHandler
    {
        public event Action<bool> OnPanelOpen;

        [SerializeField] private GameObject _abilitySlotPanel;
        [SerializeField] private List<ButtonConfig> _list_AbilitySlotButtons = new List<ButtonConfig>();

        private MN_Enchantments _enchantmentsManager;
        private MN_Abilities _abilitiesManager;
        //private readonly List<Sprite> _list_EmptySlotSprites = new List<Sprite>();
        private readonly List<UnityAction> _list_SlotCallbacks = new List<UnityAction>();

        public bool IsPanelOpen { get; set; }

        private bool _isSubscribed;

        private void Awake()
        {
            ClosePanel();
            for (int i = 0; i < _list_AbilitySlotButtons.Count; i++)
            {
                Button button = _list_AbilitySlotButtons[i].ButtonComponent;
                //_list_EmptySlotSprites.Add(button != null && button.image != null ? button.image.sprite : null);
                int index = i;
                UnityAction callback = () => SelectAbilitySlot(index);

                _list_SlotCallbacks.Add(callback);

                if (_list_AbilitySlotButtons[i] != null) 
                    _list_AbilitySlotButtons[i].ButtonComponent.onClick.AddListener(callback);
            }
        }

        private void Start() => TrySubscribe();

        private void LateUpdate()
        {
            if (IsPanelOpen) RefreshSlotIcons();
        }

        private void RefreshSlotIcons()
        {
            if (_abilitiesManager == null) _abilitiesManager = MN_Abilities.Instance;
            if (_abilitiesManager == null) return;

            var slots = _abilitiesManager.List_ActiveAbilitySlots;

            for (int i = 0; i < _list_AbilitySlotButtons.Count; i++)
            {
                if (_list_AbilitySlotButtons[i].AbilityIcon == null || slots==null || slots[i] == null || slots[i].IconSprite == null) 
                    continue;

                _list_AbilitySlotButtons[i].AbilityIcon.sprite = slots[i].IconSprite;
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
            if (_isSubscribed && _enchantmentsManager != null)
                _enchantmentsManager.OnSelectionStateChanged -= RefreshSelectionControls;

            for (int i = 0; i < _list_SlotCallbacks.Count; i++)
                if (_list_AbilitySlotButtons[i] != null) 
                        _list_AbilitySlotButtons[i].ButtonComponent.onClick.RemoveListener(_list_SlotCallbacks[i]);
        }

        private void RefreshSelectionControls(EnchantmentSelectionState state)
        {      
            if (_enchantmentsManager == null) return;

            foreach (ButtonConfig button in _list_AbilitySlotButtons)
                if (button.ButtonComponent != null) button.ButtonComponent.interactable = state == EnchantmentSelectionState.WAITING_FOR_ABILITY_SELECTION;

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
            bool wasOpen = IsPanelOpen;
            IsPanelOpen= false;
            if (_abilitySlotPanel != null) _abilitySlotPanel.SetActive(IsPanelOpen);
            if (wasOpen) OnPanelOpen?.Invoke(IsPanelOpen);
        }

        public void OpenPanel()
        {
            if (IsPanelOpen) return;
            RefreshSlotIcons();
            IsPanelOpen = true;
            if (_abilitySlotPanel != null) _abilitySlotPanel.SetActive(IsPanelOpen);
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
