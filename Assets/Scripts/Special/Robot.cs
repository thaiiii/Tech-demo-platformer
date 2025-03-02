using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    private float chargeTime = 0f;
    public float maxChargeTime = 2f;
    public float minJumpForce = 20f;
    public float maxJumpForce = 50f;
    public bool isDestroyed = false;
    public bool isPlayerInRange = false;
    public bool isControlled = false;
    private Rigidbody2D rb;
    private HealthComponent health;
    public PlayerAbilities playerAbilities;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthComponent>();
        health.GetHealthSystem();
        //health.GetHealthSystem().OnDeath += OnRobotDestroyed;
    }
    private void Update()
    {
        if (playerAbilities != null)
        {
            if (Input.GetKeyDown(KeyCode.E) && playerAbilities.CanEnterRobot(this))
                playerAbilities.EnterRobot(this);

            if (isControlled)
            {
                RobotMove();
                if (Input.GetKeyDown(KeyCode.Q))
                    playerAbilities.ExitRobot();
            }
        }
    }

    private void RobotMove()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * 5f, rb.velocity.y);

        if (Input.GetKey(KeyCode.Space))
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("up");
            float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargeTime / maxChargeTime);
            Vector2 jumpDir = Vector2.up;

            if (Input.GetKey(KeyCode.A)) jumpDir = new Vector2(-1, 1);
            if (Input.GetKey(KeyCode.D)) jumpDir = new Vector2(1, 1);

            rb.velocity = jumpDir.normalized * jumpForce;
            chargeTime = 0f;
        }

        if (playerAbilities != null && playerAbilities.transform.position != (Vector3)rb.position) //Update vị trí player nếu khác robot
            playerAbilities.transform.position = rb.position;   
    }
    public void SetControlled(bool controlled)
    {
        isControlled = controlled;
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
