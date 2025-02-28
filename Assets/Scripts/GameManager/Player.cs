using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    private GameTimer gameTimer;
    public Vector3 startPosition;

    [Header("Movement")]
    private float horizontalValue;
    public float speed = 4f;
    private bool hasMoved = false; //Kiem tra xem da di chuyen chua
    private bool isMoveable = true;
    public bool isShotHorizontally = false; //Kiểm tra xem có tác động lực phương ngang không, nếu có sẽ ngưng update vận tốc phương ngang ở Move();
    public float evironmentGravityScale = 5f;

    [Header("Wall")]
    private float wallSlideSpeed = 1f;
    public LayerMask normalWallLayer; //normal wall block
    public LayerMask glassWallLayer; //glass wall block
    private bool isTouchingNormalWall = false;
    private bool isTouchingGlassWall = false;
    public float checkWallRadius = 0.6f;

    [Header("Ground")]
    public Transform groundCheckCollider;
    public bool isGrounded;
    private float groundCheckRadius = 0.01f;
    public LayerMask groundLayer;
    private bool isJumped;
    private bool coyoteJump;
    private float jumpPower = 20f;




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
        if (!hasMoved && Mathf.Abs(horizontalValue) > 0)
        {
            hasMoved = true;
            gameTimer.StartTimer();
        }

        // Kiểm tra xem người chơi có chạm vào block thường hay block kính
        isTouchingNormalWall = Physics2D.OverlapCircle(transform.position, checkWallRadius, normalWallLayer);
        isTouchingGlassWall = Physics2D.OverlapCircle(transform.position, checkWallRadius, glassWallLayer);
        if (Input.GetButton("Jump") && !coyoteJump && (isTouchingNormalWall || isTouchingGlassWall))
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
        else //if (!Input.GetButton("Jump"))
        {
            // Bật lại trọng lực khi không giữ Space (hoặc không chạm vào tường)
            rb.gravityScale = evironmentGravityScale;  // Trọng lực được áp dụng lại
        }
        animator.SetBool("isClinging", !isGrounded && (isTouchingGlassWall || isTouchingNormalWall));
        // Nếu bấm nút nhảy
        if (Input.GetButtonDown("Jump"))
        {
            // Nếu đang trong coyote jump hoặc đứng trên mặt đất
            if ((coyoteJump || isGrounded) && !isJumped)
            {
                isJumped = true; // Đánh dấu trạng thái đang nhảy
                coyoteJump = false; // Tắt coyote jump ngay lập tức

                // Thêm lực nhảy
                rb.velocity = Vector2.up * jumpPower;
            }
        }
    }

    private void Move()
    {
        if (isShotHorizontally) //Ngưng cập nhật vận tốc chiều ngang
            return;
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

    public void SwitchUpdateHorizontalVelocity(bool value)
    {
        isShotHorizontally = value;
    }

    private void GroundCheck()
    {
        // Kiểm tra nếu người chơi đang đứng trên mặt đất
        Collider2D[] collider = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        if (collider.Length > 0)
        {
            if (!isGrounded) // Nếu mới chạm đất
            {
                isGrounded = true;
                isJumped = false; // Cho phép nhảy lại
            }

            coyoteJump = true; // Coyote jump được kích hoạt lại

        }
        else
        {
            if (isGrounded) // Nếu rời khỏi mặt đất
            {
                isGrounded = false; // Đánh dấu không còn chạm đất
                StartCoroutine(CoyoteJumpDelay()); // Kích hoạt coyote jump
            }
        }

        animator.SetBool("Jump", !isGrounded);
    }

    IEnumerator CoyoteJumpDelay()
    {
        yield return new WaitForSeconds(0.3f);
        coyoteJump = false;
    }


    #endregion

    #region Stage
    public void ResetPosition() // Trạng thái về vị trí spawn, chưa từng di chuyển
    {
        LockMove();
        transform.position = startPosition;
        hasMoved = false;
        isMoveable = true;
        rb.gravityScale = 5f;
        UnlockMove();
    }

    public void LockMove() // Không cho phép di chuyển
    {
        if (isMoveable)
        {
            //Debug.Log("lockmove: Gravity = 0");
            rb.gravityScale = 0;
            isMoveable = false;
            horizontalValue = 0;
            rb.velocity = Vector3.zero;
        }
    }

    public void UnlockMove() // Cho phép di chuyển
    {
        //Debug.Log("Unlockmove: Gravity = 5");
        rb.gravityScale = 5f;
        isMoveable = true;
    }

    public void Death() //Chết = không cho phép di chuyển + về vị trí spawn, chưa từng di chuyển
    {
        ResetPosition();
        LockMove();

    }

    #endregion


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, checkWallRadius);
    }

}
