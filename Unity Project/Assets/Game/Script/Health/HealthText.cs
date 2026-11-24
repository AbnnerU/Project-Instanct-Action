using TMPro;
using UnityEngine;

public class HealthText : MonoBehaviour
{
    [SerializeField] private HealthBasicsEvents healthObject;
    [SerializeField] private TMP_Text _maxHealthTextComponent;
    [SerializeField] private TMP_Text _currentHealthTextComponent;
    [SerializeField] private string _numberFormat = "0";

    private void Awake()
    {
        if (healthObject == null)
            healthObject.GetComponent<HealthBasicsEvents>();

        healthObject.OnHealthUpdate += HealthEvents_OnNewValue;
        healthObject.OnRespawn += HealthEvents_OnNewValue;
        healthObject.OnMaxHealthValueUpdated += MaxHealthUpdated;
    }

    private void MaxHealthUpdated(float newValue)
    {
        _maxHealthTextComponent.text = newValue.ToString(_numberFormat);
    }

    private void HealthEvents_OnNewValue(float currentHealth)
    {
       _currentHealthTextComponent.text = currentHealth.ToString(_numberFormat);
    }
    private void HealthEvents_OnNewValue()
    {
        _currentHealthTextComponent.text = healthObject.GetCurrentHealth().ToString(_numberFormat);
    }
}
