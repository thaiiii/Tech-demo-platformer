using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 10f;
    public float turnSpeed = 5f; // tốc độ bẻ hướng, có thể điều chỉnh được
    

    public Transform target;

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
            Debug.Log("Kill enemy");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
