using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    public string itemName;
    public bool isCounted;
    private Vector3 itemPosition;

    private void Start()
    {
        itemPosition = transform.position;
    }

    public abstract void OnPickUp();
    public void ActiveItem()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            OnPickUp();
        
    }
}
