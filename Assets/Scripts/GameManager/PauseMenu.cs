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
        List<ConsumableItem> consumableItems = new List<ConsumableItem>( FindObjectsOfType<ConsumableItem>());
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

        player.ResetPosition();

        //Reset objects position
        ResetTeleportTower();   //Teleport towers
        SwitchCannonActivation(); //Clear all missiles
        ResetFan(); //Reset all fans
        ResetItem();
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

    private void SwitchCannonActivation()
    {
        // bật/tắt tất cả Cannon
        Cannon[] cannons = FindObjectsOfType<Cannon>();
        foreach (Cannon cannon in cannons)
        {
            cannon.ClearAllMissiles();
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
            item.ActiveItem();
        }
        FindObjectOfType<InventoryManager>().LoadSavedInventory();
    }




    #endregion



}
