using System.Collections.Generic;
using UnityEngine;

public class TransportPlatform : MonoBehaviour
{
    public List<string> applicableTags;
    private Vector3 lastPosition;
    private List<Rigidbody2D> carriedBodies = new List<Rigidbody2D>();

    void Start()
    {
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 delta = transform.position - lastPosition;

        foreach (var rb in carriedBodies)
        {
            if (rb != null)
            {
                rb.position += (Vector2)delta; // Cộng delta vào vị trí thực
            }
        }

        lastPosition = transform.position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
        {
            Rigidbody2D rb = collision.collider.attachedRigidbody;
            if (rb != null && !carriedBodies.Contains(rb))
            {
                carriedBodies.Add(rb);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.collider.attachedRigidbody;
        if (rb != null)
        {
            carriedBodies.Remove(rb);
        }
    }
}
