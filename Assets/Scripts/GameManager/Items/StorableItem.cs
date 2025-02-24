using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorableItem : ItemBase
{
    public override void OnPickUp()
    {
            InventoryManager.Instance.AddItem(GetComponent<SpriteRenderer>().sprite ,itemName, 1, isCounted);
        gameObject.SetActive(false);
    }
}
