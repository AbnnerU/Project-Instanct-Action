using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Progression
{
    public class XpBar : MonoBehaviour
    {
        [SerializeField] private Slider _sliderComponent;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private string _levelPrefix = "Lv. ";

        private MN_Xp _playerXpManager;

        private void Awake()
        {
            if (_sliderComponent == null)
                TryGetComponent(out _sliderComponent);

            if (_levelText == null)
                _levelText = GetComponentInChildren<TMP_Text>();

            if (_sliderComponent != null)
                _sliderComponent.maxValue = 1f;
        }

        private void Start()
        {
            _playerXpManager = MN_Xp.Instance;

            if (_playerXpManager == null)
                return;

            _playerXpManager.OnXpUpdated += PlayerXpManager_OnXpUpdated;
            _playerXpManager.OnLevelUp += PlayerXpManager_OnLevelUp;

            UpdateXp(_playerXpManager.GetCurrentXpPercentage());
            UpdateLevelText(_playerXpManager.GetCurrentLevel());
        }

        private void OnDestroy()
        {
            if (_playerXpManager == null)
                return;

            _playerXpManager.OnXpUpdated -= PlayerXpManager_OnXpUpdated;
            _playerXpManager.OnLevelUp -= PlayerXpManager_OnLevelUp;
        }

        private void PlayerXpManager_OnXpUpdated(float xpPercentage, float currentXp)
        {
            UpdateXp(xpPercentage);
        }

        private void PlayerXpManager_OnLevelUp(int currentLevel)
        {
            UpdateLevelText(currentLevel);
        }

        private void UpdateXp(float xpPercentage)
        {
            if (_sliderComponent == null)
                return;

            _sliderComponent.maxValue = 1f;
            _sliderComponent.value = Mathf.Clamp01(xpPercentage);
        }

        private void UpdateLevelText(int currentLevel)
        {
            if (_levelText == null)
                return;

            _levelText.text = $"{_levelPrefix}{currentLevel}";
        }
    }
}
