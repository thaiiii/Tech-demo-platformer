using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTower : MonoBehaviour
{
    public Sprite normalSprite;       // Hình dạng bình thường của cây trụ
    public Sprite activatedSprite;       // Hình dạng khi người chơi nhập vào
    private SpriteRenderer spriteRenderer;
    public GameObject availableMark;
    public bool isOccupied = false;  // Trạng thái bị chiếm
    public bool isAvailable = true;  // Trạng thái sẵn sàng
    public float coolDown = 3f;
    private Vector3 startPosition;
    public enum TowerType
    {
        TYPE_TELEPORT,
        TYPE_SWAP
    }
    public TowerType type;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
    }

    public void Activate(bool occupied)
    {
        isOccupied = occupied;
        spriteRenderer.sprite = isOccupied ? activatedSprite : normalSprite;
    }

    public void ResetTower()
    {
        isAvailable = true;
        transform.position = startPosition;
        Activate(false); //Trả lại trạng thái ban đầu
    }

    public IEnumerator CoolDownTower()
    {
        isAvailable = false;
        yield return new WaitForSeconds(coolDown);
        isAvailable = true;
    }
}
