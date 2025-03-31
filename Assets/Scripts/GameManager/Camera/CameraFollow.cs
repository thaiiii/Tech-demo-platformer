using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 3, -10);
    private Camera mainCamera;

    [HideInInspector] public bool isFollowing = true;
    [Range(1, 10)]
    public float smoothFactor;
    public Vector3 minValues, maxValues;

    private void Awake()
    {
        target = FindObjectOfType<Player>().gameObject.transform;
        mainCamera = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        if (isFollowing)
            Follow();
    }
    
    private void Follow()
    {
        Vector3 targetPosition = target.position + offset;

        Vector3 boundPosition = new Vector3(
                Mathf.Clamp(targetPosition.x, minValues.x, maxValues.x),
                Mathf.Clamp(targetPosition.y, minValues.y, maxValues.y),
                Mathf.Clamp(targetPosition.z, minValues.z, maxValues.z));
        Vector3 smoothPosition = Vector3.Lerp(transform.position, boundPosition, smoothFactor * Time.fixedDeltaTime);
        transform.position = smoothPosition;
    }
    public void SetFollowTarget(GameObject newTarget) => target = newTarget.transform;
    public void SetSmoothFactor(float value) => smoothFactor = value;
    public void SetCameraOffset(Vector3 newOffsetvalue) => offset = newOffsetvalue;
    public void SetCameraSize(float value, float duration)
    {
        if (duration != 0)
            StartCoroutine(SmoothZoom(value, duration));
        else
            mainCamera.orthographicSize = value;
    }
    private IEnumerator SmoothZoom(float targetSize, float duration)
    {
        float startSize = mainCamera.orthographicSize;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.orthographicSize = targetSize; // Đảm bảo kích thước đúng sau khi hoàn tất
    }
    
}
