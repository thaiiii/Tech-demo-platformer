using UnityEngine;

public class Spike : MonoBehaviour
{
    public delegate void SpikeDestroyedHandler();
    public event SpikeDestroyedHandler OnSpikeDestroyed;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Gọi sự kiện hủy gai
        OnSpikeDestroyed?.Invoke();
        Destroy(gameObject); // Hủy gai
    }
}
