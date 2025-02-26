using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCannon : MonoBehaviour
{
    public Transform cannonMuzzle; //Điểm bắn ra
    public Transform cannonPivot; //Phần xoay được của pháo
    public float rotationSpeed = 100f; //Tốc độ xoay góc
    public float minForce = 5f; //Lực bắn tối thiểu
    public float maxForce = 20f;
    public float chargeTime = 3f; //Thời gian nạp tối đa

    public PlayerAbilities playerAbilities;
    public bool isPlayerInside = false;
    public float currentChargeTime = 0f;
    public bool isPlayerInRange = false; //Kiểm tra người chơi ở trong tầm hoạt động

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerAbilities == null)
                return;
            if(isPlayerInRange && !isPlayerInside)
                EnterCannon();
        }

        if (isPlayerInside)
        {
            RotateCannon(); //Xoay nòng pháo
            ChargeShot(); //Nạp lực bắn

            if (Input.GetKeyDown(KeyCode.E))
            {
                FirePlayer();
            }
        }
    }

    private void RotateCannon()
    {
        float rotateInput = Input.GetAxis("Vertical"); //Mũi tên lên/xuống
        cannonPivot.Rotate(Vector3.forward * -rotateInput * rotationSpeed * Time.deltaTime);
    }
    private void ChargeShot()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Clamp(currentChargeTime, 0, chargeTime);
        }
    }
    public void EnterCannon()
    {
        isPlayerInside = true;
        playerAbilities.EnterCannon(transform.position);
    }
    private void FirePlayer()
    {
        float force = Mathf.Lerp(minForce, maxForce, currentChargeTime / chargeTime);
        Vector2 direction = cannonMuzzle.right; // Hướng bắn theo nòng pháo
        playerAbilities.ExitCannon(direction, force);
        isPlayerInside = false;
        currentChargeTime = 0f;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerAbilities = FindObjectOfType<PlayerAbilities>();
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerAbilities = null;
        }
    }
}
