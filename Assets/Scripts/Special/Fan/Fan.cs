using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fan : MonoBehaviour
{
    [Header("Fan Settings")]
    public List<string> affectedTags;
    public Vector3 forceDirection = Vector3.up;
    public float baseForce = 5f;
    public bool isFanActivated = false;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (!isFanActivated)
            DisableFan();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isFanActivated && affectedTags.Contains(collision.tag))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float requiredForce = rb.mass * Physics2D.gravity.magnitude * rb.gravityScale;
                rb.AddForce(forceDirection.normalized * (baseForce + requiredForce), ForceMode2D.Force);
            }
        }
    }

    public void EnableFan()
    {
        isFanActivated = true;
        animator.SetBool("isFanActivated", true);
    }

    public void DisableFan()
    {
        isFanActivated = false;
        animator.SetBool("isFanActivated", false);
    }

    public void ResetFan()
    {
            DisableFan();
    }

}
