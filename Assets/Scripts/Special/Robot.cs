using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    private float chargeTime = 0f;
    public float maxChargeTime = 2f;
    public float minJumpForce = 20f;
    public float maxJumpForce = 20f;
    public float robotSpeed = 10f;
    public bool isDestroyed = false;
    public bool isPlayerInRange = false;
    public bool isControlled = false;
    private bool canMove = true;    //Mở khóa điều khiển

    private Rigidbody2D rb;
    private HealthComponent health;
    public PlayerAbilities playerAbilities;
    public LayerMask groundLayer;

    //Saved info
    [SerializeField] private bool savedDestroyStatus;
    [SerializeField] private Vector2 restartPosition;
    [SerializeField] private float savedHealth;
    //=================================================================================
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthComponent>();
        health.GetHealthSystem();

        //First save info
        restartPosition = transform.position;
        savedDestroyStatus = isDestroyed;
        savedHealth = health.maxHealth;
    }
    private void Update()
    {
        if (playerAbilities != null)
        {
            if (Input.GetKeyDown(KeyCode.E) && playerAbilities.CanEnterRobot(this))
                playerAbilities.EnterRobot(this);

            if (isControlled)
            {
                if (!CheckRobotOnGround())
                    SetCanMove(false);
                RobotMove();
                if (Input.GetKeyDown(KeyCode.Q))
                    playerAbilities.ExitRobot();
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

        if (canMove)
            rb.velocity = new Vector2(moveInput * 10f, rb.velocity.y);

        if (CheckRobotOnGround())
        {
            Vector2 jumpDir = Vector2.up;
            if (Input.GetKey(KeyCode.Space))
            {
                SetCanMove(false);
                chargeTime += Time.deltaTime;
                chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime);
                if (Input.GetKey(KeyCode.A))
                    jumpDir = new Vector2(-1, 1);
                else if (Input.GetKey(KeyCode.D))
                    jumpDir = new Vector2(1, 1);
                else if (Input.GetKey(KeyCode.W))
                    jumpDir = Vector2.up;
            }
            else
                SetCanMove(true);

            if (Input.GetKeyUp(KeyCode.Space))
            {
                float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargeTime / maxChargeTime);

                rb.velocity = jumpDir.normalized * jumpForce;
                chargeTime = 0f;
            }
        }

        if (playerAbilities != null && playerAbilities.transform.position != (Vector3)rb.position) //Update vị trí player nếu khác robot
            playerAbilities.transform.position = rb.position;
    }
    public void SetControlled(bool controlled)
    {
        isControlled = controlled;
    }
    private void SetCanMove(bool value)
    {
        canMove = value;
    }
    public void OnRobotDestroyed()
    {
        isDestroyed = true;
        if (playerAbilities == null)
            return;
        HealthComponent playerHealthComponent = playerAbilities.gameObject.GetComponent<HealthComponent>();
        if (playerAbilities.isInRobot)
        {
            playerAbilities.ExitRobot();
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
        transform.position = restartPosition + Vector2.right * 3f;
        isDestroyed = savedDestroyStatus;
        health.SetCurrentHealth(savedHealth);
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
        savedHealth = health.GetHealthSystem().currentHealth;
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
