using System.Collections;
using UnityEngine;

public class OnewayPlatform : MonoBehaviour
{
    private Collider2D platformCollider; // Collider của Tilemap
    private Collider2D playerCollider; // Collider của người chơi
    public float fallDuration = 0.3f; // Thời gian để vô hiệu hóa va chạm

    private void Awake()
    {
        platformCollider = GetComponent<CompositeCollider2D>();
    }

    private void Update()
    {
        HandlePlatformInteraction();
    }

    private void HandlePlatformInteraction()
    {
        if (Input.GetKeyDown(KeyCode.S) && IsPlayerAbovePlatform())
        {
            playerCollider = FindPlayerAbovePlatform();
            if (playerCollider != null)
            {
                StartCoroutine(AllowPlayerToFall());
            }
        }
    }

    private bool IsPlayerAbovePlatform()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            platformCollider.bounds.center,
            platformCollider.bounds.size,
            0f
        );

        foreach (var col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                return true; // Người chơi đang đứng trên Tilemap
            }
        }
        return false;
    }

    private Collider2D FindPlayerAbovePlatform()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            platformCollider.bounds.center,
            platformCollider.bounds.size,
            0f
        );

        foreach (var col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                return col; // Trả về Collider của người chơi
            }
        }
        return null;
    }

    private IEnumerator AllowPlayerToFall()
    {
        // Tạm thời vô hiệu hóa va chạm giữa người chơi và platform
        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);

        yield return new WaitForSeconds(fallDuration);

        // Kích hoạt lại va chạm
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }

    private void OnDrawGizmosSelected()
    {
        // Hiển thị vùng kiểm tra trong Scene View để debug
        Gizmos.color = Color.yellow;
        if (platformCollider != null)
        {
            Gizmos.DrawWireCube(platformCollider.bounds.center, platformCollider.bounds.size);
        }
    }
}
