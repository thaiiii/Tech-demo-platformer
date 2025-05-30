﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    private PauseMenu pauseMenu;
    private GameTimer gameTimer;
    private GameStats gameStats;
    private LevelCompleteMenu levelCompleteMenu;
    private InventoryManager inventoryManager;
    private Transform levelsContainer;
    private const string KEY = "HighestLevel";
    public Color unlockedColor = Color.white;
    public Color lockedColor = new Color(0.5f, 0.5f, 0.5f); // Màu tối hơn
    public int levelUnlocked;
    Transform ui;
    Button settingsButton;
    Button backButton;
    Button playButton;
    Button quitButton;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Đảm bảo không bị phá hủy khi chuyển cảnh
            SceneManager.sceneLoaded -= OnSceneLoaded; // Tránh đăng ký trùng
            SceneManager.sceneLoaded += OnSceneLoaded; // Đăng ký sự kiện
        }
        else
        {
            Destroy(gameObject);
        }


    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeComponentReferences();
        InitiallizeUIReferences();
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Hủy đăng ký sự kiện
    }
    private void InitializeComponentReferences()
    {
        pauseMenu = GetComponent<PauseMenu>();
        gameTimer = GetComponent<GameTimer>();
        gameStats = GetComponent<GameStats>();
        inventoryManager = GetComponent<InventoryManager>();
        levelCompleteMenu = GetComponent<LevelCompleteMenu>();

        //Kiểm tra scene hiện tại
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "MainMenu")
        {
            //Sử dụng unlock level
            //
            //
            //

            // Vô hiệu hóa các thành phần không cần thiết
            LevelManager.Instance.enabled = true;
            pauseMenu.enabled = false;
            gameTimer.enabled = false;
            levelCompleteMenu.enabled = false;
            inventoryManager.enabled = false;
            gameStats.enabled = false;

        }
        else
        {
            LevelManager.Instance.enabled = false;
            pauseMenu.enabled = true;
            gameTimer.enabled = true;
            levelCompleteMenu.enabled = true;
            inventoryManager.enabled = true;
            gameStats.enabled = true;
        }
    }
    private void InitiallizeUIReferences()
    {
        //Kiểm tra scene hiện tại
        string currentScene = SceneManager.GetActiveScene().name;

        if(currentScene == "StartMenu")
        {
            ui = GameObject.Find("UI").transform;
            playButton = ui.Find("Start").GetComponent<Button>();
            quitButton = ui.Find("Quit").GetComponent<Button>();
            AssignStartMenuButtons();
        }

        if (currentScene == "MainMenu")
        {
            ui = GameObject.Find("Levels").transform;
            settingsButton = ui.Find("SettingsButton").GetComponent<Button>();
            backButton = ui.Find("BackButton").GetComponent<Button>();
            levelsContainer = GameObject.Find("LevelsPanel")?.transform;

            AssignLevelButtons();
            AssignOtherButtons();
        }
    }

    private void AssignStartMenuButtons()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(() => LoadLevel("MainMenu"));
        }
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(() => Application.Quit());
        }
    }

    private void AssignLevelButtons()
    {
        if (levelsContainer == null)
            return;

        levelUnlocked = PlayerPrefs.GetInt("HighestLevel", 1);
        foreach (Transform child in levelsContainer)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                //Unlock Level
                UnlockAvailableLevel(button);

                string levelName = child.name; // Use the button's name as the level name
                button.onClick.RemoveAllListeners(); // Clear any existing listeners
                button.onClick.AddListener(() => LoadLevel(levelName));
            }
        }
    }
    private void AssignOtherButtons()
    {
        if (settingsButton == null)
        {
            return;
        }
        if (settingsButton.onClick.GetPersistentEventCount() == 0)
        {
            settingsButton.onClick.AddListener(LoadSettings);
        }

        if (backButton == null)
        {
            return;
        }
        if (backButton.onClick.GetPersistentEventCount() == 0)
        {
            backButton.onClick.AddListener(LoadStart);
        }
    }
    public void LoadLevel(string levelName)
    {
        if (Application.CanStreamedLevelBeLoaded(levelName))
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogError($"Level {levelName} does not exist or is not added to the build settings!");
        }
    }
    public void LoadSettings()
    {
        SceneManager.LoadScene("SettingsMenu", LoadSceneMode.Additive);
    }
    public void LoadStart()
    {
        SceneManager.LoadScene("StartMenu");
    }

    //Xử lý Level unlock
    public void SaveLevelUnlocked(int levelReached)
    {
        int current = PlayerPrefs.GetInt(KEY, 1); // mặc định là level 1
        if (levelReached > current)
        {
            PlayerPrefs.SetInt(KEY, levelReached);
            PlayerPrefs.Save();
        }
    }
    public int GetHighestUnlockedLevel()
    {
        return PlayerPrefs.GetInt(KEY, 1);
    }

    public void UnlockAvailableLevel(Button button)
    {
        button.GetComponent<Image>().color = lockedColor;
        button.interactable = false;
        int levelNumber = ExtractLevelNumber(button.gameObject.name);
        if (levelNumber <= levelUnlocked)
        {
            button.GetComponent<Image>().color = unlockedColor;
            button.interactable = true;
        }
    }
    int ExtractLevelNumber(string buttonName)
    {
        // Giả sử tên luôn bắt đầu bằng "Level"
        string levelNumber = buttonName.Replace("Level", "");
        int.TryParse(levelNumber, out int level);
        return level;
    }
}
