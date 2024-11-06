using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;

    [SerializeField]
    public float horizontalValue;
    public float speed = 0f;

    [Header("Ground")]
    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] float groundCheckRadius = 0.01f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool isJumped;
    [SerializeField] float jumpPower;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandlePCInput();
    }

    private void FixedUpdate()
    {
        GroundCheck();
        Move();
    }

    private void HandlePCInput()
    {
        horizontalValue = Input.GetAxisRaw("Horizontal");
        //if press Jump button, enable bool jump, othwewise turn it off
        if (Input.GetButtonDown("Jump"))
        {
            //Add jump force if available
            if (!isJumped && isGrounded)
            {
                rb.velocity = Vector2.up * jumpPower;
            }
            // Mark that you jumped
            isJumped = true;
        }
    }

    private void Move()
    {
        Vector2 targetVelocity = new Vector2(horizontalValue * speed * 100 * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = targetVelocity;

        //Current scale
        Vector3 currentScale = transform.localScale;
        //Facing direction
        if (horizontalValue * currentScale.x < 0)
            currentScale.x = currentScale.x * -1;
        transform.localScale = currentScale;

        //0 for idle, 6 for walk, 12 for run => Setting xVelocity in Player animator
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));

    }

    private void GroundCheck()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        if (collider.Length > 0)
        {
            isGrounded = true;
            isJumped = false;
            //Check if ground has a tag moving platform
            foreach (var c in collider)
            {
                Debug.Log("ok");
                if (c.tag == "MovingPlatform")
                    transform.parent = c.transform;
            }

        }
        else
        {
            //Un-parent the transform
            transform.parent = null;

            isGrounded = false;
            isJumped = true;
        }

        //animator.SetBool("Jump", !isGrounded);
    }

    

}
