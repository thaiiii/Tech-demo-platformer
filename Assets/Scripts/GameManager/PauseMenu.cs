using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PauseMenu : MonoBehaviour
{
    [Header("In Game")]
    private GameObject pauseMenuUI;
    private bool isPaused = false;
    private Player player;
    private GameTimer gameTimer; // Tham chiếu đến GameTimer
    private NPCDialogue npcDialogue;  // Tham chiếu đến hội thoại NPC

    public List<ItemBase> items;
    public List<SlimeClone> allSlimeClones;
    public List<Robot> allRobots;  

    #region Stage
    private void Awake()
    {
        gameTimer = FindAnyObjectByType<GameTimer>();
        npcDialogue = FindAnyObjectByType<NPCDialogue>();

        SceneManager.sceneLoaded += OnSceneLoaded; // Đăng ký sự kiện
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeUIReferences(); // Cập nhật tham chiếu khi scene mới được tải
        InitializeGameObjectReferences(); //Cập nhật tham chiếu các game object khác
        //RestartStage();
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Hủy đăng ký sự kiện
    }
    private void InitializeUIReferences()
    {
        GameObject UI = GameObject.Find("UI");
        if (UI == null)
        {
            return;
        }
        pauseMenuUI = UI.transform.Find("PauseMenu").gameObject;
        GameObject buttons = pauseMenuUI.transform.Find("Buttons").gameObject;
        Button resumeButton = buttons.transform.Find("ResumeButton").GetComponent<Button>();
        Button restartButton = buttons.transform.Find("RestartButton").GetComponent<Button>();
        Button menuButton = buttons.transform.Find("MenuButton").GetComponent<Button>();

        //Gắn method cho onClick() các nút
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeStage);
        }
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartStage);
        }
        if (menuButton != null)
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(LoadMenu);
        }
    }
    private void InitializeGameObjectReferences()
    {
        player = FindAnyObjectByType<Player>();

        //Lấy vị trí các item trong map
        List<ConsumableItem> consumableItems = new List<ConsumableItem>(FindObjectsOfType<ConsumableItem>());
        List<StorableItem> storableItems = new List<StorableItem>(FindObjectsOfType<StorableItem>());
        foreach (ConsumableItem item in consumableItems)
        {
            items.Add(item);
        }
        foreach (StorableItem item in storableItems)
        {
            items.Add(item);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Nhan ESC de tam dung
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (npcDialogue != null && npcDialogue.isInConversation)
            {
                // Nếu đang trong hội thoại, kết thúc nó
                npcDialogue.EndConversation();
                return;
            }

            if (isPaused)
                ResumeStage();
            else
                PauseStage();
        }
    }
    #endregion

    #region In Game
    public void PauseStage()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0; //Tam dung game
        isPaused = true;

        //Timer tam dung
        gameTimer.PauseTimer();

    }
    public void ResumeStage()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; //Tiep tuc game
        isPaused = false;

        //Timer tiep tuc
        gameTimer.ResumeTimer();

    }
    public void LoadMenu()
    {
        Time.timeScale = 1; //Khoi phuc toc do game
        SceneManager.LoadScene("MainMenu"); //Tai lai menu
    }
    public void RestartStage()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        //Reset objects method here
        gameTimer.StartTimer();

        //ResetPlayer
        player.ResetPosition();
        player.GetComponent<HealthComponent>().Heal(player.GetComponent<HealthComponent>().GetHealthSystem().maxHealth);

        //Reset objects position and status
        ResetTeleportTower();   
        ResetCannon(); 
        ResetFan(); 
        ResetItem();
        ResetSlimeBody();
        ResetRobot();
        ResetBlock();
        ResetLaser();
        ResetMovingTrap();
        ResetEnemy();
        ResetConveyorBelt();
    }
    private void ResetTeleportTower()
    {
        // Reset vị trí tất cả các TeleportTower
        TeleportTower[] teleportTowers = FindObjectsOfType<TeleportTower>();
        foreach (TeleportTower tower in teleportTowers)
        {
            tower.ResetTower();
        }
    }
    private void ResetCannon()
    {
        // bật/tắt tất cả Cannon
        Cannon[] cannons = FindObjectsOfType<Cannon>();
        foreach (Cannon cannon in cannons)
        {
            cannon.ClearAllMissiles();
            cannon.ResetCannon();
        }
    }
    private void ResetFan()
    {
        Fan[] fans = FindObjectsOfType<Fan>();
        foreach (Fan fan in fans)
        {
            fan.ResetFan();
        }
    }
    private void ResetItem()
    {
        foreach (ItemBase item in items)
        {
            if (!item.isCheckpointPicked)
                item.ActiveItem();
        }
        FindObjectOfType<InventoryManager>().LoadSavedInventory();
    }
    private void ResetSlimeBody()
    {
        allSlimeClones = new List<SlimeClone>(FindObjectsOfType<SlimeClone>());
        foreach (SlimeClone clone in allSlimeClones)
        {
            if (clone.isSaved)
                clone.transform.position = clone.checkpointPosition;
            else
            {
                clone.KillClone();
            }
        }
    }
    private void ResetRobot()
    {
        allRobots = new List<Robot>(FindObjectsOfType<Robot>());
        foreach (Robot robot in allRobots)
        {
            robot.LoadSavedRobotStatus();
        }
    }
    private void ResetBlock()
    {
        List<UnblockSwitch> allBlocks = new List<UnblockSwitch>(FindObjectsOfType<UnblockSwitch>());
        foreach (UnblockSwitch block in allBlocks)
        {
            block.LoadSavedBlockStatus();
        }
    }
    private void ResetLaser()
    {
        List<LaserEmitter> allLaserEmitters = new List<LaserEmitter>(FindObjectsOfType<LaserEmitter>());
        foreach (LaserEmitter emitter in allLaserEmitters)
        {
            emitter.LoadSavedEmitterStatus();
        }
    }
    private void ResetMovingTrap()
    {
        List<MovingTrap> allTraps = new List<MovingTrap>(FindObjectsOfType<MovingTrap>());
        foreach (MovingTrap trap in allTraps)
        {
            trap.LoadSavedMovingTrapStatus();
        }
    }
    private void ResetEnemy()
    {
        List<Enemy> allEnemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        foreach (Enemy enemy in allEnemies)
        {
            enemy.LoadSavedEnemyStatus();
        }
    }
    private void ResetConveyorBelt()
    {
        List<ConveyorBelt> allConveyorBelts = new List<ConveyorBelt>(FindObjectsOfType<ConveyorBelt>());
        foreach (ConveyorBelt belt in allConveyorBelts)
        {
            belt.LoadSavedConveyorBeltStatus();
        }
    }
    
    #endregion



}
