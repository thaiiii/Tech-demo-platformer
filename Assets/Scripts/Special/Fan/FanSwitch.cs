using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanSwitch : MonoBehaviour
{
    public Fan fan;
    public List<string> applicableTags;
    public float switchOffDelay = 3f; // Thời gian chờ trước khi tắt quạt nếu không có vật nào chạm vào

    private int objectsOnSwitch = 0;
    private Coroutine turnOffCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
        {
            objectsOnSwitch++;
            AudioManager.Instance.PlaySFX("switch");
            if (fan != null)
            {
                fan.EnableFan();
                if (turnOffCoroutine != null)
                {
                    StopCoroutine(turnOffCoroutine);
                    turnOffCoroutine = null;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
        {
            if (applicableTags.Contains(collision.gameObject.tag))
            {
                objectsOnSwitch--;
                AudioManager.Instance.PlaySFX("switch");
                if (objectsOnSwitch <= 0 && fan != null)
                {
                    turnOffCoroutine = StartCoroutine(TurnOffFanAfterDelay());
                }
            }
        }
    }

    private IEnumerator TurnOffFanAfterDelay()
    {
        yield return new WaitForSeconds(switchOffDelay);
        fan.DisableFan();
    }
}
