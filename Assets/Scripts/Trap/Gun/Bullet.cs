using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu đạn chạm vào người chơi
        if (!collision.CompareTag("Player"))
        {
            Debug.Log("Hit " + collision.tag);
            Destroy(gameObject);
        }
    }
}
