using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeClone : MonoBehaviour
{
    public int slimeCount;
    [HideInInspector]public Sprite slimeSprite;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSlimeCount(int count)
    {
        slimeCount = count;
        // Điều chỉnh kích thước dựa vào số slime
        //transform.localScale = Vector3.one * (1f + (slimeCount / 32f));
    }
}
