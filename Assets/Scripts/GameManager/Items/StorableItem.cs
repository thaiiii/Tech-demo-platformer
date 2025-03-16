using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorableItem : ItemBase
{
    private InventoryManager inventoryManager;
    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public override void OnPickUp()
    {
        if (isPicked)
            return;
        
        if (!inventoryManager.isFull)
        {   
            inventoryManager.AddItem(GetComponent<SpriteRenderer>().sprite, itemName, quantity, isCounted);
            isPicked = true;
            spriteRenderer.enabled = false;
        }
    }
}
