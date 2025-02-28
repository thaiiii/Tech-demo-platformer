using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HazardZone;

public class HazardZone : MonoBehaviour
{
    public enum HazardType
    {
        Fire,
        ToxicGas,
        Lava,
        Ice,
        LowGravity
    }
    protected HazardType hazardType;
    public float effectValue; //Lửa và khí độc
    public float effectDuration; //Băng và ko tluc
    private Dictionary<Collider2D, Coroutine> activeEffects = new Dictionary<Collider2D, Coroutine>();



    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthComponent health = collision.GetComponent<HealthComponent>();
        if (health == null) return;
        Coroutine effectCoroutine = null;

        switch (hazardType)
        {
            case HazardType.Fire:
                Debug.Log("on fire");
                effectCoroutine = StartCoroutine(ApplyDamageOverTime(health));
                break;
            case HazardType.ToxicGas:
                effectCoroutine = StartCoroutine(ApplyPoisonEffect(health));
                break;
            case HazardType.Lava:
                health.TakeDamage(health.maxHealth); // Chết ngay lập tức
                break;
            case HazardType.Ice:
                //Giảm ma sát
                effectCoroutine = StartCoroutine(ApplySlipperyEffect(collision.gameObject));
                break;
            case HazardType.LowGravity:
                //Set gravity = 0, không thể di chuyển, tiếp tục di chuyển với vận tốc cũ
                effectCoroutine = StartCoroutine(SetLowGravity(collision.gameObject));
                break;
            default:
                break;
        }
        if (effectCoroutine != null)
            activeEffects[collision] = effectCoroutine;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (activeEffects.ContainsKey(collision))
        {
            StopCoroutine(activeEffects[collision]);
            activeEffects.Remove(collision);
        }

        // Khôi phục trạng thái ban đầu nếu cần
        if (hazardType == HazardType.Ice)
        {
            //ResetFriction(collision);
        }
        else if (hazardType == HazardType.LowGravity)
        {
            //ResetGravity(collision);
        }
    }

    private IEnumerator ApplyDamageOverTime(HealthComponent health)
    {
        //Mất 1 lượng máu mỗi giây cho đến khi rời khỏi
        while (health.GetHealthSystem().currentHealth > 0 && health != null)
        {
            health.TakeDamage(effectValue);
            yield return new WaitForSeconds(effectDuration);
        }
    }
    private IEnumerator ApplyPoisonEffect(HealthComponent health)
    {   //Mất tổng (value) máu mỗi (duration) giây, reset nếu vẫn trong tầm ảnh hưởng
        while (health.GetHealthSystem().currentHealth > 0 && health != null)
        {
            health.ApplyDamageOverTime(effectValue / effectDuration, effectDuration);
            yield return new WaitForSeconds(effectDuration);
        }
    }
    private IEnumerator ApplySlipperyEffect(GameObject gameObject)
    {
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        float originalFriction = rb.sharedMaterial.friction;
        rb.sharedMaterial.friction = 0f; //Giảm ma sát để trượt
        yield return new WaitForSeconds(1f);
        rb.sharedMaterial.friction = originalFriction;
    }
    private IEnumerator SetLowGravity(GameObject gameObject)
    {
        Debug.Log("set ");
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        float originalGravityScale = rb.gravityScale;
        gameObject.GetComponent<Player>().evironmentGravityScale = effectValue;
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<Player>().evironmentGravityScale = originalGravityScale;
    }
}
