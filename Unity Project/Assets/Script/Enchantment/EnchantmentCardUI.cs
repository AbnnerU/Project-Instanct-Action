using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Scripts.Enchantment
{
    public class EnchantmentCardUI : MonoBehaviour
    {
        [SerializeField] private Button _buttonComponent;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _displayNameText;
        [SerializeField] private TMP_Text _descriptionText;

        private EnchantmentSOBase _enchantment;
        private Action<EnchantmentSOBase> _onSelected;
        private AsyncOperationHandle<Sprite> _iconLoadHandle;

        private void Awake()
        {
            if (_buttonComponent == null)
                TryGetComponent(out _buttonComponent);

            if (_buttonComponent != null)
                _buttonComponent.onClick.AddListener(Select);
        }

        private void OnDestroy()
        {
            if (_buttonComponent != null)
                _buttonComponent.onClick.RemoveListener(Select);

            ReleaseIconHandle();
        }

        public void Setup(EnchantmentSOBase enchantment, Action<EnchantmentSOBase> onSelected)
        {
            ReleaseIconHandle();

            _enchantment = enchantment;
            _onSelected = onSelected;

            gameObject.SetActive(enchantment != null);

            if (enchantment == null)
                return;

            if (_displayNameText != null)
                _displayNameText.text = enchantment.DisplayName;

            if (_descriptionText != null)
                _descriptionText.text = enchantment.GetResolvedDescription();

            if (_iconImage != null)
            {
                _iconImage.enabled = false;
                LoadIcon(enchantment.IconReference);
            }
        }

        public void Clear()
        {
            ReleaseIconHandle();

            _enchantment = null;
            _onSelected = null;

            if (_displayNameText != null)
                _displayNameText.text = string.Empty;

            if (_descriptionText != null)
                _descriptionText.text = string.Empty;

            if (_iconImage != null)
            {
                _iconImage.sprite = null;
                _iconImage.enabled = false;
            }

            gameObject.SetActive(false);
        }

        private void Select()
        {
            if (_enchantment == null)
                return;

            _onSelected?.Invoke(_enchantment);
        }

        private void LoadIcon(AssetReferenceSprite iconReference)
        {
            if (iconReference == null || !iconReference.RuntimeKeyIsValid())
                return;

            _iconLoadHandle = iconReference.LoadAssetAsync();
            _iconLoadHandle.Completed += IconLoadHandle_Completed;
        }

        private void IconLoadHandle_Completed(AsyncOperationHandle<Sprite> handle)
        {
            if (_iconImage == null || handle.Status != AsyncOperationStatus.Succeeded)
                return;

            _iconImage.sprite = handle.Result;
            _iconImage.enabled = handle.Result != null;
        }

        private void ReleaseIconHandle()
        {
            if (!_iconLoadHandle.IsValid())
                return;

            _iconLoadHandle.Completed -= IconLoadHandle_Completed;
            Addressables.Release(_iconLoadHandle);
            _iconLoadHandle = default;
        }
    }
}
