using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    [System.Serializable]
    public class SegmentSpeed
    {
        public int fromWaypointIndex;
        public int toWaypointIndex;
        public float speed;
        public float delay; // Thời gian chờ
    }

    public Transform[] waypoints;  // Các điểm di chuyển
    public List<SegmentSpeed> segmentSpeeds; // Danh sách tốc độ từng đoạn
    public bool isActivated = true;
    public bool loop = true;       // Có lặp lại khi đến điểm cuối không
    public bool movingForward = true;
    public bool isWaiting = false;
    public int currentWaypointIndex = 0;
    public int nextWaypointIndex;
    public float moveSpeed;

    private Transform targetWaypoint;

    private void Start()
    {
        targetWaypoint = waypoints[currentWaypointIndex];
        nextWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Điểm tiếp theo mặc định
        moveSpeed = GetCurrentSegmentSpeed();
    }

    void Update()
    {
        if (isActivated)
            Moving();
    }

    public void Moving()
    {
        if (waypoints.Length < 2 || isWaiting)
            return;

        // Kiểm tra nếu đã đến điểm tiếp theo
        if (Vector3.Distance(transform.position, targetWaypoint.position) <= 0.01f)
        {
            isWaiting = true; // Kích hoạt trạng thái chờ
            StartCoroutine(HandleWaypointDelay());
            return; // Không di chuyển trong khung hình này
        }

        // Di chuyển về phía điểm tiếp theo
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);
    }

    private float GetCurrentSegmentSpeed()
    {
        foreach (var segment in segmentSpeeds)
        {
            if (segment.fromWaypointIndex == currentWaypointIndex &&
                segment.toWaypointIndex == nextWaypointIndex)
            {
                return segment.speed;
            }
        }
        Debug.LogWarning($"Không tìm thấy tốc độ cho đoạn đường ({currentWaypointIndex}, {nextWaypointIndex})");
        return 0f; // Mặc định trả về 0 nếu không tìm thấy
    }

    private IEnumerator HandleWaypointDelay()
    {
        // Lấy delay từ danh sách segmentSpeeds
        var segment = segmentSpeeds.Find(s => s.fromWaypointIndex == currentWaypointIndex && s.toWaypointIndex == nextWaypointIndex);
        float delayTime = segment != null ? segment.delay : 0f;

        //Debug.Log($"Đợi tại waypoint {currentWaypointIndex} -> {nextWaypointIndex} trong {delayTime} giây");

        yield return new WaitForSeconds(delayTime);

        // Cập nhật waypoint và tiếp tục di chuyển
        isWaiting = false;
        currentWaypointIndex = nextWaypointIndex;

        if (loop)
        {
            nextWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
        else
        {
            if (movingForward)
            {
                nextWaypointIndex = currentWaypointIndex + 1;
                if (nextWaypointIndex >= waypoints.Length) // Nếu đạt cuối danh sách
                {
                    movingForward = false;
                    nextWaypointIndex = currentWaypointIndex - 1;
                }
            }
            else
            {
                nextWaypointIndex = currentWaypointIndex - 1;
                if (nextWaypointIndex < 0) // Nếu đạt đầu danh sách
                {
                    movingForward = true;
                    nextWaypointIndex = currentWaypointIndex + 1;
                }
            }
        }

        targetWaypoint = waypoints[nextWaypointIndex];
        moveSpeed = GetCurrentSegmentSpeed();
    }

    public void DisableMoving()
    {
        isActivated = false;
    }

    public void EnableMoving()
    {
        isActivated = true;
    }
}
