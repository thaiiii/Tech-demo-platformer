using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletType
    {
        NORMAL,
        SPECIAL
    }

    public LayerMask destroyableLayers;  // Các layer mà đạn sẽ bị hủy khi va chạm
    public float bulletDamage = 10f;
    [HideInInspector] public Sprite normalSprite;
    [HideInInspector] public Sprite specialSprite;
    private SpriteRenderer spriteRenderer;
    public BulletType type;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (type == BulletType.NORMAL)
            spriteRenderer.sprite = normalSprite;
        else if (type == BulletType.SPECIAL)
            spriteRenderer.sprite = specialSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HealthComponent healthComponent = collision.gameObject.GetComponent<HealthComponent>();
            healthComponent.TakeDamage(bulletDamage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Robot"))
        {
            HealthComponent healthComponent = collision.gameObject.GetComponent<HealthComponent>();
            healthComponent.TakeDamage(bulletDamage);
            Destroy(gameObject);
        }
        else if(collision.gameObject.CompareTag("SlimeClone"))
        {
            collision.GetComponent<SlimeClone>().KillClone();
            Destroy(gameObject);
        }

        else if (((1 << collision.gameObject.layer) & destroyableLayers) != 0)
            Destroy(gameObject);

    }



}
