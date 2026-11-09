
using Scripts.TierManipulation;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Scripts.Enchantment
{
    public class EnchantmentCardUI : MonoBehaviour
    {
        public Action OnLoadIconCompleted;
        public Action<EnchantmentSOBase> OnSelected;

        [SerializeField] private Button _buttonComponent;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _displayNameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TierVisual _tierVisual;

        private EnchantmentSOBase _enchantment;
        private AsyncOperationHandle<Sprite> _iconLoadHandle;

        private GameObject _thisObject;

        private void Awake()
        {
            if (_buttonComponent == null)
                TryGetComponent(out _buttonComponent);

            if (_buttonComponent != null)
                _buttonComponent.onClick.AddListener(Select);

            _thisObject = gameObject;
        }

        private void OnDestroy()
        {
            if (_buttonComponent != null)
                _buttonComponent.onClick.RemoveListener(Select);

            ReleaseIconHandle();
        }

        public void Setup(EnchantmentSOBase enchantment, Action<EnchantmentSOBase> onSelected)
        {
            SetAsEmpty(false);

            ReleaseIconHandle();

            _enchantment = enchantment;

            gameObject.SetActive(enchantment != null);

            if (enchantment == null)
                return;

            if (_tierVisual != null)
                _tierVisual.EnableTierVisual(_enchantment.Tier);

            if (_displayNameText != null)
                _displayNameText.text = enchantment.DisplayName;

            if (_descriptionText != null)
                _descriptionText.text = enchantment.GetResolvedDescription();

            if (_iconImage != null)
                LoadIcon(enchantment.IconReference);
            else 
                OnLoadIconCompleted?.Invoke();       
        }

        public void SetInteractable(bool interactable)
        {
            if (_buttonComponent != null) _buttonComponent.interactable = interactable;
        }

        public void Clear()
        {
            ReleaseIconHandle();

            _enchantment = null;

            if (_displayNameText != null)
                _displayNameText.text = string.Empty;

            if (_descriptionText != null)
                _descriptionText.text = string.Empty;

            if (_iconImage != null)
            {
                _iconImage.sprite = null;
                _iconImage.enabled = false;
            }
        }


        public void SetAsEmpty(bool isEmpty)
        {
            if(_thisObject == null)
                _thisObject = gameObject;

            _thisObject.SetActive(!isEmpty);
        }

        private void Select()
        {
            if (_enchantment == null)
                return;

            OnSelected?.Invoke(_enchantment);
        }

        private void LoadIcon(AssetReferenceSprite iconReference)
        {
            if (iconReference == null || !iconReference.RuntimeKeyIsValid())
            {
                OnLoadIconCompleted?.Invoke();
                return;
            }

            _iconLoadHandle = Addressables.LoadAssetAsync<Sprite>(iconReference.RuntimeKey);
            _iconLoadHandle.Completed += IconLoadHandle_Completed;
        }

        private void IconLoadHandle_Completed(AsyncOperationHandle<Sprite> handle)
        {
            if (_iconImage == null || handle.Status != AsyncOperationStatus.Succeeded)
            {
                OnLoadIconCompleted?.Invoke();
                return;
            }

            _iconImage.sprite = handle.Result;
            _iconImage.enabled = true;

            OnLoadIconCompleted?.Invoke();
        }

        private void ReleaseIconHandle()
        {
            if (!_iconLoadHandle.IsValid())
                return;

            _iconLoadHandle.Completed -= IconLoadHandle_Completed;
            Addressables.Release(_iconLoadHandle);
            _iconLoadHandle = default;
        }

        [Serializable]
        public class TierVisual
        {
            public List<TierVisualConfig> TierVisualConfigs = new List<TierVisualConfig>();

            public void EnableTierVisual(Tier tierReference)
            {
                DisableAll();

                TierVisualConfig config = TierVisualConfigs.Find(x=>x.TierReference == tierReference);

                if(config != null) 
                    config.TierObj?.SetActive(true);
            }

            public void DisableAll()
            {
                foreach(var tierVisual in TierVisualConfigs)
                    tierVisual.TierObj?.SetActive(false);
            }

        }

        [Serializable]
        public class TierVisualConfig
        {
            public Tier TierReference;
            public GameObject TierObj;
        }
    }
}
