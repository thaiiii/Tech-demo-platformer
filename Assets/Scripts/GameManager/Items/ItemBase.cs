using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    public string itemName;
    public bool isCounted;
    public bool isPicked = false;
    
    //Tùy biến
    public int quantity;
    public bool isManualPicked;
    private bool isPlayerNearby = false;

    //Saved infor
    public bool isCheckpointPicked = false; //Vật phẩm đã được nhặt khi checkpoint thì sau khi restart sẽ mất
    [HideInInspector]public SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
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
        isPicked = false;
        spriteRenderer.enabled = true;
    }
    private void OnTriggerExit2D(Collider2D collision) => isPlayerNearby = false;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Robot"))
        {
            if (!isPlayerNearby)
                isPlayerNearby = true;
            if (!isManualPicked || isManualPicked && Input.GetKeyDown(KeyCode.E))
            {
                OnPickUp();
            }
        }
    }
}
