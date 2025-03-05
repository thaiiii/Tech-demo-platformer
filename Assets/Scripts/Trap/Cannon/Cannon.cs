using System.Collections;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public GameObject missilePrefab;
    public Transform firePoint;
    public float rotationSpeed = 2f;
    public float fireRate = 3f; // thời gian chờ giữa các phát bắn
    private float nextFireTime;
    public float disableDuration;
    public LayerMask obstacleLayer;
    private Coroutine countdownCoroutine;
    public bool isCannonActivated = true;
    
    public bool disablePermanently = false;
    public bool savedActivationStatus = true;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nextFireTime = Time.time;
    }

    void Update()
    {
        if (isCannonActivated)
        {
            AimAtPlayer();
            CheckAndFire();
        }
    }

    void AimAtPlayer()
    {
        // Tính toán hướng để nhắm tới người chơi
        Vector2 direction = (player.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    void CheckAndFire()
    {
        // Kiểm tra xem có vật cản nào giữa khẩu pháo và người chơi không
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, player.position - firePoint.position, 500f, obstacleLayer);
        if (hit.collider != null && hit.collider.CompareTag("Player") && Time.time >= nextFireTime)
        {
            FireMissile();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FireMissile()
    {
        // Tạo một tên lửa tại firePoint và hướng về phía người chơi
        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
        Missile missileScript = missile.GetComponent<Missile>();
        missileScript.SetTarget(player);
    }

    public void ClearAllMissiles()
    {
        Missile[] missiles = FindObjectsOfType<Missile>();
        foreach (Missile m in missiles)
        {
            Destroy(m.gameObject);
        }
    }
    public void ResetCannon()
    {
        if (savedActivationStatus)
            isCannonActivated = true;
        else
            isCannonActivated = false;
    }

    public void DisableCannonWithoutCountdown()
    {
        isCannonActivated = false;
        // Nếu có một countdown đang chạy, hủy nó
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
    }

    public void StartCountdownForCannon()
    {
        if (!disablePermanently)
        {
            // Bắt đầu đếm ngược để bật lại laser sau khi rời khỏi switch
            countdownCoroutine = StartCoroutine(TemporaryDisable());
        }
    }

    IEnumerator TemporaryDisable()
    {
        yield return new WaitForSeconds(disableDuration);
        isCannonActivated = true;
    }
}
