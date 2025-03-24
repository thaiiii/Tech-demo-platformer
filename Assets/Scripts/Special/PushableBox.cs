using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBox : MonoBehaviour
{
    public Vector3 savedPosition;
    public bool isInLowGravityZone = false;
    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        savedPosition = transform.position;
    }
    private void Update()
    {
        if (isInLowGravityZone)
            return;
    }

    public void LoadStartPosition()
    {
        rb.velocity = Vector2.zero;
        transform.position = savedPosition;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().isTrigger = false;
        GetComponent<Rigidbody2D>().isKinematic = false;
    }
    
}
