using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerCannon : MonoBehaviour
{
    public Slider powerSlider;
    public Canvas cannonCanvas;
    [HideInInspector] public Transform cannonMuzzle; //Điểm bắn ra
    [HideInInspector] public Transform cannonPivot; //Phần xoay được của pháo

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

    [Header("Camera")]
    public Vector3 cannonCameraOffset;
    private Vector3 originalCameraOffset;
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;
    private void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>(); // Tìm camera
        if (virtualCamera != null)
        {
            transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (transposer != null)
            {
                originalCameraOffset = new Vector3(
                transposer.m_TrackedObjectOffset.x,
                transposer.m_TrackedObjectOffset.y,
                -10f // Hoặc giá trị Z bạn muốn giữ cố định
            );
            }
        }
    }
    void Update()
    {
        if (playerAbilities != null)
        {
            SetCamera();
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (isPlayerInRange && playerAbilities.CanGetInsideCannon())
                    EnterThisCannon();
            }

            if (isPlayerInside)
            {
                if (FindObjectOfType<PauseMenu>().isPaused || FindObjectOfType<LevelCompleteMenu>().isComplete)
                    return;
                playerAbilities.transform.position = transform.position;
                RotateCannon(); //Xoay nòng pháo
                ChargeShot(); //Nạp lực bắn

                //if (Input.GetKeyUp(KeyCode.Space))
                //    FirePlayer(direction, force);
                if (Input.GetKeyDown(KeyCode.Q))        //Thoát khỏi cannon
                {
                    FirePlayer(Vector2.up, 10f);
                    isPlayerInside = false;
                    playerAbilities.gameObject.transform.parent = null;
                }
            }
        }
    }
    public void EnterThisCannon()
    {
        if (!playerAbilities.isNormalStatus())
        {
            return;
        }
            
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
        //Tính camera offset
        cannonCameraOffset = direction; 

    }
    private void ChargeShot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentChargeTime = 0f;
        }
        else if (Input.GetKey(KeyCode.Space))
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
        else if (!Input.GetKey(KeyCode.Space))
        {
            cannonCanvas.enabled = false;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {

            FirePlayer(direction, force);
        }
    }
    public void FirePlayer(Vector2 direction, float force)
    {
        if (playerAbilities == null)
            return;
        // Hướng bắn theo nòng pháo
        playerAbilities.ExitCannon(direction, force);
        playerAbilities.gameObject.transform.SetParent(null);
        isPlayerInside = false;
        currentChargeTime = 0f;
        this.force = 0;
        cannonCanvas.enabled = false;
    }
    private void SetCamera()
    {
        if (virtualCamera == null) return; // Kiểm tra nếu camera không tồn tại
        if (transposer == null) return;


        if (isPlayerInside)
        {
            if (force / 2 > 7.5)
            {
                transposer.m_TrackedObjectOffset = cannonCameraOffset * (force / 2); // Thay đổi offset
                SetCameraSize(force / 2, 0f);
                
            }
        }
        else
        {
            SetCameraSize(7.5f, 1f);
            transposer.m_TrackedObjectOffset = originalCameraOffset;
        }
    }
    private void SetCameraSize(float targetSize, float duration)
    {
        if (virtualCamera != null)
        {
            StartCoroutine(SmoothZoom(targetSize, duration));
        }
    }
    private IEnumerator SmoothZoom(float targetSize, float duration)
    {
        float startSize = virtualCamera.m_Lens.OrthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        virtualCamera.m_Lens.OrthographicSize = targetSize; // Đảm bảo set đúng size cuối cùng
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerAbilities = FindObjectOfType<PlayerAbilities>();
            isPlayerInRange = true;
            playerAbilities.isNearCannon = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerAbilities.isNearCannon = false;
            playerAbilities = null;
        }
    }
}
