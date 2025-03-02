using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour
{
    public Slider healthSlider;
    public float maxHealth = 100f;
    private HealthSystem healthSystem;
    public float currentHP;
    // Start is called before the first frame update
    void Awake()
    {
        healthSystem = new HealthSystem(maxHealth);
        healthSystem.OnDeath += OnDeath;
        healthSystem.OnHealthChanged += UpdateHealthUI; //Lắng ngh sự kiện thay đổi máu
        UpdateHealthUI();
    }

    public HealthSystem GetHealthSystem()
    {
        if (healthSystem == null)
        {
            Debug.LogError("HealthSystem is not initialized in HealthComponent!");
        }
        return healthSystem;
    }


    public void TakeDamage(float amount) => healthSystem.TakeDamage(amount);
    private void OnDeath()
    {
        Debug.Log($"{gameObject.name} has died!");
        if (gameObject.CompareTag("Player"))
            gameObject.GetComponent<PlayerDeath>().KillPlayer();
        else if (gameObject.CompareTag("Robot"))
            gameObject.GetComponent<Robot>().OnRobotDestroyed();
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
        currentHP = healthSlider.value;
    }
    public bool isDead()
    {
        return healthSystem.IsDead();
    }
}
