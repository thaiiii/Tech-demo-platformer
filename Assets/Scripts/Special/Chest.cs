using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite chestOpenSprite;  // Sprite khi rương mở
    public Sprite chestClosedSprite; // Sprite khi rương đóng
    private bool isPlayerNearBy;
    private SpriteRenderer mark;
    private PlayerAbilities player;
    private enum OpenType { Negative, Positive };
    [SerializeField] private OpenType openType;

    private ItemBase item;
    public bool isOpened = false; // Trạng thái của rương

    //Save infor
    public bool savedOpenStatus = false;
    //================================================================================
    private void Start()
    {
        mark = transform.Find("InteractionMark").GetComponent<SpriteRenderer>();
        mark.enabled = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        item = transform.Find("ItemInside").GetComponent<ItemBase>();
        spriteRenderer.sprite = isOpened ? chestOpenSprite : chestClosedSprite;
    }

    private void Update()
    {
        if (player != null)
        {
            if (isOpened)
            {
                player.isNearChest = false;
                player = null;
                return;
            }
                
            if (isPlayerNearBy)
            {
                mark.enabled = true;
                player.isNearChest = true;
                if (openType == OpenType.Positive || openType == OpenType.Negative && Input.GetKeyDown(KeyCode.E) && player.CanOpenChest())
                    OpenChest();
            }
            else
            {
                mark.enabled = false;
                player.isNearChest = false;
                player = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isOpened)
        {
            isPlayerNearBy = true;
            player = collision.GetComponent<PlayerAbilities>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerNearBy = false;
    }

    private void OpenChest()
    {
        isOpened = true;
        spriteRenderer.sprite = chestOpenSprite;
        item.ActiveItem();
        mark.enabled = false;
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
