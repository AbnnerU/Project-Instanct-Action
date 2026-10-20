using UnityEngine;

namespace Scripts.Status
{
    public class StatusInfoByReference : StatusInfoSourceBase
    {
        #region Fields

        [SerializeField] private GameObject _reference;
        [SerializeField] private bool _searchByTag;
        [SerializeField] private string _referenceTag;

        private GameObject _cachedReference;
        private IHasStatusInfoReference _statusProvider;
        private Component _providerComponent;
        private bool _hasInvalidTag;

        #endregion

        #region Status

        public override StatusInfo GetStatusInfo()
        {
            if (_reference == null && _searchByTag && !_hasInvalidTag &&
                !string.IsNullOrWhiteSpace(_referenceTag))
            {
                try
                {
                    _reference = GameObject.FindWithTag(_referenceTag);
                }
                catch (UnityException)
                {
                    _hasInvalidTag = true;
                    Debug.LogWarning($"[{nameof(StatusInfoByReference)}] Undefined tag: {_referenceTag}.", this);
                }
            }

            if (_reference == null)
                return null;

            if (_cachedReference != _reference || _providerComponent == null)
            {
                _cachedReference = _reference;
                _statusProvider = _reference.GetComponent<IHasStatusInfoReference>();
                _providerComponent = _statusProvider as Component;
            }

            return _providerComponent != null ? _statusProvider.GetStatusInfo() : null;
        }

        public override bool TryGetStatusData(StatusType statusType, out StatusData statusData)
        {
            StatusInfo statusInfo = GetStatusInfo();
            statusData = statusInfo != null ? statusInfo.GetStatusData(statusType) : null;
            return statusData != null;
        }

        #endregion
    }
}
