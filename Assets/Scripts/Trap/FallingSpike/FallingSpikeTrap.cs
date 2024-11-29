using System.Collections;
using UnityEngine;

public class FallingSpikeTrap : MonoBehaviour
{
    public GameObject spikePrefab; // Prefab của gai
    public Transform initialPoint; // Điểm xuất hiện của gai
    public float respawnDelay = 5f; // Thời gian chờ để spawn gai mới
    public float raycastLength = 20f;
    public LayerMask obstacleLayer;

    private GameObject currentSpike; // Gai hiện tại đang hoạt động

    private void Start()
    {
        SpawnSpike(); // Tạo gai đầu tiên
    }

    private void Update()
    {
        if (currentSpike == null) return;

        // Raycast phía dưới gai
        RaycastHit2D hit = Physics2D.Raycast(currentSpike.transform.position + Vector3.down, Vector2.down, raycastLength, obstacleLayer);
        Debug.DrawLine(currentSpike.transform.position + Vector3.down, currentSpike.transform.position + Vector3.down + Vector3.down * raycastLength, Color.red);
        
        // Kích hoạt khi phát hiện người chơi
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            ActivateFallingTrap();
        }
    }

    private void ActivateFallingTrap()
    {
        if (currentSpike == null) return;

        var rb = currentSpike.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false; // Cho gai rơi
        }
    }

    private void SpawnSpike()
    {
        // Tạo mới gai tại vị trí initialPoint
        currentSpike = Instantiate(spikePrefab, initialPoint.position, Quaternion.identity);
        currentSpike.GetComponent<Rigidbody2D>().isKinematic = true;
        currentSpike.transform.SetParent(transform, true); 

        // Đăng ký sự kiện hủy gai
        var spikeScript = currentSpike.GetComponent<Spike>();
        if (spikeScript != null)
        {
            spikeScript.OnSpikeDestroyed += HandleSpikeDestroyed;
        }
    }

    private void HandleSpikeDestroyed()
    {
        // Xóa tham chiếu đến gai hiện tại
        currentSpike = null;

        // Hồi lại gai mới sau delay
        StartCoroutine(RespawnSpike());
    }

    private IEnumerator RespawnSpike()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnSpike();
    }
}
