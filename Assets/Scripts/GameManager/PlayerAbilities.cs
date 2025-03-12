using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static InventoryManager;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class PlayerAbilities : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private SpriteRenderer spriteRenderer;
    private HealthComponent healthComponent;
    private GameTimer gameTimer;

    [Header("Teleport")]
    public float teleportRadius = 10f;
    public LayerMask towerLayer;
    public bool isHidden = false;
    [HideInInspector] public TeleportTower currentTower;
    [HideInInspector] public Collider2D[] towersInRange;
    [HideInInspector] public float teleportDelay = 0.5f;
    [HideInInspector] public Vector2 storedVelocity;

    [Header("Shooting")]
    [HideInInspector] public GameObject playerBulletPrefab;

    [Header("Cannon")]
    [HideInInspector] public bool isInCannon;

    [Header("SplitBody")]
    [HideInInspector] public GameObject slimeClonePrefab;
    [HideInInspector] public float swapControlRadius = 10f;
    [HideInInspector] public float absorbRadius = 5f;
    private InventoryManager inventoryManager;
    //public SlimeClone[] clones;
    //public SlimeClone closestClone;

    [Header("Robot")]
    public bool isInRobot = false;
    [SerializeField] public Robot currentRobot;


    // //////////////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthComponent = GetComponent<HealthComponent>();
        gameTimer = FindObjectOfType<GameTimer>();
    }
    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
    }
    // Update is called once per frame
    void Update()
    {
        if (isNormalStatus())
        {
            HandleTeleport();
            HandleSlime();
        }
    }



    #region Teleport
    private void HandleTeleport()
    {
        //Lấy tất cả trụ trong tầm tele
        towersInRange = Physics2D.OverlapCircleAll(transform.position, teleportRadius, towerLayer);
        //Kiểm tra trụ gần nhất để teleport
        TeleportTower targetTower = GetClosestTower(towersInRange);

        if (Input.GetKeyDown(KeyCode.Space))
            ExitTower();
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHidden)
            {
                //Nếu chưa ẩn, teleport hoặc swap
                TryTeleportToTower(targetTower);
                TrySwapTower(targetTower);
            }
            else
            {
                if (targetTower == null)
                {
                    //Nếu dang ẩn, ko có trụ khác xung quanh = thoát ra
                    ExitTower();
                }
                else
                {
                    if (targetTower.type == TeleportTower.TowerType.TYPE_TELEPORT)
                    {
                        //Nếu đang ẩn, có trụ xung quanh, tele trụ mới
                        currentTower.Activate(false);
                        TryTeleportToTower(targetTower);
                    }
                    else if (targetTower.type == TeleportTower.TowerType.TYPE_SWAP)
                        TrySwapTower(targetTower);
                }
            }
        }
        if (isHidden)//trc day ở Update
        {
            if (currentTower != null)
            {
                transform.position = currentTower.transform.position;
                gameObject.GetComponent<Player>().LockMove(true);
            }
        }
    }
    private TeleportTower GetClosestTower(Collider2D[] towersInRange)
    {
        TeleportTower closestAvailableTower = null;
        float closestDistance = teleportRadius;

        // Lấy toàn bộ tower trong game
        TeleportTower[] allTowers = FindObjectsOfType<TeleportTower>();
        foreach (var tower in allTowers)
        {
            // Đảm bảo tất cả availableMark đều bị tắt trước
            tower.availableMark.GetComponent<SpriteRenderer>().enabled = false;
        }


        foreach (var towerCollider in towersInRange)
        {
            TeleportTower tower = towerCollider.GetComponent<TeleportTower>();
            if (tower != null && (!isHidden || tower != currentTower))
            {
                float distance = Vector2.Distance(transform.position, tower.transform.position);
                if (distance < closestDistance && tower.isAvailable)
                {
                    closestAvailableTower = tower;
                    closestDistance = distance;
                }
            }
        }
        // Nếu tìm được tower available gần nhất, bật availableMark của nó
        if (closestAvailableTower != null)
            closestAvailableTower.availableMark.GetComponent<SpriteRenderer>().enabled = true;
        return closestAvailableTower;
    }
    private void TryTeleportToTower(TeleportTower tower)
    {
        if (currentTower != null)
        {
            // Gọi ExitTower để đảm bảo tower hiện tại cooldown
            ExitTower();
        }

        if (tower != null && tower.isAvailable && tower.type == TeleportTower.TowerType.TYPE_TELEPORT) //Kiểm tra nếu tower teleport khả dụng
        {
            gameTimer.StartTimer();
            //Lưu lại vận tốc trước khi teleport
            storedVelocity = rb.velocity;

            // Set parent vào cây trụ, chuyển collider của player sang isTrigger
            transform.SetParent(tower.transform);
            transform.position = tower.transform.position + Vector3.up;

            playerCollider.isTrigger = true;

            // Kích hoạt trạng thái ẩn, cập nhật trụ hiện tại
            tower.Activate(true);
            currentTower = tower;
            isHidden = true;

            //ẩn sprite renderer
            spriteRenderer.enabled = false;
        }
    }
    private void TrySwapTower(TeleportTower tower)
    {
        if (tower != null && tower.isAvailable && tower.type == TeleportTower.TowerType.TYPE_SWAP) //Kiểm tra nếu tower swap khả dụng
        {
            gameTimer.StartTimer();
            if (!isHidden)
            {
                Vector3 tempPos = transform.position;
                transform.position = tower.transform.position;
                tower.transform.position = tempPos;
            }
            else
            {
                Vector3 tempPos = currentTower.transform.position;
                currentTower.transform.position = tower.transform.position;
                tower.transform.position = tempPos;
            }
        }
    }
    public void ExitTower()
    {
        if (currentTower != null)
        {
            // Bắt đầu cooldown cho tower
            StartCoroutine(currentTower.CoolDownTower());

            // Hủy liên kết với tower hiện tại
            transform.SetParent(null);
            currentTower.Activate(false);
            isHidden = false;
            rb.gravityScale = 5f;

            // Bật lại sprite, tắt isTrigger, cho phép di chuyển
            gameObject.GetComponent<Player>().UnlockMove(true);
            playerCollider.isTrigger = false;
            spriteRenderer.enabled = true;

            // Thả người chơi cao hơn một chút
            rb.transform.position = currentTower.transform.position + new Vector3(0, 1.5f, 0);
            currentTower = null;

            // Khôi phục vận tốc trước khi teleport
            rb.velocity = storedVelocity;
        }
    }
    #endregion

    #region Player skills
    public void UseItem(InventoryManager.InventorySlot selectedSlot)
    {
        gameTimer.StartTimer();
        switch (selectedSlot.itemName)
        {
            case "PlayerBullet":
                PlayerShoot();
                break;
            default:
                Debug.Log("Loi khi dung skill");
                break;
        }
    }
    public bool isNormalStatus()
    {
        if (FindObjectOfType<PauseMenu>().isPaused)
            return false;
        if (FindObjectOfType<LevelCompleteMenu>().isComplete)
            return false;
        if (GetComponent<PlayerDeath>().isDead)
            return false;
        if (isInRobot)
            return false;
        if (isInCannon)
            return false;
        return true;
    }
    #region Shooting
    private void PlayerShoot()
    {
        Instantiate(playerBulletPrefab, transform.position, transform.rotation);
    }
    #endregion

    #endregion

    #region SplitBody
    public void HandleSlime()
    {

        if (Input.GetKeyDown(KeyCode.S))
        {
            SplitBody();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SwapControl();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            AbsorbClone();
        }
    }
    public void SplitBody()
    {
        //Kiểm tra số lượng slime
        int slimeCount = 0;
        InventorySlot slimeSlot = CheckSlimeQuantity(out slimeCount);
        if (slimeSlot == null || slimeCount < 8)
            return; // Không đủ slime để chia đôi

        gameTimer.StartTimer();
        // Trừ số slime đã chia khỏi người chơi
        int splitedSlime = slimeCount / 2;
        slimeSlot.itemCount -= splitedSlime;
        inventoryManager.UpdateUI();

        // Tạo và cấu hình bản sao của người chơi
        GameObject clone = Instantiate(slimeClonePrefab, transform.position - new Vector3(0, 0.5f, 0), Quaternion.identity);
        SlimeClone cloneScript = clone.GetComponent<SlimeClone>();
        if (cloneScript != null)
        {
            cloneScript.SetSlimeCount(splitedSlime);
        }
    }
    private InventorySlot CheckSlimeQuantity(out int slimeCount)
    {
        slimeCount = 0;
        if (inventoryManager == null || inventoryManager.inventorySlots == null)
        {
            Debug.LogError("InventoryManager or inventorySlots is NULL!");
            return null;
        }
        slimeCount = 0;
        foreach (InventorySlot slot in inventoryManager.inventorySlots)
        {
            if (slot.itemName == "Slime")
            {
                slimeCount = slot.itemCount;
                return slot;
            }
        }
        return null;
    }
    public void SwapControl()
    {
        SlimeClone closestClone = FindClosestClone(swapControlRadius);
        if (closestClone == null)
            return;

        //Kiem tra so Slime cua nguoi choi
        int playerSlimeCount;
        InventorySlot slimeSlot = CheckSlimeQuantity(out playerSlimeCount);

        //Kiem tra dieu kien hoan doi
        if (slimeSlot == null || playerSlimeCount < 4)
            return;

        gameTimer.StartTimer();
        //Doi vi tri
        Vector3 tempPosision = transform.position;
        transform.position = closestClone.transform.position;
        closestClone.transform.position = tempPosision;
        //Doi so luong slime
        int tempSlime = playerSlimeCount;
        slimeSlot.itemCount = closestClone.slimeCount;
        closestClone.slimeCount = tempSlime;
        inventoryManager.UpdateUI();
    }
    private void AbsorbClone()
    {
        SlimeClone closestClone = FindClosestClone(absorbRadius);
        if (closestClone == null)
            return;
        int cloneSlime = closestClone.slimeCount;
        transform.position = closestClone.transform.position;

        //Kiem tra inventory
        foreach (InventorySlot slot in inventoryManager.inventorySlots)
        {
            if (slot.itemName == "Slime")
            {
                gameTimer.StartTimer();
                inventoryManager.AddItem(slot.itemSprite, slot.itemName, cloneSlime, slot.isCounted);
                Destroy(closestClone.gameObject);
                return;
            }
        }
        if (inventoryManager.isFull)
        {
            Debug.Log("Da full inven");
            return;
        }
        gameTimer.StartTimer();
        inventoryManager.AddItem(closestClone.slimeSprite, "Slime", cloneSlime, true);
        Destroy(closestClone.gameObject);

    }
    private SlimeClone FindClosestClone(float maxDistance)
    {
        List<SlimeClone> clones = new List<SlimeClone>(FindObjectsOfType<SlimeClone>());
        SlimeClone closestClone = null;
        float closestDistance = maxDistance;

        foreach (SlimeClone clone in clones)
        {
            float distance = Vector2.Distance(transform.position, clone.transform.position);
            if (distance < closestDistance)
            {
                closestClone = clone;
                closestDistance = distance;
            }
        }
        return closestClone;
    }
    #endregion

    #region Cannon
    public void EnterCannon(Vector3 cannonPosition)
    {
        gameTimer.StartTimer();
        if (!isNormalStatus())
            return;
        rb.velocity = Vector2.zero;
        transform.position = cannonPosition;
        gameObject.GetComponent<SpriteRenderer>().enabled = false; 
        GetComponent<Collider2D>().isTrigger = true;
        isHidden = true;
        gameObject.GetComponent<Player>().LockMove(false);
        isInCannon = true;
    }
    public void ExitCannon(Vector2 shootDirection, float shootForce)
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        isHidden = false;
        GetComponent<Collider2D>().isTrigger = false;
        rb.velocity = shootDirection * shootForce; //Bắn theo hướng vào lực truyền vào
        gameObject.GetComponent<Player>().UnlockMove(true);
        StartCoroutine(TemporaryStopUpdatingHorizontalVelocity());
    }
    public bool CanGetInsideCannon()
    {
        List<InventorySlot> inventorySlot = InventoryManager.Instance.inventorySlots;
        foreach (InventorySlot slot in inventorySlot)
        {
            if (slot.itemName == "Slime" && slot.itemCount > 8 && slot.isCounted == true)
                return false;
        }
        if (isInCannon)
            return false;
        return true;
    }
    private IEnumerator TemporaryStopUpdatingHorizontalVelocity()
    {
        gameObject.GetComponent<Player>().SwitchUpdateHorizontalVelocity(true);
        yield return new WaitForSeconds(0.5f);
        isInCannon = false;
        if (gameObject.GetComponent<Player>().isGrounded)
            gameObject.GetComponent<Player>().SwitchUpdateHorizontalVelocity(false);
        else
            StartCoroutine(TemporaryStopUpdatingHorizontalVelocity());
    }

    #endregion

    #region Robot
    public void EnterRobot(Robot robot) => StartCoroutine(BootRobot(robot));
    public void ExitRobot()
    {
        if (currentRobot == null)
            return;
        // Tắt điều khiển robot
        currentRobot.SetControlled(false);
        currentRobot.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        currentRobot.isPlayerInRange = false;

        // Khôi phục người chơi
        isInRobot = false;
        isHidden = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        rb.isKinematic = false;
        playerCollider.enabled = true;                                     //Bat collider
        GetComponent<Player>().UnlockMove(true);
        healthComponent.SetInvincible(false);
        Camera.main.GetComponent<CameraFollow>().SetFollowTarget(gameObject);

        // Đưa người chơi ra khỏi vị trí của robot (xa hơn để tránh va chạm)
        Vector3 exitPosition = currentRobot.transform.position + Vector3.up * 2f;
        gameObject.transform.position = exitPosition;

        // Xóa tham chiếu đến robot
        //currentRobot.playerAbilities = null; //Xoa khi robot cham dat
        currentRobot = null;

        //UI máu
    }
    public bool CanEnterRobot(Robot robot)
    {
        if (robot == null)
            return false;
        if (robot.isControlled)
            return false;
        if (robot.isDestroyed)
            return false;
        if (!robot.isPlayerInRange)
            return false;
        return true;
    }
    private IEnumerator BootRobot(Robot robot)
    {
        if (isNormalStatus())
        {
            gameTimer.StartTimer();

            //setting người chơi
            GetComponent<Player>().LockMove(false);
            currentRobot = robot;
            isInRobot = true;
            isHidden = true;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            playerCollider.enabled = false;                                     //Tat collider

            rb.isKinematic = true;
            healthComponent.SetInvincible(true);
            yield return new WaitForSeconds(1f);


            //Setting của robot
            currentRobot.SetControlled(true);
            currentRobot.ToggleRobotPhysics(false);
            Camera.main.GetComponent<CameraFollow>().SetFollowTarget(currentRobot.gameObject);
        }
    }
    #endregion
}
