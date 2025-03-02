using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem
{
    public float maxHealth { get; private set; }
    public float currentHealth { get; private set; }
    public float shield { get; set; } //Giap de chan damage
    public bool isInvincible { get; set; } // Bat tu

    //Su kien
    public event Action OnHealthChanged;
    public event Action OnDeath;

    public HealthSystem(float maxHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
        this.shield = 0;
        this.isInvincible = false;
    }

    public bool IsDead() => currentHealth <= 0;
    public void TakeDamage(float amount)
    {
        if (isInvincible)
            return;
        if (amount < 0)
            return;
        if (shield > 0)
        {
            float remainingDamage = Mathf.Max(0, amount - shield);
            shield -= amount;
            if (shield < 0) shield = 0;
            amount = remainingDamage;
        }
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke();
        if (currentHealth <= 0)
            OnDeath?.Invoke();
    }
    public void Heal(float amount)
    {
        if (amount <= 0)
            return;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke();
    }
    public void AddShield(float amount)
    {
        if (amount <= 0)
            return;
        shield += amount;
        OnHealthChanged?.Invoke();
    }
    public void SetInvincible(bool value, HealthComponent owner)
    {
        isInvincible = value;
    }
    public void ApplyDamageOverTime(float amountPerSecond, float duration, HealthComponent owner)
    {
        owner.StartCoroutine(DamageOverTimeCoroutine(amountPerSecond, duration));
    }
    private IEnumerator DamageOverTimeCoroutine(float amountPerSecond, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            if (IsDead())
                break;
            TakeDamage(amountPerSecond);
            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }
    public void ApplyRegeneration(float amountPerSecond, float duration, HealthComponent owner)
    {
        owner.StartCoroutine(RegenerationCoroutine(amountPerSecond, duration));
    }
    private IEnumerator RegenerationCoroutine(float amountPerSecond, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            Heal(amountPerSecond);
            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }
    }
}
