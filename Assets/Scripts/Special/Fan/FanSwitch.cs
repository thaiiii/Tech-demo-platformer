using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanSwitch : MonoBehaviour
{
    public Fan fan;
    public List<string> applicableTags;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
        {
            // Tắt fan nhưng không bắt đầu đếm ngược ngay
            if(fan != null)
            fan.DisableFanWithoutCountdown();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
        {
            // Khi người chơi rời khỏi Switch, bắt đầu đếm ngược
            if (fan != null && fan.gameObject.activeSelf) // Kiểm tra Fan đang active
            {
                fan.StartCountdownForFan();
            }

        }
    }
}
