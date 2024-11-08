using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SawBlade : MonoBehaviour
{
    Animator animator;

    [Header("Custom")]
    public float rotationSpeed; // toc do quay
    public bool rotateClockwise = true; //Chieu kim dong ho
    public float size;

    private void Start()
    {
        animator = GetComponent<Animator>();
        transform.localScale = new Vector3 (size, size, size);
    }


    // Update is called once per frame
    void Update()
    {
        int direction = rotateClockwise ? -1 : 1;
        transform.Rotate(0, 0, rotationSpeed * direction * Time.deltaTime);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Cut");
        }
    }
}
