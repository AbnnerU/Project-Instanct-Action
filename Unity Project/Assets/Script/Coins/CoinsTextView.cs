using TMPro;
using UnityEngine;

namespace Scripts.Economy
{
    public class CoinsTextView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinsText;
        private MN_Coins _coinsManager;

        private void Awake()
        {
            if (_coinsText == null) TryGetComponent(out _coinsText);
        }

        private void Start()
        {
            _coinsManager = MN_Coins.Instance;
            if (_coinsManager == null) return;

            _coinsManager.OnCoinsUpdate += UpdateText;
            UpdateText(_coinsManager.GetCoins());
        }

        private void OnDestroy()
        {
            if (_coinsManager != null) _coinsManager.OnCoinsUpdate -= UpdateText;
        }

        private void UpdateText(int coins)
        {
            if (_coinsText != null) _coinsText.text = coins.ToString();
        }
    }
}
