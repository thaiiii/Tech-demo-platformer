﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private SpriteRenderer spriteRenderer;

    [Header("Teleport")]
    public float teleportRadius = 10f;
    public LayerMask towerLayer;
    public bool isHidden = false;
    public TeleportTower currentTower;
    public Collider2D[] towersInRange;
    public float teleportDelay = 0.5f;
    public Vector2 storedVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleTeleport();
        if (isHidden)
        {
            transform.position = currentTower.transform.position;
            gameObject.GetComponent<Player>().LockMove();
            Debug.Log(rb.velocity);
        }
            
    }

    private void HandleTeleport()
    {
        //Lấy tất cả trụ trong tầm tele
        towersInRange = Physics2D.OverlapCircleAll(transform.position, teleportRadius, towerLayer);
        //Kiểm tra trụ gần nhất để teleport
        TeleportTower targetTower = GetClosestTower(towersInRange);

        if (Input.GetKeyDown(KeyCode.Space))
            ExitTower();
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHidden)
            {
                //Nếu chưa ẩn, teleport hoặc swap
                TryTeleportToTower(targetTower);
                TrySwapTower(targetTower);
            }
            else
            {

                if (targetTower == null)
                {
                    //Nếu dang ẩn, ko có trụ khác xung quanh = thoát ra
                    ExitTower();
                }
                else
                {
                    if (targetTower.type == TeleportTower.TowerType.TYPE_TELEPORT)
                    {
                        //Nếu đang ẩn, có trụ xung quanh, tele trụ mới
                        currentTower.Activate(false);
                        TryTeleportToTower(targetTower);
                    }
                    else if (targetTower.type == TeleportTower.TowerType.TYPE_SWAP)
                        TrySwapTower(targetTower);
                }
                }
            }
        }

    private TeleportTower GetClosestTower(Collider2D[] towersInRange)
    {
        TeleportTower closestAvailableTower = null;
        float closestDistance = teleportRadius;

        // Lấy toàn bộ tower trong game
        TeleportTower[] allTowers = FindObjectsOfType<TeleportTower>();
        foreach (var tower in allTowers)
        {
            // Đảm bảo tất cả availableMark đều bị tắt trước
            tower.availableMark.GetComponent<SpriteRenderer>().enabled = false;
        }


        foreach (var towerCollider in towersInRange)
        {
            TeleportTower tower = towerCollider.GetComponent<TeleportTower>();
            if (tower != null && (!isHidden || tower != currentTower))
            {
                float distance = Vector2.Distance(transform.position, tower.transform.position);
                if (distance < closestDistance && tower.isAvailable)
                {
                    closestAvailableTower = tower;
                    closestDistance = distance;
                }
            }
        }
        // Nếu tìm được tower available gần nhất, bật availableMark của nó
        if (closestAvailableTower != null)
            closestAvailableTower.availableMark.GetComponent<SpriteRenderer>().enabled = true;
        return closestAvailableTower;
    }

    private void TryTeleportToTower(TeleportTower tower)
    {
        if (currentTower != null)
        {
            // Gọi ExitTower để đảm bảo tower hiện tại cooldown
            ExitTower();
        }

        if (tower != null && tower.isAvailable && tower.type == TeleportTower.TowerType.TYPE_TELEPORT) //Kiểm tra nếu tower teleport khả dụng
        {
            //Lưu lại vận tốc trước khi teleport
            storedVelocity = rb.velocity;

            // Set parent vào cây trụ, chuyển collider của player sang isTrigger
            transform.SetParent(tower.transform);
            transform.position = tower.transform.position + Vector3.up;

            playerCollider.isTrigger = true;

            // Kích hoạt trạng thái ẩn, cập nhật trụ hiện tại
            tower.Activate(true);
            currentTower = tower;
            isHidden = true;

            //ẩn sprite renderer
            spriteRenderer.enabled = false;
        }
    }

    private void TrySwapTower(TeleportTower tower)
    {
        if (tower != null && tower.isAvailable && tower.type == TeleportTower.TowerType.TYPE_SWAP) //Kiểm tra nếu tower swap khả dụng
        {
            if (!isHidden)
            {
                Vector3 tempPos = transform.position;
                transform.position = tower.transform.position;
                tower.transform.position = tempPos;
            }
            else
            {
                Vector3 tempPos = currentTower.transform.position;
                currentTower.transform.position = tower.transform.position;
                tower.transform.position = tempPos;
            }
        }
    }
    public void ExitTower()
    {
        if (currentTower != null)
        {
            // Bắt đầu cooldown cho tower
            StartCoroutine(currentTower.CoolDownTower());

            // Hủy liên kết với tower hiện tại
            transform.SetParent(null);
            currentTower.Activate(false);
            isHidden = false;

            // Bật lại sprite, tắt isTrigger, cho phép di chuyển
            gameObject.GetComponent<Player>().UnlockMove();
            playerCollider.isTrigger = false;
            spriteRenderer.enabled = true;

            // Thả người chơi cao hơn một chút
            rb.transform.position = currentTower.transform.position + new Vector3(0, 1.5f, 0);
            currentTower = null;

            // Khôi phục vận tốc trước khi teleport
            rb.velocity = storedVelocity;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, teleportRadius);
    //}
}
