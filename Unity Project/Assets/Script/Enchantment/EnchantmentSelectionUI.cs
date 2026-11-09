using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Enchantment
{
    public class EnchantmentSelectionUI : MonoBehaviour, IPanelHandler
    {
        public event Action<bool> OnPanelOpen;

        [SerializeField] private GameObject _panelObject;
        [SerializeField] private List<EnchantmentCardUI> _list_Cards = new List<EnchantmentCardUI>();
        [SerializeField] private bool _pauseGameWhileSelecting = true;
        [SerializeField] private Button _skipButton;

        private MN_Enchantments _enchantmentsManager;

        private float _previousTimeScale = 1f;
        private int _expectedIconsAmount = 0;
        private int _iconsLoaded = 0;
        private bool _hasPausedGame;
        private bool _isSubscribed;

        public bool IsPanelOpen { get; set; }

        private void Awake()
        {
            if (_skipButton != null) _skipButton.onClick.AddListener(Skip);

            Close();
        }

        private void Start() => TrySubscribe();

        private void OnDestroy()
        {
            if (_isSubscribed && _enchantmentsManager != null)
            {
                _enchantmentsManager.OnEnchantmentsOffered -= Enchantments_OnEnchantmentsOffered;
                _enchantmentsManager.OnSelectionStateChanged -= RefreshSelectionControls;
            }

            foreach (EnchantmentCardUI card in _list_Cards)
                if (card != null)
                {
                    card.OnLoadIconCompleted -= AllIconsLoaded;
                    card.OnSelected -= SelectEnchantment;
                }

            if (_skipButton != null) _skipButton.onClick.RemoveListener(Skip);
         
            if (_hasPausedGame) Time.timeScale = _previousTimeScale;
        }

        private void Enchantments_OnEnchantmentsOffered(IReadOnlyList<EnchantmentSOBase> enchantments)
        {
            Open(enchantments);
        }

        public void Open(IReadOnlyList<EnchantmentSOBase> list_Enchantments)
        {
            if (list_Enchantments == null || list_Enchantments.Count == 0)
                return;

            if (_expectedIconsAmount > 0)
                return;

            _expectedIconsAmount = GetExpectedIconsAmount(list_Enchantments);

            if (_expectedIconsAmount <= 0)
                return;

            if (_pauseGameWhileSelecting)
            {
                _previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                _hasPausedGame = true;
            }

            _iconsLoaded = 0;

            for (int i = 0; i < _list_Cards.Count; i++)
            {
                if (_list_Cards[i] == null) continue;

                if (i < list_Enchantments.Count)
                    _list_Cards[i].Setup(list_Enchantments[i], SelectEnchantment);
                else
                    _list_Cards[i].SetAsEmpty(true);
            }
        }

        public void Close()
        {
            _expectedIconsAmount = 0;
            _iconsLoaded = 0;
            for (int i = 0; i < _list_Cards.Count; i++)
            {
                if (_list_Cards[i] != null)
                    _list_Cards[i].Clear();
            }

            if (_hasPausedGame)
            {
                Time.timeScale = _previousTimeScale;
                _hasPausedGame = false;
            }

            if (_skipButton != null) _skipButton.gameObject.SetActive(false);
            ClosePanel();
        }

        private void SelectEnchantment(EnchantmentSOBase enchantment)
        {
            if (_enchantmentsManager != null) _enchantmentsManager.TrySelectEnchantment(enchantment);
        }

        public void Skip()
        {
            if (_enchantmentsManager != null) _enchantmentsManager.TrySkipOffer();
        }

        private void RefreshSelectionControls(EnchantmentSelectionState state)
        {      
            if (_enchantmentsManager == null) return;

            if (state == EnchantmentSelectionState.NONE)
            {
                Close();
                return;
            }
            bool showCards = (state == EnchantmentSelectionState.ENCHANTMENTS_OFFERED ||
                state == EnchantmentSelectionState.APPLYING_ENCHANTMENT) && _expectedIconsAmount > 0 &&
                _iconsLoaded >= _expectedIconsAmount;

            if (_skipButton != null)
                _skipButton.gameObject.SetActive(showCards && _enchantmentsManager.CanSkip);

            foreach (EnchantmentCardUI card in _list_Cards)
                if (card != null) card.SetInteractable(showCards && state == EnchantmentSelectionState.ENCHANTMENTS_OFFERED);

            if (showCards)
                OpenPanel();
            else
                ClosePanel();         
        }

        private void TrySubscribe()
        {
            if (_isSubscribed || MN_Enchantments.Instance == null)
                return;

            _enchantmentsManager = MN_Enchantments.Instance;

            _enchantmentsManager.OnEnchantmentsOffered += Enchantments_OnEnchantmentsOffered;
            _enchantmentsManager.OnSelectionStateChanged += RefreshSelectionControls;
            _isSubscribed = true;

            for (int i = 0; i < _list_Cards.Count; i++)
            {
                if (_list_Cards[i] == null) continue;
                _list_Cards[i].OnLoadIconCompleted += AllIconsLoaded;
                _list_Cards[i].OnSelected += SelectEnchantment;
            }

            if (_enchantmentsManager.CurrentState != EnchantmentSelectionState.NONE)
                Open(_enchantmentsManager.List_CurrentOfferedEnchantments);

            RefreshSelectionControls(_enchantmentsManager.CurrentState);
        }

        private void AllIconsLoaded()
        {
            if (_expectedIconsAmount <= 0 || _iconsLoaded >= _expectedIconsAmount) return;

            _iconsLoaded++;

            RefreshSelectionControls(_enchantmentsManager.CurrentState);
        }

        private int GetExpectedIconsAmount(IReadOnlyList<EnchantmentSOBase> list_Enchantments)
        {
            int expectedIconsAmount = 0;

            for (int i = 0; i < _list_Cards.Count; i++)
            {
                if (_list_Cards[i] != null && i < list_Enchantments.Count)
                    expectedIconsAmount++;
            }

            return expectedIconsAmount;
        }

        public void OpenPanel()
        {
            if (IsPanelOpen) return;
            IsPanelOpen = true;

            if (_panelObject != null) 
                _panelObject.SetActive(IsPanelOpen);
        
            OnPanelOpen?.Invoke(IsPanelOpen);
        }

        public void ClosePanel()
        {
            bool wasOpen = IsPanelOpen;
            IsPanelOpen = false;

            if (_panelObject != null) 
                _panelObject.SetActive(IsPanelOpen);

            if (wasOpen) OnPanelOpen?.Invoke(IsPanelOpen);
        }
    }
}
