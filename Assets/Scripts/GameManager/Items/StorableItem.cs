using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorableItem : ItemBase
{
    private InventoryManager inventoryManager;
    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
    }
    public override void OnPickUp()
    {
        inventoryManager.AddItem(GetComponent<SpriteRenderer>().sprite, itemName, quantity, isCounted);
        if (!inventoryManager.isFull)
            gameObject.SetActive(false);
    }
}
