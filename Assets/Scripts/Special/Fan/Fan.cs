﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fan : MonoBehaviour
{
    [Header("Fan Settings")]
    public List<string> affectedTags; // Danh sách tag bị ảnh hưởng
    public Vector3 forceDirection = Vector3.up; // Hướng gió
    private Vector2 boxSize = new Vector2(1f, 1f); // Kích thước vùng tác động (hình hộp chữ nhật)
    
    public bool isFanActivated = true;
    public bool disablePermanently = false;
    public float disableDuration = 3f;
    public float baseForce = 5f; // Lực tác động

    private Coroutine countdownCoroutine;
    private Animator animator;
    public GameObject windZone;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Kiểm tra nếu đối tượng có tag hợp lệ
        if (isFanActivated && affectedTags.Contains(collision.tag))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Tính lực tối thiểu để thắng trọng lực
                float requiredForce = rb.mass * Physics2D.gravity.magnitude * rb.gravityScale;

                // Áp dụng lực (bao gồm cả baseForce để tạo hiệu ứng mạnh hơn)
                rb.AddForce(forceDirection.normalized * (baseForce + requiredForce), ForceMode2D.Force);
            }
        }
    }

    public void DisableFanWithoutCountdown()
    {
        isFanActivated = false;
        animator.SetBool("isFanActivated", isFanActivated);
        // Nếu có một countdown đang chạy, hủy nó
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
    }

    public void StartCountdownForFan()
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
        isFanActivated = true;
        animator.SetBool("isFanActivated", isFanActivated);

        // Áp dụng lực ngay lập tức cho tất cả các vật thể trong vùng tác động
        ApplyForceToAllInZone();
    }

    private void ApplyForceToAllInZone()
    {
        // Lấy danh sách tất cả các vật thể trong vùng tác động
        Collider2D[] objectsInZone = Physics2D.OverlapBoxAll(transform.position + Vector3.up * boxSize.y / 2, boxSize, 0);

        foreach (Collider2D obj in objectsInZone)
        {
            if (affectedTags.Contains(obj.tag))
            {
                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Tính toán lực cần thiết và áp dụng ngay
                    float requiredForce = rb.mass * Physics2D.gravity.magnitude * rb.gravityScale;
                    rb.AddForce(forceDirection.normalized * (baseForce + requiredForce), ForceMode2D.Force);
                }
            }
        }
    }

}