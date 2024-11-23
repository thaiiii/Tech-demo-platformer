using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTower : MonoBehaviour
{
    public Sprite normalSprite;       // Hình dạng bình thường của cây trụ
    public Sprite activatedSprite;       // Hình dạng khi người chơi nhập vào
    private SpriteRenderer spriteRenderer;
    public bool isOccupied = false;  // Trạng thái cây trụ
    public enum TowerType
    {
        TYPE_TELEPORT,
        TYPE_SWAP
    }
    public TowerType type;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Activate(bool occupied)
    {
        isOccupied = occupied;
        spriteRenderer.sprite = isOccupied ? activatedSprite : normalSprite;
    }
}
