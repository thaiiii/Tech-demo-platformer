using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    private GameObject conveyorPlatform;
    private List<Transform> objectsOnBelt = new List<Transform>();
    private Dictionary<Rigidbody2D, RigidbodyInterpolation2D> originalInterpolation = new Dictionary<Rigidbody2D, RigidbodyInterpolation2D>();

    public float conveyorSpeed = 3f;
    public bool moveRight = true;
    public bool isActive = true;

    public bool savedActivationStatus;
    public bool savedDirection = true;

    private void Start()
    {
        conveyorPlatform = new GameObject("PushingObject");
        conveyorPlatform.transform.position = transform.position;
        conveyorPlatform.transform.SetParent(transform);

        savedDirection = moveRight;
        savedActivationStatus = isActive;
    }

    private void Update()
    {
        if (isActive && objectsOnBelt.Count > 0)
        {
            float direction = moveRight ? 1 : -1;
            if (moveRight)
                GetComponent<SpriteRenderer>().flipX = true;
            else
                GetComponent<SpriteRenderer>().flipX=false;
            conveyorPlatform.transform.position += new Vector3(conveyorSpeed * Time.deltaTime * direction, 0, 0);
        }
        if (objectsOnBelt.Count == 0)
        {
            if (conveyorPlatform.transform.position != transform.position)
                conveyorPlatform.transform.position = transform.position;
        }
    }

    private void LateUpdate()
    {
        if (Vector3.Distance(conveyorPlatform.transform.position, transform.position) > 5f)
        {
            Vector3 offset = conveyorPlatform.transform.position - transform.position;
            conveyorPlatform.transform.position = transform.position;

            foreach (Transform obj in objectsOnBelt)
            {
                obj.position += offset;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.rigidbody;
        if (rb != null)
        {
            // Lưu lại trạng thái interpolate hiện tại
            if (!originalInterpolation.ContainsKey(rb))
            {
                originalInterpolation.Add(rb, rb.interpolation);
                rb.interpolation = RigidbodyInterpolation2D.None;
            }

            collision.transform.SetParent(conveyorPlatform.transform);
            if (!objectsOnBelt.Contains(collision.transform))
                objectsOnBelt.Add(collision.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.rigidbody;
        if (rb != null)
        {
            if (originalInterpolation.ContainsKey(rb))
            {
                rb.interpolation = originalInterpolation[rb]; // Phục hồi lại interpolate gốc
                originalInterpolation.Remove(rb);
            }
        }

        if (objectsOnBelt.Contains(collision.transform) && collision.transform.parent != null)
        {
            collision.transform.SetParent(null);
            objectsOnBelt.Remove(collision.transform);
        }
    }

    public void LoadSavedConveyorBeltStatus()
    {
        isActive = savedActivationStatus;
        moveRight = savedDirection;
    }

    public void SwitchDirection() => moveRight = !moveRight;
}
