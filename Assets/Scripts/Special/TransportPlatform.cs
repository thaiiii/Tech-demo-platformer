﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportPlatform : MonoBehaviour
{
    public List<string> applicableTags; // Các tag được phép di chuyển
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
            collision.transform.SetParent(transform, true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
            collision.transform.SetParent(null);
    }
}