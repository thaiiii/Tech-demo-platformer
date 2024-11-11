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

    public LineRenderer laserBeam;
    public LayerMask obstacleLayers;
    private PlayerDeath playerDeath;

    // Start is called before the first frame update
    void Start()
    {
        laserBeam.enabled = isLaserActivate;
        playerDeath = FindObjectOfType<PlayerDeath>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLaserActivate)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, laserAngle);
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
            if (hit.collider.CompareTag("Player"))
            {
                playerDeath.Die();
                hit.collider.gameObject.GetComponent<Player>().Death();
            }
        }
        else
        {
            endPoint = transform.position + transform.up * maxLaserLength;
        }

        laserBeam.SetPosition(0, transform.position);
        laserBeam.SetPosition(1, endPoint);
    
    
    }

   


    public void DisableLaser()
    {
        if (disablePermanently)
        {
            isLaserActivate = false;
           
        }
        else
        {
            StartCoroutine(TemporaryDisable());
        }
        laserBeam.enabled = false;




    }

    IEnumerator TemporaryDisable()
    {
        isLaserActivate = false;
        yield return new WaitForSeconds(disableDuration);
        isLaserActivate = true;
        laserBeam.enabled = true;

    }
}
