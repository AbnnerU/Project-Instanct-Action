using System.Collections;
using Scripts.Timer;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Scripts.Abilities.UI
{
    public class AbilitySlotView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _rechargeImage;
        [SerializeField] private GameObject _blockedObject;

        private MN_PlayerAbilities.ActiveAbilitySlot _slot;
        private AsyncOperationHandle<Sprite> _iconLoadHandle;
        private Coroutine _cooldownRoutine;

        #endregion

        #region Unity

        private void OnDisable()
        {
            StopCooldownRoutine();
        }

        private void OnDestroy()
        {
            UnbindSlot();
            ReleaseIconHandle();
        }

        #endregion

        #region Public

        public void Setup(MN_PlayerAbilities.ActiveAbilitySlot slot)
        {
            if (_slot == slot)
            {
                Refresh();
                return;
            }

            UnbindSlot();
            _slot = slot;
            BindSlot();
            Refresh();
        }

        public void Clear()
        {
            UnbindSlot();
            _slot = null;
            ReleaseIconHandle();
            SetIcon(null);
            SetCooldownFillAmount(0f);
            SetBlocked(false);
        }

        #endregion

        #region Slot

        private void BindSlot()
        {
            if (_slot == null)
                return;

            _slot.OnAbilityUpdated += Slot_OnAbilityUpdated;
            _slot.OnAbilityCooldownStarted += Slot_OnAbilityCooldownStarted;
            _slot.OnAbilityCooldownCompleted += Slot_OnAbilityCooldownCompleted;
            _slot.OnAbilitySlotStateUpdated += Slot_OnAbilitySlotStateUpdated;
        }

        private void UnbindSlot()
        {
            if (_slot == null)
                return;

            _slot.OnAbilityUpdated -= Slot_OnAbilityUpdated;
            _slot.OnAbilityCooldownStarted -= Slot_OnAbilityCooldownStarted;
            _slot.OnAbilityCooldownCompleted -= Slot_OnAbilityCooldownCompleted;
            _slot.OnAbilitySlotStateUpdated -= Slot_OnAbilitySlotStateUpdated;
        }

        private void Slot_OnAbilityUpdated(MN_PlayerAbilities.ActiveAbilitySlot slot)
        {
            Refresh();
        }

        private void Slot_OnAbilityCooldownStarted(float cooldown)
        {
            StartCooldownRoutine(cooldown, 0f);
        }

        private void Slot_OnAbilityCooldownCompleted(float cooldown)
        {
            StopCooldownRoutine();
            SetCooldownFillAmount(0f);
        }

        private void Slot_OnAbilitySlotStateUpdated(MN_PlayerAbilities.ActiveAbilitySlot.AbilitySlotState slotState)
        {
            SetBlocked(slotState == MN_PlayerAbilities.ActiveAbilitySlot.AbilitySlotState.BLOCKED);
        }

        #endregion

        #region Refresh

        private void Refresh()
        {
            ReleaseIconHandle();

            if (_slot == null || _slot.IsEmpty)
            {
                SetIcon(null);
                SetCooldownFillAmount(0f);
                SetBlocked(false);
                return;
            }

            LoadIcon(_slot.IconReference);
            SetBlocked(_slot.SlotState == MN_PlayerAbilities.ActiveAbilitySlot.AbilitySlotState.BLOCKED);
            RefreshCooldown();
        }

        private void RefreshCooldown()
        {
            IHasCooldown cooldownHandler = _slot.CooldownHandler;

            if (cooldownHandler == null || _slot.SlotState != MN_PlayerAbilities.ActiveAbilitySlot.AbilitySlotState.ABILITY_IN_COOLDOWN)
            {
                SetCooldownFillAmount(0f);
                return;
            }

            float cooldown = cooldownHandler.Cooldown;
            float elapsedTime = cooldownHandler.CooldownProgression;

            StartCooldownRoutine(cooldown, elapsedTime);
        }

        #endregion

        #region Cooldown

        private void StartCooldownRoutine(float cooldown, float elapsedTime)
        {
            StopCooldownRoutine();

            if (cooldown <= 0f)
            {
                SetCooldownFillAmount(0f);
                return;
            }

            _cooldownRoutine = StartCoroutine(CooldownRoutine(cooldown, elapsedTime));
        }

        private IEnumerator CooldownRoutine(float cooldown, float elapsedTime)
        {
            while (elapsedTime < cooldown)
            {
                float fillAmount = 1f - Mathf.Clamp01(elapsedTime / cooldown);
                SetCooldownFillAmount(fillAmount);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            SetCooldownFillAmount(0f);
            _cooldownRoutine = null;
        }

        private void StopCooldownRoutine()
        {
            if (_cooldownRoutine == null)
                return;

            StopCoroutine(_cooldownRoutine);
            _cooldownRoutine = null;
        }

        private void SetCooldownFillAmount(float fillAmount)
        {
            if (_rechargeImage == null)
                return;

            _rechargeImage.fillAmount = Mathf.Clamp01(fillAmount);
        }

        #endregion

        #region Icon

        private void LoadIcon(AssetReferenceSprite iconReference)
        {
            if (iconReference == null || !iconReference.RuntimeKeyIsValid())
            {
                SetIcon(null);
                return;
            }

            _iconLoadHandle = Addressables.LoadAssetAsync<Sprite>(iconReference.RuntimeKey);
            _iconLoadHandle.Completed += IconLoadHandle_Completed;
        }

        private void IconLoadHandle_Completed(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
                return;

            SetIcon(handle.Result);
        }

        private void SetIcon(Sprite icon)
        {
            if (_iconImage == null)
                return;

            _iconImage.sprite = icon;
            _iconImage.enabled = icon != null;
        }

        private void ReleaseIconHandle()
        {
            if (!_iconLoadHandle.IsValid())
                return;

            _iconLoadHandle.Completed -= IconLoadHandle_Completed;
            Addressables.Release(_iconLoadHandle);
            _iconLoadHandle = default;
        }

        #endregion

        #region Blocked

        private void SetBlocked(bool blocked)
        {
            if (_blockedObject == null)
                return;

            _blockedObject.SetActive(blocked);
        }

        #endregion
    }
}
