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
    public bool isLaserActivate = true;
    public bool disablePermanently = false;
    public bool savedActivationStatus = true;

    public LineRenderer laserBeam;
    public LayerMask obstacleLayers;
    private PlayerDeath playerDeath;
    private Coroutine countdownCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        laserBeam.enabled = isLaserActivate;
        playerDeath = FindObjectOfType<PlayerDeath>();
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
            if (hit.collider.CompareTag("Player") && !hit.collider.GetComponent<PlayerDeath>().HiddenStatus())
            {
                playerDeath.KillPlayer();
                hit.collider.gameObject.GetComponent<PlayerDeath>().KillPlayer();
            }
        }
        else
        {
            endPoint = transform.position + transform.up * maxLaserLength;
        }

        if (laserBeam.enabled == false)
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
            countdownCoroutine = StartCoroutine(TemporaryDisable());
        }
    }

    IEnumerator TemporaryDisable()
    {
        yield return new WaitForSeconds(disableDuration);
        isLaserActivate = true;
        laserBeam.enabled = true;

    }

    public void LoadSavedEmitterStatus()
    {
        isLaserActivate = true ? savedActivationStatus : false;
    }
}
