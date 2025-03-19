using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEmitter : MonoBehaviour
{
    public float maxLaserLength = 10f;
    public float disableDuration = 5f;
    [Range(0f, 360f)]
    public float laserAngle;
    public bool isLaserActivate;
    public bool disablePermanently = false;
    public bool savedActivationStatus;
    public float savedLaserAngle;

    public LineRenderer laserBeam;
    public LayerMask obstacleLayers;
    private PlayerDeath playerDeath;
    private Coroutine countdownCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        laserBeam.enabled = isLaserActivate;
        playerDeath = FindObjectOfType<PlayerDeath>();
        savedActivationStatus = isLaserActivate;
        savedLaserAngle = laserAngle;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, laserAngle);
        if (isLaserActivate)
        {
            UpdateLaserBeam();
        }
         
    }

    private void UpdateLaserBeam()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, maxLaserLength, obstacleLayers);
        Vector3 endPoint;
        if (hit.collider != null)
        {
            endPoint = hit.point;
            
            //Kiểm tra player
            if (hit.collider.CompareTag("Player") && !hit.collider.GetComponent<PlayerDeath>().HiddenStatus())
                playerDeath.KillPlayer();
            // Kiểm tra nếu trúng đạn (dựa vào layer)
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Bullet"))
                Destroy(hit.collider.gameObject);
            //Kiểm tra trúng clone
            else if (hit.collider.gameObject.CompareTag("SlimeClone"))
                hit.collider.gameObject.GetComponent<SlimeClone>().KillClone();
            //Kiểm tra robot
            else if (hit.collider.gameObject.CompareTag("Robot"))
                hit.collider.GetComponent<HealthComponent>().TakeDamage(hit.collider.GetComponent<HealthComponent>().maxHealth);
            //Kiểm tra Enemy 
            else if (hit.collider.gameObject.CompareTag("Enemy"))
                hit.collider.GetComponent<Enemy>().KillEnemy();

        }
        else
        {
            endPoint = transform.position + transform.up * maxLaserLength;
        }

        if (!laserBeam.enabled)
            laserBeam.enabled = isLaserActivate;
        laserBeam.SetPosition(0, transform.position);
        laserBeam.SetPosition(1, endPoint);
    
    
    }

   


    public void DisableLaserWithoutCountdown()
    {
        laserBeam.enabled = false;
        isLaserActivate = false;
        // Nếu có một countdown đang chạy, hủy nó
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }



    }

    public void StartCountdownForLaser()
    {
        if (!disablePermanently)
        {
            // Bắt đầu đếm ngược để bật lại laser sau khi rời khỏi switch
            countdownCoroutine = StartCoroutine(TemporaryDisable(disableDuration));
        }
    }

    IEnumerator TemporaryDisable(float disableDuration)
    {
        yield return new WaitForSeconds(disableDuration);
        isLaserActivate = true;
        laserBeam.enabled = true;

    }

    public void LoadSavedEmitterStatus()
    {
        isLaserActivate = savedActivationStatus;
        laserAngle = savedLaserAngle;
    }
}
