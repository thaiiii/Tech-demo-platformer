using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool isActivated = false;
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().startPosition = transform.position + Vector3.up * 5; 
            isActivated = true;
            animator.SetBool("isActivated", isActivated);
        }
    }
}
