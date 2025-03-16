using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite chestOpenSprite;  // Sprite khi rương mở
    public Sprite chestClosedSprite; // Sprite khi rương đóng
    private Collider2D detectionTrigger;
    private enum OpenType { Negative, Positive };
    [SerializeField] private OpenType openType;

    private ItemBase item;
    public bool isOpened = false; // Trạng thái của rương

    //Save infor
    public bool savedOpenStatus = false;
    //================================================================================
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        detectionTrigger = transform.Find("DetectionTrigger").GetComponent<Collider2D>();
        item = transform.Find("ItemInside").GetComponent<ItemBase>();
        spriteRenderer.sprite = isOpened ? chestOpenSprite : chestClosedSprite;
    }

    private void Update()
    {
        if (isOpened)
            return;
        if (detectionTrigger.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            if (openType == OpenType.Positive || openType == OpenType.Negative && Input.GetKeyDown(KeyCode.E))
                OpenChest();
        }
    }
    private void OpenChest()
    {
        isOpened = true;
        spriteRenderer.sprite = chestOpenSprite;
        item.ActiveItem();
    }
    public void LoadSavedChestStatus()
    {
        if (savedOpenStatus)
        {
            isOpened = true;
            spriteRenderer.sprite = chestOpenSprite;
        }
        else
        {
            isOpened = false;
            spriteRenderer.sprite = chestClosedSprite;
            item.spriteRenderer.enabled = false;
            item.isPicked = true;
        }
    }

}
