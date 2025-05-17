using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTrap : MonoBehaviour
{
    public GameObject bulletPrefab;      // Prefab của đạn
    [SerializeField]private Transform firePoint;          // Vị trí bắn đạn
    public float fireInterval = 2.0f;    // Thời gian giữa các lần bắn
    public float bulletSpeed = 5.0f;     // Tốc độ bay của đạn
    [Range(0f, 360f)]public float bulletAngle = 0.0f;     // Góc bắn (độ)

    //Checkpoint
    public float savedBulletAngle;
    private void Start()
    {
        StartCoroutine(FireBullets());
        savedBulletAngle = bulletAngle;
    }

    void Update()
    {
        // Nội suy từ góc hiện tại đến góc mục tiêu
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, bulletAngle), Time.deltaTime * 5f);
    }

    private IEnumerator FireBullets()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireInterval);
            Fire();
        }
    }

    private void Fire()
    {
        // Tạo viên đạn tại firePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.transform.parent = gameObject.transform;
        

        // Tính toán hướng bắn dựa trên góc
        Vector2 direction = new Vector2(Mathf.Cos(bulletAngle * Mathf.Deg2Rad), Mathf.Sin(bulletAngle * Mathf.Deg2Rad));
        bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        // Hủy đạn sau một thời gian (tránh tiêu tốn tài nguyên)
        Destroy(bullet, 10f);
    }
    public void LoadSavedGunStatus()
    {
        bulletAngle = savedBulletAngle;
    }
}
