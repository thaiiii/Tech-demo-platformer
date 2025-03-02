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

    [Header("Low Gravity")]
    private Dictionary<Collider2D, Coroutine> activeEffects = new Dictionary<Collider2D, Coroutine>();
    private Dictionary<GameObject, float> originalGravityScales = new Dictionary<GameObject, float>();



    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthComponent health = collision.GetComponent<HealthComponent>();
        if (health == null) return;
        Coroutine effectCoroutine = null;

        //Debug.Log("smt");
        switch (hazardType)
        {
            case HazardType.Fire:
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
                ApplyIceEffect(collision.gameObject);
                break;
            case HazardType.LowGravity:
                //Set gravity = 0, không thể di chuyển, tiếp tục di chuyển với vận tốc cũ
                SetLowGravity(collision.gameObject);
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
            ResetIceEffect(collision.gameObject);
        }
        else if (hazardType == HazardType.LowGravity)
        {
            ResetGravity(collision.gameObject);
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
    {   //Mất tổng (value) máu trong vòng (duration) giây, reset nếu vẫn trong tầm ảnh hưởng
        while (health.GetHealthSystem().currentHealth > 0 && health != null)
        {
            health.ApplyDamageOverTime(effectValue / effectDuration, effectDuration);
            yield return new WaitForSeconds(effectDuration);
        }
    }
    private void ApplyIceEffect(GameObject gameObject)
    {
        if (gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<Player>().isSliding = true;
            gameObject.GetComponent<Player>().slipperyValue = effectValue;
        }
    }
    private void ResetIceEffect(GameObject gameObject)
    {
        if (gameObject.CompareTag("Player"))
            gameObject.GetComponent<Player>().isSliding = false;
    }
    private void SetLowGravity(GameObject gameObject)
    {
        if (gameObject.CompareTag("Player"))
        {
            Player player = gameObject.GetComponent<Player>();
            if (!originalGravityScales.ContainsKey(gameObject))
                originalGravityScales[gameObject] = player.environmentGravityScale;
            player.environmentGravityScale = effectValue;
            if (player.isGrounded)
            {
                //Cho phép di chuyển trên mặt đất
                player.UnlockMove(true);
            }
            else
                player.LockMove(true);

            // Lắng nghe sự kiện khi trạng thái grounded thay đổi
            player.OnGroundedChanged += OnPlayerGroundedChanged;
        }
        else
        {
            Debug.Log("Applying low gravity to object: " + gameObject.name);
            Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                if (!originalGravityScales.ContainsKey(gameObject))
                    originalGravityScales[gameObject] = rb.gravityScale;
                rb.gravityScale = effectValue;
                //Di chuyển với vận tốc cuối cùng, khóa chuyển động
            }
        }
    }
    private void ResetGravity(GameObject gameObject)
    {
        if (originalGravityScales.ContainsKey(gameObject))
        {
            if (gameObject.CompareTag("Player"))
            {
                Player player = gameObject.GetComponent<Player>();
                gameObject.GetComponent<Player>().environmentGravityScale = originalGravityScales[gameObject];
                gameObject.GetComponent<Player>().UnlockMove(false);
                player.OnGroundedChanged -= OnPlayerGroundedChanged; // Ngừng lắng nghe sự kiện grounded
            }
            else
            {
                Debug.Log("Resetting gravity for object: " + gameObject.name);
                Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.gravityScale = originalGravityScales[gameObject];
                    //Mở khóa di chuyển
                }
            }

            originalGravityScales.Remove(gameObject);
        }
    }
    private void OnPlayerGroundedChanged(Player player, bool isGrounded)
    {
        if (isGrounded)
        {
            // Nếu người chơi chạm đất khi đang trong zone
            player.UnlockMove(true); // Cho phép di chuyển bình thường
            player.allowKeepVelocity = true;
        }
        else
        {
            // Nếu người chơi rời khỏi mặt đất trong zone
            player.LockMove(true); // Khóa di chuyển
        }
    }
}
