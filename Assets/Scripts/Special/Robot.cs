﻿using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Cinemachine;

public class Robot : MonoBehaviour
{
    [Header("UI")]
    public Canvas healthUI;
    public Canvas jumpUI;
    public Slider jumpForceSlider;

    [Header("Move")]
    public Transform groundCheck;
    private float horizontalValue;
    public float robotSpeed = 10f;
    private float chargeTime = 0f;
    public float maxChargeTime = 2f;
    public float minJumpForce = 20f;
    public float maxJumpForce = 20f;
    public bool isSliding = false;
    public float slipperyValue;

    [Header("Robot status")]
    public bool isDestroyed = false;
    public bool isPlayerInRange = false;
    public bool isControlled = false;
    [SerializeField] private bool canMove = true;    //Mở khóa điều khiển
    public float originalGravity = 5f;
    private Rigidbody2D rb;
    private HealthComponent health;
    public PlayerAbilities playerAbilities;
    public LayerMask groundLayer;
    private CinemachineVirtualCamera virtualCamera;

    [Header("Save info")]//Saved info
    [SerializeField] private bool savedDestroyStatus;
    [SerializeField] private Vector2 restartPosition;

    [Header("Dash")]//Dash
    public float dashSpeed = 20f; // Tốc độ Dash
    public float dashDuration = 0.3f; // Thời gian Dash
    public float dashCooldown = 5f; // Hồi chiêu Dash
    [SerializeField] public bool isDashing = false;
    [SerializeField] private float lastDashTime = 0;

    float defaultRobotCameraSize;
    //=================================================================================
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthComponent>();
        health.GetHealthSystem();
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>(); // Tìm Cinemachine Camera
        defaultRobotCameraSize = virtualCamera.m_Lens.OrthographicSize + 1;

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
                SetCamera();
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
        #region Normal move
        if (canMove && !isDashing)
        {
            horizontalValue = Input.GetAxisRaw("Horizontal");
            if (horizontalValue > 0)
            {
                if (transform.localScale.x < 0)
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                if (jumpUI.gameObject.GetComponent<RectTransform>().localScale.x < 0)
                    jumpUI.gameObject.GetComponent<RectTransform>().localScale = new Vector3(
                        -jumpUI.gameObject.GetComponent<RectTransform>().localScale.x,
                        jumpUI.gameObject.GetComponent<RectTransform>().localScale.y,
                        jumpUI.gameObject.GetComponent<RectTransform>().localScale.z);
            }
            else if (horizontalValue < 0)
            {
                if (transform.localScale.x > 0)
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                if (jumpUI.gameObject.GetComponent<RectTransform>().localScale.x > 0)
                    jumpUI.gameObject.GetComponent<RectTransform>().localScale = new Vector3(
                        -jumpUI.gameObject.GetComponent<RectTransform>().localScale.x,
                        jumpUI.gameObject.GetComponent<RectTransform>().localScale.y,
                        jumpUI.gameObject.GetComponent<RectTransform>().localScale.z);
            }
            if (isSliding)
                RobotSlide(slipperyValue);
            else
                rb.velocity = new Vector2(horizontalValue * robotSpeed, rb.velocity.y);
        }
        #endregion

        #region Jump
        if (CheckRobotOnGround())
        {
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

                if (Input.GetKeyDown(KeyCode.A))
                    jumpDir = new Vector2(-1, 1);
                else if (Input.GetKeyDown(KeyCode.D))
                    jumpDir = new Vector2(1, 1);
            }
            else
                canMove = true;

            if (Input.GetKeyUp(KeyCode.Space))
            {
                AudioManager.Instance.PlaySFX("robot_jump");
                jumpUI.enabled = false;
                float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargeTime / maxChargeTime);
                rb.velocity = jumpDir.normalized * jumpForce;

                chargeTime = 0f;
            }

        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                jumpUI.enabled = false;
                chargeTime = 0f;
            }
        }
        #endregion

        #region Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
            StartCoroutine(Dash());
        #endregion

        //Update vị trí player nếu khác robot
        if (playerAbilities != null && playerAbilities.transform.position != (Vector3)rb.position)
            playerAbilities.transform.position = rb.position;
    }
    private void RobotSlide(float slipperyValue)
    {
        rb.AddForce(horizontalValue * robotSpeed * slipperyValue * Vector2.right);
    }
    public void SetControlled(bool controlled) => isControlled = controlled;
    public void OnRobotDestroyed()
    {
        if (isDestroyed)
            return;
        transform.position = restartPosition;
        isDestroyed = true;
        rb.velocity = Vector3.zero;
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
    public bool CheckRobotOnGround()
    {
        Vector2 boxSize = new Vector2(GetComponent<Collider2D>().bounds.size.x - 0.1f, 0.1f);
        Collider2D[] collider = Physics2D.OverlapBoxAll(groundCheck.position, boxSize, 0f, groundLayer);
        if (collider.Length > 0)
            return true;
        else
            return false;
    }
    public void ToggleRobotPhysics(bool value)
    {
        if (playerAbilities != null)
            Physics2D.IgnoreCollision(playerAbilities.GetComponent<Collider2D>(), GetComponent<Collider2D>(), !value);
        GetComponent<Rigidbody2D>().isKinematic = value;
        GetComponent<Collider2D>().isTrigger = value;
    }
    public void LoadSavedRobotStatus()
    {
        ExitThisRobot();

        canMove = true;
        isPlayerInRange = false;
        isDestroyed = savedDestroyStatus;
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
        Vector2 lastVelocity = rb.velocity;
        //float originalSmoothFactor = cameraFollow.smoothFactor;

        if (Time.time - lastDashTime < dashCooldown) yield break; // Chặn nếu chưa hồi chiêu

        //Khóa input
        isDashing = true;
        canMove = false;
        lastDashTime = Time.time;

        //cameraFollow.SetSmoothFactor(dashSpeed / robotSpeed * originalSmoothFactor);
        originalGravity = rb.gravityScale; // Lưu trọng lực cũ
        rb.gravityScale = 0; // Tắt trọng lực để không bị rơi
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0); // Dash theo hướng hiện tại

        AudioManager.Instance.PlaySFX("dash");

        yield return new WaitForSeconds(dashDuration); // Đợi Dash kết thúc

        //cameraFollow.SetSmoothFactor(originalSmoothFactor);
        rb.velocity = Vector2.zero;
        rb.velocity = lastVelocity;
        rb.gravityScale = originalGravity; // Khôi phục trọng lực

        //Mở khóa input
        isDashing = false;
        canMove = true;
    }
    private void EnterThisRobot()
    {
        if (!playerAbilities.isNormalStatus())
            return;
        SetCameraSize(virtualCamera.m_Lens.OrthographicSize + 1f, 1f);
        healthUI.enabled = true;
        playerAbilities.GetComponent<HealthComponent>().healthUI.enabled = false;
        playerAbilities.EnterRobot(this);

    }
    private void ExitThisRobot()
    {
        if (playerAbilities == null)
            return;
        playerAbilities.GetComponent<HealthComponent>().healthUI.enabled = true;
        healthUI.enabled = false;
        jumpUI.enabled = false;
        SetCameraSize(virtualCamera.m_Lens.OrthographicSize - 1f, 1f);

        chargeTime = 0f;
        isControlled = false;

        playerAbilities.ExitRobot();
    }
    public IEnumerator TemporaryStopUpdatingHorizontalVelocity()
    {
        canMove = false;
        yield return new WaitForSeconds(0.1f);
        if (CheckRobotOnGround())
            canMove = true;
        else
            StartCoroutine(TemporaryStopUpdatingHorizontalVelocity());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerAbilities = other.GetComponent<PlayerAbilities>();
            playerAbilities.isNearRobot = true;
            isPlayerInRange = true;
        }
        if (other.CompareTag("Trap"))
        {
            OnRobotDestroyed();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerAbilities exitingPlayer = other.GetComponent<PlayerAbilities>();
            playerAbilities.isNearRobot = false;
            if (exitingPlayer != null && !exitingPlayer.isInRobot)
            {
                isPlayerInRange = false;
                playerAbilities = null;
            }
        }
    }


    private IEnumerator SmoothZoom(float targetSize, float duration)
    {
        if (duration == 0)
        {
            virtualCamera.m_Lens.OrthographicSize = targetSize;
            yield return null;
        }    
        float startSize = virtualCamera.m_Lens.OrthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        virtualCamera.m_Lens.OrthographicSize = targetSize; // Đảm bảo set đúng size cuối cùng
    }
    private void SetCamera()
    {
        if (chargeTime > 0)
            SetCameraSize(defaultRobotCameraSize + (chargeTime/maxChargeTime) * 3f, 0f);
        else
            SetCameraSize(defaultRobotCameraSize, 0.5f);
    }
    private void SetCameraSize(float targetSize, float duration)
    {
        if (virtualCamera != null)
        {
            StartCoroutine(SmoothZoom(targetSize, duration));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(groundCheck.position, new Vector2(GetComponent<Collider2D>().bounds.size.x - 0.1f, 0.1f));
    }


}
