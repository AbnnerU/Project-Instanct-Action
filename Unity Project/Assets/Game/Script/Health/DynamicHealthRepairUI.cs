using Scripts.Economy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DynamicHealthRepairUI : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private HealthManager _healthManager;
    [SerializeField] private Button _repairButton;
    [SerializeField] private TMP_Text _repairCostText;

    [Header("Base price")]
    [SerializeField] private int _minRepairCost = 10;
    [SerializeField] private int _maxRepairCost = 100;
    [Range(0.01f, 0.99f)]
    [SerializeField] private float _maxCostHealthThreshold = 0.3f;
    [Header("Revive")]
    [SerializeField] private bool _overrideReviveCost = true;
    [SerializeField] private int _reviveCost = 150;
    [Header("Price Colors")]
    [SerializeField] private Color _expensiveColor = Color.red;
    [SerializeField] private Color _cheapColor = Color.green;

    private Color _defaultColor;

    private float _currentInflation = 1f;
    private int _currentBaseCost = 0;

    private void Awake()
    {
        if (_repairButton != null)
            _repairButton.onClick.AddListener(TryRepair);

        if (_repairCostText != null)
            _defaultColor = _repairCostText.color;

        if (_healthManager != null)
        {
            _healthManager.OnHealthUpdate += HandleHealthChanged;
            _healthManager.OnDeath += UpdateUI;
            _healthManager.OnRespawn += UpdateUI;
        }
    }

    private void Start()
    {
        if (MN_Coins.Instance != null)
        {
            MN_Coins.Instance.OnInflationUpdate += HandleInflationChanged;
            MN_Coins.Instance.OnCoinsUpdate += HandleCoinsChanged;

            _currentInflation = MN_Coins.Instance.GetInflation();
        }

        UpdateUI();
    }

    private void OnDestroy()
    {
        if (_healthManager != null)
        {
            _healthManager.OnHealthUpdate -= HandleHealthChanged;
            _healthManager.OnDeath -= UpdateUI;
            _healthManager.OnRespawn -= UpdateUI;
        }

        if (MN_Coins.Instance != null)
        {
            MN_Coins.Instance.OnInflationUpdate -= HandleInflationChanged;
            MN_Coins.Instance.OnCoinsUpdate -= HandleCoinsChanged;
        }
    }


    #region Events
    private void HandleHealthChanged(float currentHealth) => UpdateUI();
    private void HandleCoinsChanged(int currentCoins) => UpdateButtonState();
    private void HandleInflationChanged(float inflation)
    {
        _currentInflation = inflation;
        UpdateUI();
    }
    #endregion


    private void UpdateUI()
    {
        if (_healthManager == null) return;

        _currentBaseCost = CalculateBaseCost();

        if (_repairCostText != null)
        {
            int displayValue = Mathf.RoundToInt(_currentBaseCost * _currentInflation);
            _repairCostText.text = displayValue.ToString();

            if (_currentInflation > 1f)
                _repairCostText.color = _expensiveColor;
            else if (_currentInflation < 1f)
                _repairCostText.color = _cheapColor;
            else
                _repairCostText.color = _defaultColor;
        }

        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        bool isFullHealth = _healthManager.ItIsAtMaximumHealthValue();
        bool canAfford = MN_Coins.Instance != null && MN_Coins.Instance.CanPay(_currentBaseCost);

        _repairButton.interactable = !isFullHealth && canAfford;
    }

    private int CalculateBaseCost()
    {
        if (!_healthManager.IsAlive() && _overrideReviveCost)
            return _reviveCost;

        float maxHealth = _healthManager.GetMaxHealth();
        float currentHealth = _healthManager.GetCurrentHealth();

        if (maxHealth <= 0) return 0;

        float healthPercent = currentHealth / maxHealth;

        if (healthPercent >= 1f) return 0;

        if (healthPercent <= _maxCostHealthThreshold)
            return _maxRepairCost;

        // InverseLerp nos dá um valor de 0 a 1 dizendo "onde" a porcentagem atual está entre os dois pontos
        float t = Mathf.InverseLerp(1f, _maxCostHealthThreshold, healthPercent);

        return Mathf.RoundToInt(Mathf.Lerp(_minRepairCost, _maxRepairCost, t));
    }


    private void TryRepair()
    {
        if (MN_Coins.Instance != null && MN_Coins.Instance.CanPay(_currentBaseCost))
        {
            MN_Coins.Instance.RemoveCoins(_currentBaseCost);

            _healthManager.ResetHealth();

            UpdateUI();
        }
    }
}
