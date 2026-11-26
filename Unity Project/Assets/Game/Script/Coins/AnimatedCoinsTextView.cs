using TMPro;
using UnityEngine;

namespace Scripts.Economy
{
    public class AnimatedCoinsTextView : MonoBehaviour
    {
        [SerializeField] private MN_Coins _coinsManager;
        [SerializeField] private TMP_Text _coinsText;
        [SerializeField] private TMP_Text _addedCoinsText;
        [SerializeField, Min(0f)] private float _animationDuration = 0.4f;
        [SerializeField, Min(0f)] private float _addedCoinsVisibleDuration = 2f;

        private bool _subscribed;
        private float _displayedCoins;
        private float _coinsFrom;
        private int _coinsTarget;
        private float _coinsElapsed;
        private float _displayedAdded;
        private float _addedFrom;
        private int _addedTarget;
        private float _addedElapsed;
        private float _hideAddedAt;
        private bool _showingAdded;

        private void OnEnable() => Subscribe();
        private void Start() => Subscribe();

        private void Subscribe()
        {
            if (_subscribed) return;
            if (_coinsManager == null) _coinsManager = MN_Coins.Instance;
            if (_coinsManager == null) return;

            _coinsManager.OnCoinsUpdate += UpdateCoins;
            _coinsManager.OnAdd += AddCoins;
            _subscribed = true;
            _coinsTarget = _coinsManager.GetCoins();
            _displayedCoins = _coinsFrom = _coinsTarget;
            _coinsElapsed = _animationDuration;
            if (_coinsText != null) _coinsText.text = _coinsTarget.ToString();
            HideAdded();
        }

        private void OnDisable()
        {
            if (_subscribed && _coinsManager != null)
            {
                _coinsManager.OnCoinsUpdate -= UpdateCoins;
                _coinsManager.OnAdd -= AddCoins;
            }
            _subscribed = false;
            HideAdded();
        }

        private void UpdateCoins(int amount)
        {
            _coinsFrom = _displayedCoins;
            _coinsTarget = amount;
            _coinsElapsed = 0f;
        }

        private void AddCoins(int amount)
        {
            if (amount <= 0 || _addedCoinsText == null) return;

            if (!_showingAdded || Time.unscaledTime >= _hideAddedAt)
            {
                _addedTarget = 0;
                _displayedAdded = 0f;
            }

            _addedFrom = _displayedAdded;
            _addedTarget += amount;
            _addedElapsed = 0f;
            _hideAddedAt = Time.unscaledTime + Mathf.Max(_addedCoinsVisibleDuration, _animationDuration);
            _showingAdded = true;
            _addedCoinsText.text = "+" + Mathf.RoundToInt(_displayedAdded);
            _addedCoinsText.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!_subscribed) return;

            _coinsElapsed += Time.unscaledDeltaTime;
            _displayedCoins = Mathf.Lerp(_coinsFrom, _coinsTarget, Progress(_coinsElapsed));
            if (_coinsText != null) _coinsText.text = Mathf.RoundToInt(_displayedCoins).ToString();

            if (!_showingAdded) return;
            _addedElapsed += Time.unscaledDeltaTime;
            _displayedAdded = Mathf.Lerp(_addedFrom, _addedTarget, Progress(_addedElapsed));
            if (_addedCoinsText != null) _addedCoinsText.text = "+" + Mathf.RoundToInt(_displayedAdded);
            if (Time.unscaledTime >= _hideAddedAt) HideAdded();
        }

        private float Progress(float elapsed)
        {
            return _animationDuration <= 0f ? 1f : Mathf.Clamp01(elapsed / _animationDuration);
        }

        private void HideAdded()
        {
            _showingAdded = false;
            _addedTarget = 0;
            _displayedAdded = 0f;
            if (_addedCoinsText != null) _addedCoinsText.gameObject.SetActive(false);
        }
    }
}
