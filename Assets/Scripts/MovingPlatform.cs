using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Transform> points;
    public Transform platform;
    int goalPoint = 0;
    public float moveSpeed = 2f;

    private void Update()
    {
        MoveToNextPoint();
    }

    void MoveToNextPoint()
    {
        //change the position of the platform 
        //check if in very close to next point, change to next point
        platform.position = Vector2.MoveTowards(platform.position, points[goalPoint].position, moveSpeed * Time.deltaTime);
        if (Vector2.Distance(platform.position, points[goalPoint].position) < 0.1f)
        {
            //If so change goal point to the next one
            if (goalPoint == points.Count - 1)
                goalPoint = 0;
            else
                goalPoint++;



        }


    }
}
