using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    private GameTimer gameTimer;
    private Vector3 startPosition;


    [Header("Movement")]
    public float horizontalValue;
    public float speed = 0f;
    public bool hasMoved = false; //Kiem tra xem da di chuyen chua
    public bool isMoveable = true;

    [Header("Wall")]
    public float wallSlideSpeed = 1f;
    public LayerMask normalWallLayer; //normal wall block
    public LayerMask glassWallLayer; //glass wall block
    public bool isTouchingNormalWall = false;
    public bool isTouchingGlassWall = false;
    public float checkWallRadius;

    [Header("Ground")]
    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] float groundCheckRadius = 0.01f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool isJumped;
    [SerializeField] bool coyoteJump;
    [SerializeField] float jumpPower;




    #region Default
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameTimer = FindAnyObjectByType<GameTimer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoveable)
            HandlePCInput();
    }

    private void FixedUpdate()
    {
        GroundCheck();
        Move();
    }
    #endregion

    #region Movement

    private void HandlePCInput()
    {
        horizontalValue = Input.GetAxisRaw("Horizontal");

        //if move first time, hasMoved = true
        if(!hasMoved && Mathf.Abs(horizontalValue) > 0) {
            hasMoved = true;
            gameTimer.StartTimer();
        }

        // Kiểm tra xem người chơi có chạm vào block thường hay block kính
        isTouchingNormalWall = Physics2D.OverlapCircle(transform.position, checkWallRadius, normalWallLayer);
        isTouchingGlassWall = Physics2D.OverlapCircle(transform.position, checkWallRadius, glassWallLayer);
        if (Input.GetButton("Jump") && (isTouchingNormalWall || isTouchingGlassWall))
        {
            // Tạm thời vô hiệu hóa trọng lực khi giữ Space
            rb.gravityScale = 0;

            if (isTouchingNormalWall)
            {
                // Dừng trượt nếu chạm vào block thường
                rb.velocity = new Vector2(rb.velocity.x, 0);
                isJumped = false;
            }
            else if (isTouchingGlassWall)
            {
                // Trượt chậm nếu chạm vào block kính
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
                isJumped = false;   
            }

            // Sử dụng Raycast để phát hiện hướng của tường
            RaycastHit2D wallHit = Physics2D.Raycast(transform.position, Vector2.left, checkWallRadius, normalWallLayer | glassWallLayer);
            bool isTouchingLeftWall = wallHit && wallHit.collider != null;
            wallHit = Physics2D.Raycast(transform.position, Vector2.right, checkWallRadius, normalWallLayer | glassWallLayer);
            bool isTouchingRightWall = wallHit && wallHit.collider != null;

            if (horizontalValue > 0 && isTouchingLeftWall)
            {
                // Nhảy cao sang phải khi đang đu tường bên trái
                rb.velocity = new Vector2(speed, jumpPower);  // Điều chỉnh lực nhảy theo yêu cầu
            }
            else if (horizontalValue < 0 && isTouchingRightWall)
            {
                // Nhảy cao sang trái khi đang đu tường bên phải
                rb.velocity = new Vector2(-speed, jumpPower);  // Điều chỉnh lực nhảy theo yêu cầu
            }

        }
        else
        {
            // Bật lại trọng lực khi không giữ Space (hoặc không chạm vào tường)
            rb.gravityScale = 5;  // Trọng lực được áp dụng lại
        }
        animator.SetBool("isClinging", isTouchingGlassWall || isTouchingNormalWall);
        //if press Jump button, enable bool jump, othwewise turn it off
        if (Input.GetButtonDown("Jump"))
        {
            //Add jump force if available
            if (coyoteJump && !isJumped)
            {
                isJumped = true;
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
            coyoteJump = true;
            //Check if ground has a tag moving platform
            foreach (var c in collider)
            {
                if (c.tag == "MovingPlatform")
                {
                    if (isGrounded == false)
                        isGrounded = true;
                    transform.parent = c.transform;

                }
            }

        }
        else
        {
            //Un-parent the transform
            transform.parent = null;
            isJumped = true;
            isGrounded = false;
            StartCoroutine(CoyoteJumpDelay());
        }

        //animator.SetBool("Jump", !isGrounded);
    }

    IEnumerator CoyoteJumpDelay()
    {
        yield return new WaitForSeconds(0.3f);
        coyoteJump = false;
    }


    #endregion

    #region Stage
    public void ResetPosition()
    {
        transform.position = startPosition;
        hasMoved = false;
        isMoveable = true;
        rb.gravityScale = 5;
    }

    public void LockMove()
    {
        isMoveable = false;
        horizontalValue = 0;
        rb.gravityScale = 0;
        rb.velocity = Vector3.zero;
    }

    public void UnlockMove()
    {
        isMoveable = true;
        rb.gravityScale = 5;
    }

    public void Death()
    {
        LockMove();
        transform.position = startPosition;
    }

    #endregion


    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(groundCheckCollider.position, groundCheckRadius);


    //}

}
