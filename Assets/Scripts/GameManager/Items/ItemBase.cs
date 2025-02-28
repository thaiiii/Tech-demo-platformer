using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    public string itemName;
    public bool isCounted;
    private Vector3 itemPosition;
    public bool isCheckpointPicked = false; //Vật phẩm đã được nhặt khi checkpoint thì sau khi restart sẽ mất

    //Tùy biến
    public int quantity;
    public bool isManualPicked;
    private bool isPlayerNearby = false;
    private void Start()
    {
        itemPosition = transform.position;
    }

    private void Update()
    {
        // Nếu isManualPicked = true, chỉ nhặt khi bấm E và đang ở gần item
        if (isPlayerNearby && isManualPicked && Input.GetKeyDown(KeyCode.E))
        {
            OnPickUp();
        }
    }

    public abstract void OnPickUp();
    public void ActiveItem()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isManualPicked)
                OnPickUp();
            else 
                isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerNearby = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (!isManualPicked || isManualPicked && Input.GetKeyDown(KeyCode.E))
            {
                OnPickUp();
                Debug.Log("bbb");
            }
        }
    }

    public void PickToSaveInventory()
    {
        isCheckpointPicked = true;
    }
}
