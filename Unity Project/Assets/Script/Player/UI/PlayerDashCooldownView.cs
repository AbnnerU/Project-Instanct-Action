using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Player.UI
{
    public class PlayerDashCooldownView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private PlayerDash _playerDash;
        [SerializeField] private List<DashCooldownBinding> _list_DashCooldownBindings = new List<DashCooldownBinding>();

        #endregion

        #region Unity

        private void Awake()
        {
            if (_playerDash == null)
                _playerDash = FindAnyObjectByType<PlayerDash>();
        }

        private void OnEnable()
        {
            SubscribeEvents();
            RefreshImages();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
            StopCooldownRoutines();
        }

        #endregion

        #region Events

        private void SubscribeEvents()
        {
            if (_playerDash == null)
                return;

            _playerDash.OnDashCooldownStarted += PlayerDash_OnDashCooldownStarted;
            _playerDash.OnDashCooldownFinished += PlayerDash_OnDashCooldownFinished;
        }

        private void UnsubscribeEvents()
        {
            if (_playerDash == null)
                return;

            _playerDash.OnDashCooldownStarted -= PlayerDash_OnDashCooldownStarted;
            _playerDash.OnDashCooldownFinished -= PlayerDash_OnDashCooldownFinished;
        }

        private void PlayerDash_OnDashCooldownStarted(DirectionSide directionSide, float cooldown)
        {
            DashCooldownBinding binding = GetBinding(directionSide);

            if (binding == null)
                return;

            StartCooldownRoutine(binding, cooldown);
        }

        private void PlayerDash_OnDashCooldownFinished(DirectionSide directionSide)
        {
            DashCooldownBinding binding = GetBinding(directionSide);

            if (binding == null)
                return;

            StopCooldownRoutine(binding);
            SetFillAmount(binding, 0f);
        }

        #endregion

        #region Cooldown

        private void RefreshImages()
        {
            for (int i = 0; i < _list_DashCooldownBindings.Count; i++)
            {
                DashCooldownBinding binding = _list_DashCooldownBindings[i];

                if (binding == null)
                    continue;

                float fillAmount = _playerDash != null ? _playerDash.GetCooldownFillAmount(binding.DirectionSide) : 0f;
                SetFillAmount(binding, fillAmount);
            }
        }

        private void StartCooldownRoutine(DashCooldownBinding binding, float cooldown)
        {
            StopCooldownRoutine(binding);

            if (cooldown <= 0f)
            {
                SetFillAmount(binding, 0f);
                return;
            }

            binding.CooldownRoutine = StartCoroutine(CooldownRoutine(binding, cooldown));
        }

        private IEnumerator CooldownRoutine(DashCooldownBinding binding, float cooldown)
        {
            float elapsedTime = 0f;

            while (elapsedTime < cooldown)
            {
                float fillAmount = 1f - Mathf.Clamp01(elapsedTime / cooldown);
                SetFillAmount(binding, fillAmount);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            SetFillAmount(binding, 0f);
            binding.CooldownRoutine = null;
        }

        private void StopCooldownRoutines()
        {
            for (int i = 0; i < _list_DashCooldownBindings.Count; i++)
            {
                if (_list_DashCooldownBindings[i] != null)
                    StopCooldownRoutine(_list_DashCooldownBindings[i]);
            }
        }

        private void StopCooldownRoutine(DashCooldownBinding binding)
        {
            if (binding.CooldownRoutine == null)
                return;

            StopCoroutine(binding.CooldownRoutine);
            binding.CooldownRoutine = null;
        }

        private void SetFillAmount(DashCooldownBinding binding, float fillAmount)
        {
            if (binding.RechargeImage == null)
                return;

            binding.RechargeImage.fillAmount = Mathf.Clamp01(fillAmount);
        }

        private DashCooldownBinding GetBinding(DirectionSide directionSide)
        {
            for (int i = 0; i < _list_DashCooldownBindings.Count; i++)
            {
                DashCooldownBinding binding = _list_DashCooldownBindings[i];

                if (binding != null && binding.DirectionSide == directionSide)
                    return binding;
            }

            return null;
        }

        #endregion

        #region Classes

        #pragma warning disable 0649
        [Serializable]
        private class DashCooldownBinding
        {
            public DirectionSide DirectionSide;
            public Image RechargeImage;

            [NonSerialized] public Coroutine CooldownRoutine;
        }
        #pragma warning restore 0649

        #endregion
    }
}
