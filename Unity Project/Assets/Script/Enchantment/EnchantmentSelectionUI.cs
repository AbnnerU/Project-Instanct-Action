using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Enchantment
{
    public class EnchantmentSelectionUI : MonoBehaviour
    {
        [SerializeField] private GameObject _panelObject;
        [SerializeField] private List<EnchantmentCardUI> _list_Cards = new List<EnchantmentCardUI>();
        [SerializeField] private bool _pauseGameWhileSelecting = true;

        private float _previousTimeScale = 1f;
        private bool _isOpen;
        private bool _isSubscribed;
        private CanvasGroup _selfCanvasGroup;

        private void Awake()
        {
            if (_panelObject == null)
                _panelObject = gameObject;

            if (_panelObject == gameObject)
                _selfCanvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

            TrySubscribe();
            Close();
        }

        private void Start()
        {
            TrySubscribe();
        }

        private void OnDestroy()
        {
            if (_isSubscribed && MN_Enchantments.Instance != null)
                MN_Enchantments.Instance.OnEnchantmentsOffered -= Enchantments_OnEnchantmentsOffered;
        }

        private void Enchantments_OnEnchantmentsOffered(IReadOnlyList<EnchantmentSOBase> enchantments)
        {
            Open(enchantments);
        }

        public void Open(IReadOnlyList<EnchantmentSOBase> enchantments)
        {
            if (enchantments == null || enchantments.Count == 0)
                return;

            if (_selfCanvasGroup != null)
                SetCanvasGroupVisible(true);
            else if (_panelObject != null)
                _panelObject.SetActive(true);

            if (!_isOpen && _pauseGameWhileSelecting)
            {
                _previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }

            _isOpen = true;

            for (int i = 0; i < _list_Cards.Count; i++)
            {
                if (_list_Cards[i] == null) continue;

                if (i < enchantments.Count)
                    _list_Cards[i].Setup(enchantments[i], SelectEnchantment);
                else
                    _list_Cards[i].Clear();
            }
        }

        public void Close()
        {
            if (_selfCanvasGroup != null)
                SetCanvasGroupVisible(false);
            else if (_panelObject != null)
                _panelObject.SetActive(false);

            for (int i = 0; i < _list_Cards.Count; i++)
            {
                if (_list_Cards[i] != null)
                    _list_Cards[i].Clear();
            }

            if (_isOpen && _pauseGameWhileSelecting)
                Time.timeScale = _previousTimeScale;

            _isOpen = false;
        }

        private void SelectEnchantment(EnchantmentSOBase enchantment)
        {
            if (MN_Enchantments.Instance != null)
                MN_Enchantments.Instance.ApplyEnchantment(enchantment);

            Close();
        }

        private void TrySubscribe()
        {
            if (_isSubscribed || MN_Enchantments.Instance == null)
                return;

            MN_Enchantments.Instance.OnEnchantmentsOffered += Enchantments_OnEnchantmentsOffered;
            _isSubscribed = true;
        }

        private void SetCanvasGroupVisible(bool visible)
        {
            _selfCanvasGroup.alpha = visible ? 1f : 0f;
            _selfCanvasGroup.interactable = visible;
            _selfCanvasGroup.blocksRaycasts = visible;
        }
    }
}
