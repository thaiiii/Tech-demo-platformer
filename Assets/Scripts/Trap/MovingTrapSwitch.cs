using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrapSwitch : MonoBehaviour
{
    public MovingTrap movingTrap;
    public List<string> applicableTags; // Các tag được cho phép kích hoạt


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
        {
            AudioManager.Instance.PlaySFX("switch");
            movingTrap.DisableMovingWithoutCountdown();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
        {
            // Khi người chơi rời khỏi Switch, bắt đầu đếm ngược
            movingTrap.StartCountdownForMoving();
        }
    }
}
