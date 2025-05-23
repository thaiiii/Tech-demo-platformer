﻿using System.Collections;
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
    public Vector3 savedPosition;

    private Animator animator;

    private void Awake()
    {
        savedPosition = transform.position;
        animator = GetComponent<Animator>();
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
        if (!isFanActivated)
            AudioManager.Instance.PlayStopableSFX("fan");
        isFanActivated = true;
        animator.SetBool("isFanActivated", true);
    }

    public void DisableFan()
    {
        isFanActivated = false;
        animator.SetBool("isFanActivated", false);
        AudioManager.Instance.StopSFX("fan");
    }

    public void ResetFan()
    {
        transform.position = savedPosition;    
        DisableFan();
    }

}
