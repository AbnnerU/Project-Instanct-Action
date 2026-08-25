using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Enchantment
{
    public class EnchantmentSelectionUI : MonoBehaviour, IPanelHandler
    {
        public event Action<bool> OnPanelOpen;

        [SerializeField] private GameObject _panelObject;
        [SerializeField] private List<EnchantmentCardUI> _list_Cards = new List<EnchantmentCardUI>();
        [SerializeField] private bool _pauseGameWhileSelecting = true;

        private float _previousTimeScale = 1f;
        private int _expectedIconsAmount = 0;
        private int _iconsLoaded = 0;
        private bool _isSelectionActive;
        private bool _hasPausedGame;
        private bool _isSubscribed;

        public bool IsPanelOpen { get; set; }

        private void Awake() => Close();

        private void Start() => TrySubscribe();

        private void OnDestroy()
        {
            if (_isSubscribed && MN_Enchantments.Instance != null)
                MN_Enchantments.Instance.OnEnchantmentsOffered -= Enchantments_OnEnchantmentsOffered;
        }

        private void Enchantments_OnEnchantmentsOffered(IReadOnlyList<EnchantmentSOBase> enchantments)
        {
            Open(enchantments);
        }

        public void Open(IReadOnlyList<EnchantmentSOBase> list_Enchantments)
        {
            if (list_Enchantments == null || list_Enchantments.Count == 0)
                return;

            if (_isSelectionActive)
                return;

            _isSelectionActive = true;
            _expectedIconsAmount = GetExpectedIconsAmount(list_Enchantments);

            if (_expectedIconsAmount <= 0)
            {
                _isSelectionActive = false;
                return;
            }

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
            for (int i = 0; i < _list_Cards.Count; i++)
            {
                if (_list_Cards[i] != null)
                    _list_Cards[i].Clear();
            }

            if (_pauseGameWhileSelecting && _hasPausedGame)
            {
                Time.timeScale = _previousTimeScale;
                _hasPausedGame = false;
            }

            _isSelectionActive = false;
            ClosePanel();
        }

        private void SelectEnchantment(EnchantmentSOBase enchantment)
        {
            try
            {
                if (MN_Enchantments.Instance != null)
                    MN_Enchantments.Instance.ApplyEnchantment(enchantment);
            }
            finally
            {
                Close();
            }
        }

        private void TrySubscribe()
        {
            if (_isSubscribed || MN_Enchantments.Instance == null)
                return;

            MN_Enchantments.Instance.OnEnchantmentsOffered += Enchantments_OnEnchantmentsOffered;
            _isSubscribed = true;

            for (int i = 0; i < _list_Cards.Count; i++)
            {
                _list_Cards[i].OnLoadIconCompleted += AllIconsLoaded;
                _list_Cards[i].OnSelected += SelectEnchantment;
            }
        }

        private void AllIconsLoaded()
        {
            if (IsPanelOpen) return;

            _iconsLoaded++;

            if (_iconsLoaded >= _expectedIconsAmount)
                OpenPanel();
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
            _panelObject.SetActive(true);
            IsPanelOpen = true;
            OnPanelOpen?.Invoke(IsPanelOpen);
        }

        public void ClosePanel()
        {
            _panelObject.SetActive(false);
            IsPanelOpen = false;
            OnPanelOpen?.Invoke(IsPanelOpen);
        }
    }
}
