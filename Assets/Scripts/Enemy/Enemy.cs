using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Normal,
        Pointy
    }
    
    public float attackSpeed = 10f; // Tốc độ lao về phía điểm tiếp theo
    public float attackDelay = 1f; // Thời gian dừng trước khi tấn công
    public EnemyType type;
    private Rigidbody2D rb;
    private Animator animator;
    private MovingTrap movingTrap; // Tham chiếu đến script MovingEnemy
    private Vector3 previousPosition;
    [HideInInspector] public HealthComponent healthComponent;
    private Canvas healthUI;
    private bool isAttacking = false;
    public bool isEnemyDead = false;
    

    //Checkpoint info
    public Vector3 savedPosition;
    public bool savedDeadStatus;

    //============================================================================
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movingTrap = GetComponent<MovingTrap>();    
        animator = GetComponent<Animator>();
        healthComponent = GetComponent<HealthComponent>();
        healthUI = healthComponent.healthUI;
        previousPosition = transform.position;
        savedPosition = transform.position;
        savedDeadStatus = isEnemyDead;
    }
    private void Update()
    {
        if (transform.localScale.x > 0)
        {
            if (healthUI.GetComponent<RectTransform>().localScale.x < 0)
            healthUI.GetComponent<RectTransform>().localScale = new Vector3(
                Mathf.Abs(healthUI.GetComponent<RectTransform>().localScale.x),
                healthUI.GetComponent<RectTransform>().localScale.y,
                healthUI.GetComponent<RectTransform>().localScale.z);
        }
        else
        {
            if (healthUI.GetComponent<RectTransform>().localScale.x > 0)
                healthUI.GetComponent<RectTransform>().localScale = new Vector3(
                    -Mathf.Abs(healthUI.GetComponent<RectTransform>().localScale.x),
                    healthUI.GetComponent<RectTransform>().localScale.y,
                    healthUI.GetComponent<RectTransform>().localScale.z);
        }

        if (isEnemyDead)
            return;
        HandleAnimator();
        if (type == EnemyType.Pointy && !isAttacking)
        {
            DetectAndAttackPlayer();
        }
    }


    private void DetectAndAttackPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 playerPos = player.transform.position;
        Vector3 enemyPos = transform.position;
        Vector3 targetPos = movingTrap.waypoints[movingTrap.nextWaypointIndex].position;

        // Kiểm tra người chơi có nằm trong đường đi từ enemy tới target không
        if (IsPlayerOnPath(playerPos, enemyPos, targetPos) && !player.GetComponent<PlayerAbilities>().isHidden)
        {
            StartCoroutine(AttackPlayer(targetPos));
        }
    }
    private bool IsPlayerOnPath(Vector3 playerPos, Vector3 enemyPos, Vector3 targetPos)
    {
        Vector3 toTarget = targetPos - enemyPos;
        Vector3 toPlayer = playerPos - enemyPos;

        // Kiểm tra người chơi nằm trên đoạn đường từ enemy đến target
        return Vector3.Dot(toTarget.normalized, toPlayer.normalized) > 0.99f && toPlayer.magnitude < toTarget.magnitude;
    }
    private IEnumerator AttackPlayer(Vector3 targetPos)
    {
        isAttacking = true;
        movingTrap.DisableMovingWithoutCountdown(); // Dừng di chuyển tạm thời

        // Dừng 1s trước khi tấn công
        yield return new WaitForSeconds(1);

        // Lao nhanh về phía targetPos
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, attackSpeed * Time.deltaTime);
            yield return null;
        }

        isAttacking = false;
        movingTrap.StartCountdownForMoving(); // Kích hoạt di chuyển trở lại
    }
    private void HandleAnimator()
    {
        if (animator == null)
            return;
        // Tính toán vận tốc dựa trên thay đổi vị trí
        Vector3 currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
        animator.SetFloat("xVelocity", Mathf.Abs( currentVelocity.x));
    }
    public void LoadSavedEnemyStatus()
    {
        if (!savedDeadStatus)
        {
            isEnemyDead = savedDeadStatus;
            transform.position = savedPosition;
            rb.isKinematic = false;
            GetComponent<SpriteRenderer>().enabled = true;
            if (GetComponent<MovingTrap>() != null)
                GetComponent<MovingTrap>().isMovingActivated = true;
            GetComponent<Collider2D>().enabled = true;
            healthComponent.healthUI.enabled = false;
        } else
        {
            if (GetComponent<MovingTrap>() != null)
                GetComponent<MovingTrap>().isMovingActivated = false;
        }
    }
    public void KillEnemy()
    {
        if (isEnemyDead)
            return;
        isEnemyDead = true;
        GetComponent<SpriteRenderer>().enabled = false;
        transform.position = savedPosition;
        if (GetComponent<MovingTrap>() != null)
            GetComponent<MovingTrap>().isMovingActivated = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        healthComponent.healthUI.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trap"))
            KillEnemy();
        if (collision.CompareTag("Robot"))
            KillEnemy();
    }
}
