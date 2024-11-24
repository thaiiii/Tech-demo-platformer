using System;
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
    public bool canTeleport = true;
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

    }

    private void HandleTeleport()
    {
        //Lấy tất cả trụ trong tầm tele
        towersInRange = Physics2D.OverlapCircleAll(transform.position, teleportRadius, towerLayer);
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ExitTower();
        }

        if (Input.GetKeyDown(KeyCode.E) && canTeleport)
        {
            StartCoroutine(TeleportDelay());
            //Kiểm tra trụ gần nhất để teleport
            TeleportTower targetTower = GetClosestTower(towersInRange);
            Debug.Log(targetTower);
            if (!isHidden)
            {
               //Nếu chưa ẩn, teleport 
                TryTeleportToTower(targetTower);
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
                    //Nếu đang ẩn, có trụ xung quanh, tele trụ mới
                    currentTower.Activate(false);
                    TryTeleportToTower(targetTower);
                }
            }
        }
    }

    private TeleportTower GetClosestTower(Collider2D[] towersInRange)
    {
        TeleportTower closestTower = null;
        float closestDistance = teleportRadius;

        foreach (var towerCollider in towersInRange)
        {
            TeleportTower tower = towerCollider.GetComponent<TeleportTower>();
            if (tower != null && (!isHidden || tower != currentTower))
            {
                float distance = Vector2.Distance(transform.position, tower.transform.position);
                if (distance < closestDistance)
                {
                    closestTower = tower;
                    closestDistance = distance;
                }
            }
        }
        
        return closestTower;
    }

    private void TryTeleportToTower(TeleportTower tower)
    {
        if (tower != null)
        {
            if (tower.type == TeleportTower.TowerType.TYPE_TELEPORT)
            {
                //Lưu lại vận tốc trước khi teleport
                storedVelocity = rb.velocity;

                // Dịch chuyển người chơi tới cây trụ, chuyển collider của player sang isTrigger
                transform.position = tower.transform.position + Vector3.up;
                playerCollider.isTrigger = true;

                // Kích hoạt trạng thái ẩn, cập nhật trụ hiện tại
                tower.Activate(true);
                currentTower = tower;
                isHidden = true;

                //ko cho phép di chuyển, ẩn sprite renderer
                gameObject.GetComponent<Player>().LockMove();
                spriteRenderer.enabled = false;
            }
            else if (tower.type == TeleportTower.TowerType.TYPE_SWAP)
            {
                Vector3 tempPos = transform.position;
                transform.position = tower.transform.position;
                tower.transform.position = tempPos;
            }

        }
    }

    private void ExitTower()
    {
        if (currentTower != null)
        {
            currentTower.Activate(false);
            isHidden = false;

            // Bật lại sprite, tắt isTrigger, cho phép di chuyển
            gameObject.GetComponent<Player>().UnlockMove();
            playerCollider.isTrigger = false;
            spriteRenderer.enabled = true;

            //thả cao hơn một chút
            rb.transform.position = currentTower.transform.position + new Vector3(0, 2f, 0);
            currentTower = null;
            rb.velocity = storedVelocity;
        }
    }

    private IEnumerator TeleportDelay()
    {
        canTeleport = false;
        yield return new WaitForSeconds(teleportDelay);
        canTeleport = true;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, teleportRadius);
    //}
}
