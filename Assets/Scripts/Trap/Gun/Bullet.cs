using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletType
    {
        NORMAL, 
        SPECIAL
    }

    [Header("Chỉ định Layer để hủy đạn")]
    public LayerMask destroyableLayers;  // Các layer mà đạn sẽ bị hủy khi va chạm

    public Sprite normalSprite;
    public Sprite specialSprite;
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
        if (((1 << collision.gameObject.layer) & destroyableLayers) != 0)
            Destroy(gameObject);
    }

    

}
