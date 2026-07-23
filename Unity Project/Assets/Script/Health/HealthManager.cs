using System;
using UnityEngine;
using Scripts.PoolManager;

public class HealthManager : HealthBasicsEvents, IHittable, IHealable
{
    public event Action<int> OnHitted;
    public event Action OnHeal;

    [SerializeField] private bool releaseOnDeath;
    [SerializeField] private bool resetHealthOnEnable;

    private void Awake()
    {
        currentHealth = maxHealth;
        isAlive = true;
    }

    public void Hit(float damage, out bool killed)
    {
        killed = false;

        if (_isBlocked) return;

        if (isHittable)
        {
            currentHealth -= damage;

            OnHitted?.Invoke((int)damage);

            OnHealthUpdate?.Invoke(currentHealth);

            if (currentHealth <= 0 && isAlive)
            {
                killed = true;

                currentHealth = 0;

                OnHealthUpdate?.Invoke(currentHealth);

                if (isKillable)
                    Kill();
            }
        }
    }

    public void Hit(float damage)
    {
        if (_isBlocked) return;

        if (isHittable && isAlive)
        {
            currentHealth -= damage;

            OnHitted?.Invoke((int)damage);

            OnHealthUpdate?.Invoke(currentHealth);

            if (currentHealth <= 0 && isAlive)
            {
                currentHealth = 0;

                OnHealthUpdate?.Invoke(currentHealth);

                if (isKillable)
                    Kill();
            }
        }
    }

    public void Hit(float damage, bool triggerDamageEvent)
    {
        if (_isBlocked) return;

        if (isHittable)
        {
            if (currentHealth == 0)
                return;

            currentHealth -= damage;

            if (triggerDamageEvent)
                OnHitted?.Invoke((int)damage);

            OnHealthUpdate?.Invoke(currentHealth);

            if (currentHealth <= 0 && isAlive)
            {
                currentHealth = 0;

                OnHealthUpdate?.Invoke(currentHealth);

                if (isKillable)
                    Kill();
            }
        }
    }

    public void Heal(float healValue)
    {
        if (_isBlocked) return;

        if (isAlive)
        {
            currentHealth += healValue;

            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            OnHeal?.Invoke();

            OnHealthUpdate?.Invoke(currentHealth);
        }
    }

    public bool ItIsAtMaximumHealthValue()
    {
        return currentHealth >= maxHealth;
    }

    public void Kill()
    {
        if (_isBlocked) return;

        if (isAlive)
        {
            if (releaseOnDeath)
                PoolManager.ReleaseObject(gameObject);

            isAlive = false;

            OnDeath?.Invoke();
        }
    }

    public void ResetHealth()
    {
        isAlive = true;

        currentHealth = maxHealth;

        OnRespawn?.Invoke();
    }

    public float GetCurrentHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    private void OnEnable()
    {
        if (resetHealthOnEnable)
            ResetHealth();
    }

    public void SetCurrentHealthValue(float newValue)
    {
        currentHealth = Mathf.Clamp(newValue, 0, maxHealth);

        if (currentHealth <= 0)
            isAlive = false;

        OnHealthUpdate?.Invoke(currentHealth);
    }
}
