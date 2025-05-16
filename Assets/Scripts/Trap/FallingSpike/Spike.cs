using System.Collections;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public delegate void SpikeDestroyedHandler();
    public event SpikeDestroyedHandler OnSpikeDestroyed;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Gọi sự kiện hủy gai
        OnSpikeDestroyed?.Invoke();
        StartCoroutine(DelayDestroy());
    }
    
    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject); // Hủy gai
    }
}
