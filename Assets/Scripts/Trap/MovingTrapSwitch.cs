using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrapSwitch : MonoBehaviour
{
    public MovingTrap movingTrap;

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("MassObject"))
        {
            movingTrap.DisableMoving();
            Debug.Log("a");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.gameObject.CompareTag("MassObject"))
        {
            // Khi người chơi rời khỏi Switch, bắt đầu đếm ngược
            movingTrap.EnableMoving();
        }
    }
}
