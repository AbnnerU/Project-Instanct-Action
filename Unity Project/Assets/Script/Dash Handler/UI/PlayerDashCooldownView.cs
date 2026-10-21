using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.DashHandler.UI
{
    public class PlayerDashCooldownView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Dash2D _playerDash;
        [SerializeField] private List<DashCooldownBinding> _list_DashCooldownBindings = new List<DashCooldownBinding>();

        #endregion

        #region Unity

        private void Awake()
        {
            if (_playerDash == null)
                _playerDash = FindAnyObjectByType<Dash2D>();
        }

        private void OnEnable() => RefreshImages();
        private void Update() => RefreshImages();

        #endregion

        #region View

        private void RefreshImages()
        {
            foreach (DashCooldownBinding binding in _list_DashCooldownBindings)
            {
                if (binding == null || binding.RechargeImage == null)
                    continue;
                binding.RechargeImage.fillAmount = _playerDash != null
                    ? _playerDash.GetCooldownFillAmount(binding.DirectionSide)
                    : 0f;
            }
        }

        #endregion

        #region Classes

        [Serializable]
        private class DashCooldownBinding
        {
            public DashInputSide DirectionSide;
            public Image RechargeImage;
        }

        #endregion
    }
}
