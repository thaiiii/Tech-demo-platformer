using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrapSwitch : MonoBehaviour
{
    public MovingTrap movingTrap;
    public List<string> applicableTags; // Các tag được cho phép kích hoạt

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
        {
            movingTrap.DisableMoving();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (applicableTags.Contains(collision.gameObject.tag))
        {
            // Khi người chơi rời khỏi Switch, bắt đầu đếm ngược
            movingTrap.EnableMoving();
        }
    }
}
