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

    public EnemyType type;
    private Animator animator;
    private Rigidbody2D rb;
    private MovingEnemy movingEnemy; // Tham chiếu đến script MovingEnemy
    private Vector3 previousPosition;
    public float attackSpeed = 10f; // Tốc độ lao về phía điểm tiếp theo
    public float attackDelay = 1f; // Thời gian dừng trước khi tấn công
    
    private bool isAttacking = false;

    //Checkpoint info
    public Vector3 savedPosition;
    //Hệ thống máu
    public bool isEnemyDead = false;

    //============================================================================
    private void Awake()
    {
        movingEnemy = GetComponent<MovingEnemy>();    
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        previousPosition = transform.position;
        savedPosition = transform.position;
    }

    private void Update()
    {
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
        Vector3 targetPos = movingEnemy.waypoints[movingEnemy.nextWaypointIndex].position;

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
        movingEnemy.DisableMovingWithoutCountdown(); // Dừng di chuyển tạm thời

        // Dừng 1s trước khi tấn công
        yield return new WaitForSeconds(1);

        // Lao nhanh về phía targetPos
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, attackSpeed * Time.deltaTime);
            yield return null;
        }

        isAttacking = false;
        movingEnemy.StartCountdownForMoving(); // Kích hoạt di chuyển trở lại
    }

    private void HandleAnimator()
    {
        // Tính toán vận tốc dựa trên thay đổi vị trí
        Vector3 currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
        animator.SetFloat("xVelocity", Mathf.Abs( currentVelocity.x));
    }

    public void LoadSavedEnemyStatus()
    {
        //chua co gi
    }

    public void KillEnemy()
    {
        //giet enemy
        Debug.Log("enemy dead");
    }
}
