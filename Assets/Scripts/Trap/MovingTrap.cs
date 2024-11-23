using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    public Transform[] waypoints;  // Các điểm di chuyển
    public float moveSpeed = 2f;   // Tốc độ di chuyển
    public bool loop = true;       // Có lặp lại khi đến điểm cuối không
    public bool isActivated = true;

    private int currentWaypointIndex = 0;
    private bool movingForward = true;

    // Update is called once per frame
    void Update()
    {
        if (isActivated)
            Moving();
    }

    public void Moving()
    {
        if (waypoints.Length < 2)
            return;


        // Di chuyển về phía điểm tiếp theo
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        // Kiểm tra nếu đã đến điểm tiếp theo
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            if (loop)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            else
            {
                // Đổi chiều di chuyển nếu không lặp
                if (movingForward)
                {
                    if (currentWaypointIndex < waypoints.Length - 1)
                        currentWaypointIndex++;
                    else
                    {
                        movingForward = false;
                        currentWaypointIndex--;
                    }
                }
                else
                {
                    if (currentWaypointIndex > 0)
                        currentWaypointIndex--;
                    else
                    {
                        movingForward = true;
                        currentWaypointIndex++;
                    }
                }
            }
        }
    }

    public void DisableMoving()
    {
        isActivated = false;
        gameObject.GetComponent<MovingTrap>().enabled = false;
    }

    public void EnableMoving()
    {
        isActivated = true;
        gameObject.GetComponent<MovingTrap>().enabled = true;
    }
}
