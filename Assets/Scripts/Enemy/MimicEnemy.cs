using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MimicEnemy : MonoBehaviour
{
    private enum MimicState { Disguised, Revealed }
    private enum MimicType { Positive, Negative }
    [SerializeField] private MimicState currentState = MimicState.Disguised;
    [SerializeField] private MimicType type;
    private Collider2D detectionTrigger;

    public float returnToDisguiseTime = 3f; // Thời gian quay lại trạng thái ẩn
    public float timeSinceLastSeenPlayer = 0f;

    private SpriteRenderer spriteRenderer;
    private PlayerAbilities playerAbilities;
    public Sprite disguisedSprite; // Sprite khi giả dạng
    public Sprite revealedSprite; // Sprite khi hiện hình

    private bool isReturningToDisguise = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = disguisedSprite; // Ban đầu là vật thể giả dạng
        detectionTrigger = transform.Find("DetectionTrigger").GetComponent<Collider2D>();

        playerAbilities = FindObjectOfType<PlayerAbilities>();
    }

    private void Update()
    {
        if (!GetComponent<Collider2D>().isTrigger)
            GetComponent<Collider2D>().isTrigger = true;
        if (!GetComponent<Rigidbody2D>().isKinematic)
            GetComponent<Rigidbody2D>().isKinematic = true;
        if (!GetComponent<Enemy>().isEnemyDead)
        {
            switch (currentState)
            {
                case MimicState.Disguised:
                    if (CheckAround())
                    {
                        if (type == MimicType.Positive || type == MimicType.Negative && Input.GetKeyDown(KeyCode.E))
                        {
                            RevealMimic();
                        }
                    }
                    break;

                case MimicState.Revealed:
                    CoolDownDisguise();
                    AttackPlayer();
                    break;
            }
        }
    }

    private bool CheckAround()
    {
        if (detectionTrigger.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            playerAbilities.isNearChest = true;
            return true;
        }
        else
            return false;
    }
    private void RevealMimic()
    {
        if (gameObject.tag != "Enemy")
            gameObject.tag = "Enemy";
        if (isReturningToDisguise) return;

        currentState = MimicState.Revealed;
        spriteRenderer.sprite = revealedSprite;
        //animator.SetTrigger("Reveal");

    }
    private void AttackPlayer()
    {
        
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
        if (currentState == MimicState.Disguised) return;

        timeSinceLastSeenPlayer = 0f;
        isReturningToDisguise = true;
        ReturnToDisguise();
    }
    private void ReturnToDisguise()
    {
        if (gameObject.tag != "Untagged")
            gameObject.tag = "Untagged";
        spriteRenderer.sprite = disguisedSprite;
        currentState = MimicState.Disguised;
        isReturningToDisguise = false;
    }
}
