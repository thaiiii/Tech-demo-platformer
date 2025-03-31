using System.Collections;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 12f;
    public float turnSpeed = 3f; // tốc độ bẻ hướng, có thể điều chỉnh được
    private CameraFollow cameraFollow;

    public Transform target;

    private void Start()
    {
        cameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
    }
    void Update()
    {
        if (target != null)
        {
            ChaseTarget();
        }
    }

    public void SetTarget(Transform player)
    {
        target = player;
    }

    void ChaseTarget()
    {
        // Tính toán hướng di chuyển đến người chơi với tốc độ bẻ hướng chậm
        Vector2 direction = (target.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * turnSpeed);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Di chuyển tên lửa tới người chơi
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra va chạm với vật cản
        if (collision.CompareTag("Ignore") || collision.CompareTag("HazardZone"))
        {
            return;
        }
        else if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().KillEnemy();
        }
        else if (collision.CompareTag("SlimeClone"))
        {
            collision.gameObject.GetComponent<SlimeClone>().KillClone();
        }
        else if (collision.gameObject.CompareTag("Robot"))
        {
            collision.gameObject.GetComponent<HealthComponent>().TakeDamage(collision.gameObject.GetComponent<HealthComponent>().maxHealth / 2f);

        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerDeath>().KillPlayer();
        }

        AudioManager.Instance.PlaySFX("explosion");
        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(10f, 0.25f); // Độ rung: 3, thời gian: 0.25 giây
        Destroy(gameObject);
    }
    
}
