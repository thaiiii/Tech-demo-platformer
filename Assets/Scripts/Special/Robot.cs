using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [Header("UI")]
    public Canvas healthUI;
    public Canvas jumpUI;
    public Slider jumpForceSlider;

    [Header("Jump")]
    public float robotSpeed = 10f;
    private float chargeTime = 0f;
    public float maxChargeTime = 2f;
    public float minJumpForce = 20f;
    public float maxJumpForce = 20f;

    [Header("Robot status")]
    public bool isDestroyed = false;
    public bool isPlayerInRange = false;
    public bool isControlled = false;
    [SerializeField] private bool canMove = true;    //Mở khóa điều khiển
    private Rigidbody2D rb;
    private HealthComponent health;
    public PlayerAbilities playerAbilities;
    public LayerMask groundLayer;
    private CameraFollow cameraFollow;

    [Header("Save info")]//Saved info
    [SerializeField] private bool savedDestroyStatus;
    [SerializeField] private Vector2 restartPosition;

    [Header("Dash")]//Dash
    public float dashSpeed = 20f; // Tốc độ Dash
    public float dashDuration = 0.3f; // Thời gian Dash
    public float dashCooldown = 5f; // Hồi chiêu Dash
    [SerializeField] private bool isDashing = false;
    [SerializeField] private float lastDashTime = 0;
    //=================================================================================
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthComponent>();
        health.GetHealthSystem();
        cameraFollow = Camera.main.GetComponent<CameraFollow>();

        //First save info
        restartPosition = transform.position;
        savedDestroyStatus = isDestroyed;
    }
    private void Update()
    {
        if (playerAbilities != null)
        {
            if (Input.GetKeyDown(KeyCode.E) && playerAbilities.CanEnterRobot(this))
                EnterThisRobot();

            if (isControlled)
            {
                if (!CheckRobotOnGround())
                    canMove = false;
                RobotMove();
                if (Input.GetKeyDown(KeyCode.Q))
                    ExitThisRobot();
            }
            else
            {
                if (CheckRobotOnGround())
                {
                    rb.velocity = Vector3.zero;
                    ToggleRobotPhysics(true);
                    if (!isPlayerInRange)
                        playerAbilities = null;
                }
            }
        }
        else
        {
            if (CheckRobotOnGround())
                ToggleRobotPhysics(true);
        }
    }

    private void RobotMove()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (canMove && !isDashing)
        {
            rb.velocity = new Vector2(moveInput * robotSpeed, rb.velocity.y);
            if (moveInput > 0)
            {
                if (transform.localScale.x < 0)
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                jumpUI.gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            else if (moveInput < 0)
            {
                if (transform.localScale.x > 0)
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                jumpUI.gameObject.GetComponent<RectTransform>().localScale = new Vector3(-0.1f, 0.1f, 0.1f);
            }
        }

        if (CheckRobotOnGround())
        {
            #region Jump
            Vector2 jumpDir = Vector2.up;
            if (Input.GetKey(KeyCode.Space))
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                canMove = false;
                chargeTime += Time.deltaTime;
                chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime);

                if (!jumpUI.enabled) 
                    jumpUI.enabled = true;
                jumpForceSlider.value = chargeTime / maxChargeTime;
                                
                if (Input.GetKey(KeyCode.A))
                    jumpDir = new Vector2(-1, 1);
                else if (Input.GetKey(KeyCode.D))
                    jumpDir = new Vector2(1, 1);
            }
            else
                canMove = true;

            if (Input.GetKeyUp(KeyCode.Space))
            {
                jumpUI.enabled = false;
                float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargeTime / maxChargeTime);
                rb.velocity = jumpDir.normalized * jumpForce;
                
                chargeTime = 0f;
            }
            #endregion
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                jumpUI.enabled = false;
                chargeTime = 0f;
            }
        }
        #region Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
            StartCoroutine(Dash());
        #endregion

        //Update vị trí player nếu khác robot
        if (playerAbilities != null && playerAbilities.transform.position != (Vector3)rb.position)
            playerAbilities.transform.position = rb.position;
    }
    public void SetControlled(bool controlled) => isControlled = controlled;
    public void OnRobotDestroyed()
    {
        isDestroyed = true;
        if (playerAbilities == null)
            return;
        HealthComponent playerHealthComponent = playerAbilities.gameObject.GetComponent<HealthComponent>();
        if (playerAbilities.isInRobot)
        {
            ExitThisRobot();
            playerHealthComponent.TakeDamage(playerHealthComponent.maxHealth); // Chết theo robot
        }
    }
    public HealthComponent GetHealthComponent() => health;
    public bool CheckRobotOnGround() => GetComponent<Collider2D>().IsTouchingLayers(groundLayer);
    public void ToggleRobotPhysics(bool value)
    {
        if (playerAbilities != null)
            Physics2D.IgnoreCollision(playerAbilities.GetComponent<Collider2D>(), GetComponent<Collider2D>(), !value);
        GetComponent<Rigidbody2D>().isKinematic = value;
        GetComponent<Collider2D>().isTrigger = value;
    }
    public void LoadSavedRobotStatus()
    {
        transform.position = restartPosition;
        isDestroyed = savedDestroyStatus;
        isControlled = false;
        canMove = true;
        isPlayerInRange = false;
        chargeTime = 0f;

        // Đợi 0.1 giây rồi mới bật vật lý
        StartCoroutine(EnablePhysicsAfterDelay(0.02f));
    }
    private IEnumerator EnablePhysicsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ToggleRobotPhysics(false); // Áp dụng vật lý sau delay
    }
    public void SaveRobotStatus()
    {
        restartPosition = transform.position;
        savedDestroyStatus = isDestroyed;
    }
    private IEnumerator Dash()
    {
        float originalSmoothFactor = cameraFollow.smoothFactor;

        if (Time.time - lastDashTime < dashCooldown) yield break; // Chặn nếu chưa hồi chiêu

        //Khóa input
        isDashing = true;
        canMove = false;
        lastDashTime = Time.time;

        cameraFollow.SetSmoothFactor(dashSpeed / robotSpeed * originalSmoothFactor);
        float originalGravity = rb.gravityScale; // Lưu trọng lực cũ
        rb.gravityScale = 0; // Tắt trọng lực để không bị rơi
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0); // Dash theo hướng hiện tại


        yield return new WaitForSeconds(dashDuration); // Đợi Dash kết thúc

        cameraFollow.SetSmoothFactor(originalSmoothFactor);
        rb.velocity = Vector2.zero;
        rb.gravityScale = originalGravity; // Khôi phục trọng lực

        //Mở khóa input
        isDashing = false;
        canMove = true;
    }
    private void EnterThisRobot()
    {
        if (!playerAbilities.isNormalStatus())
            return;
        cameraFollow.SetCameraSize(cameraFollow.GetComponent<Camera>().orthographicSize + 1);
        healthUI.enabled = true;
        playerAbilities.GetComponent<HealthComponent>().healthUI.enabled = false;
        playerAbilities.EnterRobot(this);
        
    }
    private void ExitThisRobot()
    {
        playerAbilities.GetComponent<HealthComponent>().healthUI.enabled = true;
        healthUI.enabled = false;
        jumpUI.enabled = false;
        chargeTime = 0f;
        cameraFollow.SetCameraSize(cameraFollow.GetComponent<Camera>().orthographicSize -1);
        playerAbilities.ExitRobot();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerAbilities = other.GetComponent<PlayerAbilities>();
            isPlayerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerAbilities exitingPlayer = other.GetComponent<PlayerAbilities>();
            // Chỉ đặt isPlayerInRange = false nếu người chơi không còn trong robot
            if (exitingPlayer != null && !exitingPlayer.isInRobot)
            {
                isPlayerInRange = false;
                playerAbilities = null;
            }
        }
    }
}
