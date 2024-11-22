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

        Collider2D towerCollider = Physics2D.OverlapCircle(transform.position, teleportRadius, towerLayer);
        currentTower = (towerCollider != null) ? towerCollider.GetComponent<TeleportTower>() : null;

        if (Input.GetKeyDown(KeyCode.E) && canTeleport)
        {
            StartCoroutine(TeleportDelay());
            if (!isHidden)
            {
                TryTeleportToTower(currentTower);
            }
            else
            {
                ExitTower();
            }
        }
    }

    private void TryTeleportToTower(TeleportTower tower)
    {
        if (tower != null)
        {
            if (tower.type == TeleportTower.TowerType.TYPE_STATIC)
            {
                //Lưu lại vận tốc trước khi teleport
                storedVelocity = rb.velocity;

                // Dịch chuyển người chơi tới cây trụ
                transform.position = tower.transform.position;

                // Kích hoạt trạng thái ẩn
                tower.Activate(true);
                isHidden = true;

                //ko cho phép di chuyển, ẩn sprite renderer
                gameObject.GetComponent<Player>().LockMove();
                spriteRenderer.enabled = false;
            }
            else if (tower.type == TeleportTower.TowerType.TYPE_DYNAMIC)
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

            // Bật lại sprite, cho phép di chuyển
            gameObject.GetComponent<Player>().UnlockMove();
            spriteRenderer.enabled = true;

            //thả cao hơn một chút
            rb.transform.position = currentTower.transform.position + new Vector3(0, 1f, 0);
            rb.velocity = storedVelocity;
        }
    }

    private IEnumerator TeleportDelay()
    {
        canTeleport = false;
        yield return new WaitForSeconds(teleportDelay);
        canTeleport = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, teleportRadius);
    }
}
