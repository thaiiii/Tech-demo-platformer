using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    private GameObject conveyorPlatform;
    
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

        //First save
        savedDirection = moveRight;
        savedActivationStatus = isActive;
    }

    private void Update()
    {
        if (isActive)
        {
            // Di chuyển nền tảng vô hình theo hướng băng chuyền
            float direction = moveRight ? 1 : -1;
            conveyorPlatform.transform.position += new Vector3(conveyorSpeed * Time.deltaTime * direction, 0, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActive)
        {
            // Khi vật thể chạm vào băng chuyền, đặt nó làm con của ConveyorPlatform
            collision.transform.SetParent(conveyorPlatform.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isActive)
        {
            // Khi rời băng chuyền, gỡ khỏi ConveyorPlatform
            collision.transform.SetParent(null);
        }
    }

    public void LoadSavedConveyorBeltStatus()
    {
        isActive = savedActivationStatus;
        moveRight = savedDirection;
    }
    public void SwitchDirection()
    {
        moveRight = !moveRight;
    }
}
