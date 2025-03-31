using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSwitch : MonoBehaviour
{
    public bool switchSound = true;
    public LaserEmitter laserEmitter;
    public List<string> applicableTags;

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
        {
            if (switchSound)
                AudioManager.Instance.PlaySFX("switch");
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
        {
            // Tắt laser nhưng không bắt đầu đếm ngược ngay
            laserEmitter.DisableLaserWithoutCountdown();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
        {
            // Khi người chơi rời khỏi Switch, bắt đầu đếm ngược
            laserEmitter.StartCountdownForLaser();
        }
    }


}
