using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnewayPlatform : MonoBehaviour
{
    private PlatformEffector2D platformEffector;
    public Collider2D platformCollider;
    public bool isFalling = false;

    private void Awake()
    {
        platformCollider = GetComponent<Collider2D>();
        platformEffector = GetComponent<PlatformEffector2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlatformInteraction();
    }

    private void HandlePlatformInteraction()
    {
        //Khi người chơi chưa rơi và nhấn xuống , cho phép rơi xuống
        if (Input.GetKeyDown(KeyCode.S) && !isFalling)
        {
            platformEffector.rotationalOffset = 180f; //Cho phép đi xuyên từ trên xuống
            isFalling = true;
            StartCoroutine(ResetRotationalOffset());
        }
        
    }

    private IEnumerator ResetRotationalOffset()
    {
        yield return new WaitForSeconds(0.3f);
        //
        if (isFalling && !IsPlayerAbovePlatform())
        {
            Debug.Log("Player left platform, resetting offset to 0");
            platformEffector.rotationalOffset = 0f; // Đặt lại để người chơi đứng được
            isFalling = false;
        }
    }

    private bool IsPlayerAbovePlatform ()
    {
        // Lấy tất cả Collider trong phạm vi của platform
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            platformCollider.bounds.center,
            platformCollider.bounds.size,
            0f
        );

        foreach (var col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                return true; // Người chơi vẫn nằm trong phạm vi platform
            }
        }
        return false; // Không có người chơi trong phạm vi
    }
}
