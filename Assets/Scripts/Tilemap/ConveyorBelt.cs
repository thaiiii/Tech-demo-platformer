using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    private GameObject conveyorPlatform;
    private List<Transform> objectsOnBelt = new List<Transform>(); //list các vật thể trên băng

    public float conveyorSpeed = 3f; // Tốc độ băng chuyền
    public bool moveRight = true; // Hướng băng chuyền
    public bool isActive = true;

    //Save info
    public bool savedActivationStatus;
    public bool savedDirection = true;


    private void Start()
    {
        // Tạo một object vô hình để giữ các vật thể trên băng chuyền
        conveyorPlatform = new GameObject("PushingObject");
        conveyorPlatform.transform.position = transform.position;
        conveyorPlatform.transform.SetParent(transform);

        //First save
        savedDirection = moveRight;
        savedActivationStatus = isActive;
    }

    private void Update()
    {
        if (isActive && objectsOnBelt.Count > 0)
        {
            // Di chuyển nền tảng vô hình theo hướng băng chuyền
            float direction = moveRight ? 1 : -1;
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

            //Di chuyển các vật thể về đúng chỗ tương ứng
            foreach (Transform obj in objectsOnBelt)
            {
                obj.position += offset;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Khi vật thể chạm vào băng chuyền, đặt nó làm con của ConveyorPlatform
        collision.transform.SetParent(conveyorPlatform.transform);
        objectsOnBelt.Add(collision.transform);

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (objectsOnBelt.Contains(collision.transform) && collision.transform.parent != null)
        {
            // Khi rời băng chuyền, gỡ khỏi ConveyorPlatform
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
