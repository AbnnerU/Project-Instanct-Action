using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Scripts.Status
{
    public class StatusInfoTextView : MonoBehaviour
    {
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [SerializeField] private bool _updateOnStart = true;
        [SerializeField] private bool _autoUpdate = true;
        [SerializeField] private List<StatusTextBinding> _list_StatusTextBindings = new List<StatusTextBinding>();

        private StatusInfo _statusInfoReference;

        #region Unity

        private void Start()
        {
            _statusInfoReference = _statusSource.GetStatusInfo();

            StartCoroutine(Wait());
        }

        IEnumerator Wait()
        {
            yield return new WaitForEndOfFrame();
            if (_autoUpdate)
                SubscribeStatusEvents();

            if (_updateOnStart)
                UpdateAllTexts();
        }

        private void OnDisable()
        {
            UnsubscribeStatusEvents();
        }

        #endregion

        #region Events

        private void SubscribeStatusEvents()
        {
            if (_statusInfoReference == null)
                return;

            for (int i = 0; i < _list_StatusTextBindings.Count; i++)
            {
                StatusTextBinding binding = _list_StatusTextBindings[i];

                if (binding == null) continue;
                if (!_statusSource.TryGetStatusData(binding.StatusType, out StatusData data) || data == null)
                {
                    Debug.LogWarning($"[StatusInfoTextView] Can't find status {binding.StatusType}");
                    continue;
                }

                data.OnCurrentValueUpdated += binding.UpdateText;
                Debug.Log($"Can update {binding.StatusType.ToString()} | {binding.TextComponent.name}");
            }
        }

        private void UnsubscribeStatusEvents()
        {
            if(_statusInfoReference == null) return;

            for (int i = 0; i < _list_StatusTextBindings.Count; i++)
            {
                StatusTextBinding binding = _list_StatusTextBindings[i];
                if (binding == null) continue;

                if (!_statusSource.TryGetStatusData(binding.StatusType, out StatusData data) || data == null)
                {
                    Debug.LogWarning($"[StatusInfoTextView] Can't find status {binding.StatusType}");
                    continue;
                }
                
                data.OnCurrentValueUpdated -= binding.UpdateText;
            }
        }

        #endregion

        #region Public

        public void UpdateAllTexts()
        {
            if (_statusSource == null)
                return;

            for (int i = 0; i < _list_StatusTextBindings.Count; i++)
                UpdateText(_list_StatusTextBindings[i]);
        }

        public void UpdateText(StatusType statusType)
        {
            if (_statusSource == null)
                return;

            float currentValue = _statusSource.GetStatusInfo().GetCurrentValueOf(statusType);
            UpdateText(statusType, currentValue);
        }

        #endregion

  

        #region Text
        private void UpdateText(StatusTextBinding binding)
        {
            if (binding == null)
                return;

            float currentValue = _statusSource.GetStatusInfo().GetCurrentValueOf(binding.StatusType);
            binding.UpdateText(currentValue);
        }

        private void UpdateText(StatusType statusType, float value)
        {
            for (int i = 0; i < _list_StatusTextBindings.Count; i++)
            {
                StatusTextBinding binding = _list_StatusTextBindings[i];

                if (binding == null || binding.StatusType != statusType)
                    continue;

                binding.UpdateText(value);
            }
        }
        #endregion

        #region Classes

        [Serializable]
        public class StatusTextBinding
        {
            public StatusType StatusType;
            public TMP_Text TextComponent;
            [Tooltip("Use 0 for whole numbers or 0'%' for a literal percent suffix. Do not use 0%: it multiplies the value by 100.")]
            public string _numberFormat = "0";

            public void UpdateText( float value)
            {
                Debug.Log($"Text updated {StatusType.ToString()}");

                if (TextComponent == null)
                    return;

                TextComponent.text = value.ToString(_numberFormat);
            }
        }

        #endregion
    }
}
