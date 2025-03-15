using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MimicEnemy : MonoBehaviour
{
    public enum MimicState { Disguised, Revealed }
    public MimicState currentState = MimicState.Disguised;
    private Collider2D detectionTrigger;

    public float attackSpeed = 3f; // Tốc độ khi đuổi theo
    public float returnToDisguiseTime = 3f; // Thời gian quay lại trạng thái ẩn
    public float timeSinceLastSeenPlayer = 0f;

    //private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public Sprite disguisedSprite; // Sprite khi giả dạng
    public Sprite revealedSprite; // Sprite khi hiện hình

    private bool isReturningToDisguise = false;

    private void Awake()
    {
        //animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = disguisedSprite; // Ban đầu là vật thể giả dạng
        detectionTrigger = transform.Find("DetectionTrigger").GetComponent<Collider2D>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case MimicState.Disguised:
                if (CheckAround())
                    RevealMimic();
                break;

            case MimicState.Revealed:
                CoolDownDisguise();
                AttackPlayer();
                break;
        }
    }

    private bool CheckAround()
    {
        if (detectionTrigger.IsTouchingLayers(LayerMask.GetMask("Player")))
            return true;
        else 
            return false;
    }
    private void RevealMimic()
    {
        if (isReturningToDisguise) return;

        currentState = MimicState.Revealed;
        spriteRenderer.sprite = revealedSprite;
        //animator.SetTrigger("Reveal");

    }
    private void AttackPlayer()
    {
        gameObject.tag = "Enemy";
    }
    private void CoolDownDisguise()
    {
        if (!CheckAround())
        {
            timeSinceLastSeenPlayer += Time.deltaTime;
            if (timeSinceLastSeenPlayer >= returnToDisguiseTime)
            {
                HideMimic();
            }
        }
        else
        {
            timeSinceLastSeenPlayer = 0;
        }
    }
    private void HideMimic()
    {
        gameObject.tag = "Untagged";
        if (currentState == MimicState.Disguised) return;

        timeSinceLastSeenPlayer = 0f;
        isReturningToDisguise = true;
        ReturnToDisguise();
    }
    private void ReturnToDisguise()
    {
        spriteRenderer.sprite = disguisedSprite;
        currentState = MimicState.Disguised;
        rb.velocity = Vector2.zero;
        isReturningToDisguise = false;
    }
}
