using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public float parallaxSpeed = 0.5f;
    public bool loop = true;

    private Transform[] parts;
    private float viewZone = 10f;
    private int leftIndex;
    private int rightIndex;
    private float backgroundSize;

    private Transform cameraTransform;
    private float lastCameraX;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraX = cameraTransform.position.x;

        backgroundSize = GetComponentInChildren<SpriteRenderer>().bounds.size.x;

        parts = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            parts[i] = transform.GetChild(i);
        }

        leftIndex = 0;
        rightIndex = parts.Length - 1;
    }

    void Update()
    {
        float deltaX = cameraTransform.position.x - lastCameraX;
        transform.position += Vector3.right * (deltaX * parallaxSpeed);
        lastCameraX = cameraTransform.position.x;

        if (!loop) return;

        if (cameraTransform.position.x < parts[leftIndex].position.x + viewZone)
            ScrollLeft();
        if (cameraTransform.position.x > parts[rightIndex].position.x - viewZone)
            ScrollRight();
    }

    private void ScrollLeft()
    {
        int lastRight = rightIndex;
        float newX = parts[leftIndex].position.x - backgroundSize;
        Vector3 newPos = new Vector3(newX, parts[rightIndex].position.y, parts[rightIndex].position.z);

        parts[rightIndex].position = newPos;

        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0) rightIndex = parts.Length - 1;
    }

    private void ScrollRight()
    {
        int lastLeft = leftIndex;
        float newX = parts[rightIndex].position.x + backgroundSize;
        Vector3 newPos = new Vector3(newX, parts[leftIndex].position.y, parts[leftIndex].position.z);

        parts[leftIndex].position = newPos;

        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex == parts.Length) leftIndex = 0;
    }
}
