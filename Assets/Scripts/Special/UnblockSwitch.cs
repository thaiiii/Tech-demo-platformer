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
    public Collider2D[] objectInside;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
        {
            if (blocker != null)
            {
                if (blocker.GetComponent<SpriteRenderer>().enabled)
                    blocker.GetComponent<SpriteRenderer>().enabled = false;
                if (!blocker.GetComponent<Collider2D>().isTrigger)
                    blocker.GetComponent<Collider2D>().isTrigger = true;
                StopAllCoroutines();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag) && !isPermanent)
        {
            Debug.Log(collision.name + " ra");
            StartCoroutine(ReappearBlocker());
        }
    }

    private IEnumerator ReappearBlocker()
    {
        yield return new WaitForSeconds(reappearDelay);

        //Kiểm tra va chạm trước khi bật lại
        Collider2D blockerCollider = blocker.GetComponent<Collider2D>();
        objectInside = Physics2D.OverlapBoxAll(
            blockerCollider.bounds.center,
            blockerCollider.bounds.size,
            0f);
        foreach (Collider2D obj in objectInside)
        {
            if (obj.CompareTag("Player"))
                obj.GetComponent<PlayerDeath>()?.KillPlayer();
            else if (obj.CompareTag("Enemy"))
                obj.GetComponent<Enemy>()?.KillEnemy();
            else if (obj.CompareTag("Robot"))
                obj.GetComponent<Robot>()?.OnRobotDestroyed();
            else if (obj.CompareTag("SlimeClone"))
                obj.GetComponent<SlimeClone>().KillClone();
            else if (obj.CompareTag("MassObject"))
                Destroy(obj);
        }

        blocker.GetComponent<SpriteRenderer>().enabled = true;
        blocker.GetComponent<Collider2D>().isTrigger = false;
    }

    public void LoadSavedBlockStatus()
    {
        if (savedBlockStatus)
        {
            blocker.GetComponent<SpriteRenderer>().enabled = true;
            blocker.GetComponent<Collider2D>().isTrigger = false;
        }
        else
        {
            blocker.GetComponent<SpriteRenderer>().enabled = false;
            blocker.GetComponent<Collider2D>().isTrigger = true;
        }
    }
}

