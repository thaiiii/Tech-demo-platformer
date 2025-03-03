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
    private bool hasMoved = false; //Kiem tra xem da di chuyen chua
    private bool isMoveable = true;
    public bool isShotHorizontally = false; //Kiểm tra xem có tác động lực phương ngang không, nếu có sẽ ngưng update vận tốc phương ngang ở Move();
    public bool allowKeepVelocity = true;
    public bool isSliding = false;
    public float slipperyValue;
    private float horizontalValue;
    public float speed = 4f;
    public float environmentGravityScale = 5f;
    public float lastDirection = 0f; //-1: trái, 0: đứng im, 1: phải

    [Header("Wall")]
    private float wallSlideSpeed = 1f;
    [SerializeField] public float checkWallRadius = 0.6f;
    [SerializeField] public LayerMask normalWallLayer; //normal wall block
    [SerializeField] public LayerMask glassWallLayer; //glass wall block
    private bool isTouchingNormalWall = false;
    private bool isTouchingGlassWall = false;


    [Header("Ground")]
    [SerializeField] public Transform groundCheckCollider;
    [SerializeField] public LayerMask groundLayer;
    private float groundCheckRadius = 0.01f;
    private float jumpPower = 20f;
    private bool isJumped;
    private bool coyoteJump;
    private bool _isGrounded;
    public bool isGrounded
    {
        get => _isGrounded;
        set
        {
            if (_isGrounded != value) // Chỉ gọi sự kiện nếu trạng thái thực sự thay đổi
            {
                _isGrounded = value;
                OnGroundedChanged?.Invoke(this, _isGrounded);
            }
        }
    }
    public event Action<Player, bool> OnGroundedChanged;

    #region Default
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameTimer = FindAnyObjectByType<GameTimer>();
    }
    void Start()
    {
        startPosition = transform.position;
    }
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
        else
        {
            // Bật lại trọng lực khi không giữ Space (hoặc không chạm vào tường)
            rb.gravityScale = environmentGravityScale;  // Trọng lực được áp dụng lại
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
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            }
        }
    }
    private void Move()
    {
        if (!CanInputControl()) //Ngưng cập nhật vận tốc chiều ngang nếu ko thể di chuyển hoặc bị pháo đẩy đi
            return;
        if (isSliding) //Đổi kiểu di chuyển là trượt
            Slide(slipperyValue);
        else
        {
            Vector2 targetVelocity = new Vector2(horizontalValue * speed * 100 * Time.fixedDeltaTime, rb.velocity.y);
            rb.velocity = targetVelocity;
        }
        //Current scale
        Vector3 currentScale = transform.localScale;
        //Facing direction
        if (horizontalValue * currentScale.x < 0)
            currentScale.x = currentScale.x * -1;
        transform.localScale = currentScale;

        //0 for idle, 6 for walk, 12 for run => Setting xVelocity in Player animator
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
    }
    private void Slide(float slipperyValue)
    {
        if (!CanInputControl()) //Ngưng cập nhật vận tốc chiều ngang nếu ko thể di chuyển hoặc bị pháo đẩy đi
            return;
        rb.AddForce(horizontalValue * speed * slipperyValue * Vector2.right);
    }
    public void SwitchUpdateHorizontalVelocity(bool value)
    {
        isShotHorizontally = value;
    }
    private bool CanInputControl()
    {
        if (isShotHorizontally)
            return false;
        if (!isMoveable)
            return false;
        return true;
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
                //OnGroundedChanged?.Invoke(this, _isGrounded);                   //Event
                isJumped = false; // Cho phép nhảy lại
            }

            coyoteJump = true; // Coyote jump được kích hoạt lại

        }
        else
        {
            if (isGrounded) // Nếu rời khỏi mặt đất
            {
                isGrounded = false; // Đánh dấu không còn chạm đất
                //OnGroundedChanged?.Invoke(this, _isGrounded);                   //Event
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
        LockMove(false);
        transform.position = startPosition;
        hasMoved = false;
        isMoveable = true;
        rb.gravityScale = 5f;
        UnlockMove(false);
    }
    public void LockMove(bool isAllowed) // Không cho phép di chuyển
    {
        if (isMoveable)
        {
            allowKeepVelocity = isAllowed; //set lưu vận tốc
            rb.gravityScale = 0;
            isMoveable = false;
            horizontalValue = 0;
            if (!allowKeepVelocity)
            {
                //Debug.Log("zero");
                rb.velocity = Vector3.zero;
            }
        }
    }
    public void UnlockMove(bool isAllowed) // Cho phép di chuyển
    {
        //Debug.Log("Unlockmove: Gravity = 5");
        rb.gravityScale = 5f;
        isMoveable = true;
        allowKeepVelocity = isAllowed; //set lưu vận tốc
    }
    public void Death() //Chết = không cho phép di chuyển + về vị trí spawn, chưa từng di chuyển
    {
        ResetPosition();
        LockMove(false);

    }

    #endregion



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, checkWallRadius);
    }

}
