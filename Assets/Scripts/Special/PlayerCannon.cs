using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCannon : MonoBehaviour
{
    public Slider powerSlider;
    public Canvas cannonCanvas;
    [HideInInspector]public Transform cannonMuzzle; //Điểm bắn ra
    [HideInInspector]public Transform cannonPivot; //Phần xoay được của pháo
   
    public float minForce = 5f; //Lực bắn tối thiểu
    public float maxForce = 20f;
    public float force = 0f;
   
    private float chargeTime = 3f; //Thời gian nạp tối đa
    private float rotationSpeed = 100f; //Tốc độ xoay góc
    private float currentChargeTime = 0f;
    private Vector2 direction;
    private PlayerAbilities playerAbilities;

    [HideInInspector] public bool isPlayerInside = false;
    private bool isPlayerInRange = false; //Kiểm tra người chơi ở trong tầm hoạt động

    // Update is called once per frame
    void Update()
    {
        if (playerAbilities != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (isPlayerInRange && playerAbilities.CanGetInsideCannon())
                    EnterCannon();
            }

            if (isPlayerInside)
            {
                RotateCannon(); //Xoay nòng pháo
                ChargeShot(); //Nạp lực bắn

                if (Input.GetKeyUp(KeyCode.Space))
                    FirePlayer();
                if (Input.GetKeyDown(KeyCode.Q))        //Thoát khỏi cannon
                {
                    playerAbilities.ExitCannon(Vector2.up, 10f);
                    isPlayerInside = false;
                    playerAbilities.gameObject.transform.parent = null;
                }

            }
        }
    }
    public void EnterCannon()
    {
        isPlayerInside = true;
        playerAbilities.EnterCannon(transform.position);
        playerAbilities.gameObject.transform.SetParent(transform); //Thành cha của player
        
    }
    private void RotateCannon()
    {
        float rotateInput = Input.GetAxis("Vertical"); //Mũi tên lên/xuống
        cannonMuzzle.Rotate(Vector3.forward * rotateInput * rotationSpeed * Time.deltaTime);
        float angle = cannonMuzzle.eulerAngles.z;
        direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized; // Hướng bắn theo nòng pháo
    }
    private void ChargeShot()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Clamp(currentChargeTime, 0.5f, chargeTime);
            force = Mathf.Lerp(minForce, maxForce, currentChargeTime / chargeTime);
            if (isPlayerInside == true)
            {
                cannonCanvas.enabled = true;
                powerSlider.value = force / maxForce;
            }
        }
    }
    private void FirePlayer()
    {
        // Hướng bắn theo nòng pháo
        
        playerAbilities.ExitCannon(direction, force);
        playerAbilities.gameObject.transform.SetParent(null);
        isPlayerInside = false;
        currentChargeTime = 0f;
        cannonCanvas.enabled = false;
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
