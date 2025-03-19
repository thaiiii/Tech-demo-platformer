using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeClone : MonoBehaviour
{
    public int slimeCount;
    [HideInInspector]public Sprite slimeSprite;
    public SpriteRenderer mark;
    private SpriteRenderer spriteRenderer;

    public bool isSaved = false; //trạng thái được checkpoint của clone
    public Vector3 checkpointPosition;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mark = transform.Find("InteractionMark").GetComponent<SpriteRenderer>();
    }
    public void SetSlimeCount(int count)
    {
        slimeCount = count;
        // Điều chỉnh kích thước dựa vào số slime
        //transform.localScale = Vector3.one * (1f + (slimeCount / 32f));
    }
    public void SaveClone()
    {
        isSaved = true;
        checkpointPosition = transform.position;
    }
    public void KillClone()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            KillClone();
        }
    }
}
