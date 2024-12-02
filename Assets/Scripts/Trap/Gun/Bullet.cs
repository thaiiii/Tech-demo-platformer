using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletType
    {
        NORMAL, 
        SPECIAL
    }

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
        if(collision.tag != "Player")
            Destroy(gameObject);
    }

    

}
