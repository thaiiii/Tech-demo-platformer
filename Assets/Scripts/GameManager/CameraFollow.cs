using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    private Vector3 offset = new Vector3(0, 2, -10);

    [HideInInspector] public bool isFollowing = true;
    [Range(1, 10)]
    public float smoothFactor;
    public Vector3 minValues, maxValues;

    private void Awake()
    {
        target = FindObjectOfType<Player>().gameObject.transform;
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
}
