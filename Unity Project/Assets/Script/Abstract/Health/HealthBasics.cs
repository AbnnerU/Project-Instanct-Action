using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public class HealthUnityEvent : UnityEvent { }

#region Main
public abstract class HealthBasicsEvents : HealthEvents
{
}

public abstract class HealthBasics : MonoBehaviour
{
    [SerializeField] protected bool _isBlocked;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected bool isHittable;
    [SerializeField] protected bool isKillable;

    protected float currentHealth;

    protected bool isAlive = true;

    public void SetHittable(bool canHit)
    {
        this.isHittable = canHit;
    }

    public virtual void SetBloked(bool isBlocked)
    {
        _isBlocked = isBlocked;
    }

    public bool IsBlocked()
    {
        return _isBlocked;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public virtual void SetMaxHealthValue(float maxHealth, bool resetCurrentHealth)
    {
        this.maxHealth = maxHealth;

        if (resetCurrentHealth)
            currentHealth = maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool IsMaxHealth()
    {
        return currentHealth >= maxHealth;
    }
}

public abstract class HealthEvents : HealthBasics
{
    public Action OnDeath;
    public Action OnRespawn;
    public Action<float> OnHealthUpdate;
    public Action<float> OnMaxHealthValueUpdated;
    public Action<bool> OnBlockUpdate;

    public override void SetMaxHealthValue(float maxHealth, bool resetCurrentHealth)
    {
        base.SetMaxHealthValue(maxHealth, resetCurrentHealth);

        OnMaxHealthValueUpdated?.Invoke(maxHealth);
    }

    public override void SetBloked(bool isBlocked)
    {
        base.SetBloked(isBlocked);

        OnBlockUpdate?.Invoke(isBlocked);
    }
}

#endregion


#region AdditionalClasses
public abstract class OnDeathActionBase : MonoBehaviour
{
    [SerializeField] protected HealthBasicsEvents _healthEventsComponent;
    [SerializeField] protected bool _verifyStateOnStart = true;

    protected virtual void Awake()
    {
        if (_healthEventsComponent == null)
            TryGetComponent<HealthBasicsEvents>(out _healthEventsComponent);


        if (_healthEventsComponent)
            _healthEventsComponent.OnDeath += HealthEvents_OnDeath;
    }

    protected virtual void Start()
    {
        if (_verifyStateOnStart)
        {
            if (_healthEventsComponent != null && _healthEventsComponent.IsAlive() == false)
                ExecuteDeathAction();
        }
    }

    protected virtual void OnDestroy()
    {
        if (_healthEventsComponent)
            _healthEventsComponent.OnDeath -= HealthEvents_OnDeath;
    }

    protected abstract void HealthEvents_OnDeath();

    public void ExecuteDeathAction() => HealthEvents_OnDeath();
}

public abstract class LifeCycleActionsBase : OnDeathActionBase
{
    protected override void Awake()
    {
        if (_healthEventsComponent == null)
            TryGetComponent<HealthBasicsEvents>(out _healthEventsComponent);


        if (_healthEventsComponent)
        {
            _healthEventsComponent.OnDeath += HealthEvents_OnDeath;
            _healthEventsComponent.OnRespawn += HealthEvents_OnRespawn;
        }
    }

    protected override void OnDestroy()
    {
        if (_healthEventsComponent)
        {
            _healthEventsComponent.OnDeath -= HealthEvents_OnDeath;
            _healthEventsComponent.OnRespawn -= HealthEvents_OnRespawn;
        }
    }

    protected abstract void HealthEvents_OnRespawn();

    public void ExecuteRespawnAction() => HealthEvents_OnRespawn();
}
#endregion


#region Interface
public interface IHittable
{
    public event Action<int> OnHitted;

    void ResetHealth();

    void Hit(float damage);

    void Hit(float damage, bool triggerDamageEvent);

    void Hit(float damage, out bool killed);

    bool IsAlive();

    void Kill();
}

public interface IHealable
{
    public event Action OnHeal;

    void Heal(float healValue);

    bool ItIsAtMaximumHealthValue();
}

public interface ICanBeKilled
{
    public event Action OnDeath;
}
#endregion