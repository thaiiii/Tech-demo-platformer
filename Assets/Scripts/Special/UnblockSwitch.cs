using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnblockSwitch : MonoBehaviour
{
    public GameObject blocker;
    public bool isPermanent = false;
    public bool savedBlockStatus = true;
    public float reappearDelay = 0f;
    public List<string> applicableTags; // Các tag được phép di chuyển

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
        {
            if (blocker != null)
            {
                blocker.SetActive(!blocker.activeSelf);
                StopAllCoroutines();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag) && !isPermanent)
        {
            StartCoroutine(ReappearBlocker());
        }
    }

    private IEnumerator ReappearBlocker()
    {
        yield return new WaitForSeconds(reappearDelay);
        if (blocker != null)
        {
            blocker.SetActive(true);
        }
    }

    public void LoadSavedBlockStatus()
    {
        if (savedBlockStatus)
            blocker.SetActive(true);
        else
            blocker.SetActive(false);
    }
}

