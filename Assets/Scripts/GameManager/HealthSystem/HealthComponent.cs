﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour
{
    public Canvas healthUI;
    public Slider healthSlider;
    public float maxHealth = 100f;
    private HealthSystem healthSystem;
    public float currentHealth;
    public float savedCurrentHealth;

    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        healthSystem = new HealthSystem(maxHealth);
        healthSystem.OnDeath += OnDeath;
        healthSystem.OnHealthChanged += UpdateHealthUI; //Lắng nghe sự kiện thay đổi máu
        currentHealth = healthSystem.currentHealth;
        UpdateHealthUI();
        savedCurrentHealth = currentHealth;
    
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public HealthSystem GetHealthSystem()
    {
        if (healthSystem == null)
        {
            Debug.LogError("HealthSystem is not initialized in HealthComponent!");
        }
        return healthSystem;
    }
    public void SetCurrentHealth(float value)
    {
        healthSystem.currentHealth = value;
        UpdateHealthUI();
    }
    public void TakeDamage(float amount)
    {
        if (!healthUI.enabled)
            healthUI.enabled = true;
        if (currentHealth > 0)
        {
            AudioManager.Instance.PlaySFX("hit");
            StartCoroutine(FlashRedEffect());
        }
        healthSystem.TakeDamage(amount);
    }
    private void OnDeath()
    {
        if (gameObject.CompareTag("Player"))
            gameObject.GetComponent<PlayerDeath>().KillPlayer();
        else if (gameObject.CompareTag("Robot"))
            gameObject.GetComponent<Robot>().OnRobotDestroyed();
        else if (gameObject.CompareTag("Enemy") || gameObject.GetComponent<Enemy>() != null)
            gameObject.GetComponent<Enemy>().KillEnemy();
        else
            Destroy(gameObject); // Có thể thay bằng hiệu ứng chết
    }
    public void Heal(float amount) => healthSystem.Heal(amount);
    public void AddShield(float amount) => healthSystem.AddShield(amount);
    public void SetInvincible(bool value) => healthSystem.SetInvincible(value, this);
    public void ApplyDamageOverTime(float amountPerSecond, float duration) => healthSystem.ApplyDamageOverTime(amountPerSecond, duration, this);
    public void ApplyRegeneration(float amount, float duration) => healthSystem.ApplyRegeneration(amount, duration, this);
    private void UpdateHealthUI()
    {
        if (healthSystem == null) return;
        healthSlider.value = healthSystem.currentHealth / healthSystem.maxHealth;
        currentHealth = healthSystem.currentHealth;
    }
    public bool isDead()
    {
        return healthSystem.IsDead();
    }

    #region Effect
    private IEnumerator FlashRedEffect()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }
    #endregion
}
