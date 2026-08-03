using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    [SerializeField] private HealthBasicsEvents healthObject;
    [SerializeField] private Slider slider;
    [SerializeField] private bool disableGameObjectSliderOnBlock;
    [SerializeField] private bool disableGameObjectSliderOnDeath;
    [SerializeField] private bool enableGameObjectSliderOnRespawn;

    private void Awake()
    {
        if (healthObject == null)
            healthObject.GetComponent<HealthBasicsEvents>();

        healthObject.OnHealthUpdate += HealthEvents_OnNewValue;
        healthObject.OnRespawn += HealthEvents_OnNewValue;
        healthObject.OnMaxHealthValueUpdated += MaxHealthUpdated;

        if (disableGameObjectSliderOnBlock)
            healthObject.OnBlockUpdate += BlockedUpdate;

        if (disableGameObjectSliderOnDeath)
            healthObject.OnDeath += OnDeath;

        if (enableGameObjectSliderOnRespawn)
            healthObject.OnRespawn += OnRespawn;

        slider.maxValue = healthObject.GetMaxHealth();
        slider.value = healthObject.GetMaxHealth();
    }

    private void Start()
    {
        BlockedUpdate(healthObject.IsBlocked());
    }

    private void BlockedUpdate(bool isBlocked)=> slider.gameObject.SetActive(!isBlocked);

    private void OnDeath()=> slider.gameObject.SetActive(false);

    private void OnRespawn() => slider.gameObject.SetActive(true);

    private void MaxHealthUpdated(float newValue)
    {
        slider.maxValue = newValue;
        slider.value = healthObject.GetCurrentHealth();
    }

    private void HealthEvents_OnNewValue(float currentHealth)
    {
        slider.value = currentHealth;
    }
    private void HealthEvents_OnNewValue()
    {
        slider.value = healthObject.GetCurrentHealth();
    }
}
