using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    private static LevelManager Instance;
    private PauseMenu pauseMenu;
    private GameTimer gameTimer;
    private GameStats gameStats;
    private LevelCompleteMenu levelCompleteMenu;
    private InventoryManager inventoryManager;

    [Header("Level Buttons Canvas")]
    private Transform levelsContainer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Đảm bảo không bị phá hủy khi chuyển cảnh
        }
        else
        {
            Destroy(gameObject);
        }

        pauseMenu = GetComponent<PauseMenu>();
        gameTimer = GetComponent<GameTimer>();
        gameStats = GetComponent<GameStats>();
        inventoryManager = GetComponent<InventoryManager>();
        levelCompleteMenu = GetComponent<LevelCompleteMenu>();

        SceneManager.sceneLoaded += OnSceneLoaded; // Đăng ký sự kiện
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeComponentReferences();
        InitiallizeGameObjectReferences();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Hủy đăng ký sự kiện
    }

    private void InitializeComponentReferences()
    {
        //Kiểm tra scene hiện tại
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "MainMenu")
        {
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

    private void InitiallizeGameObjectReferences()
    {
        //Kiểm tra scene hiện tại
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "MainMenu")
            return;

        levelsContainer = GameObject.Find("Levels")?.transform;
        if (levelsContainer == null)
        {
            return;
        }
        AssignLevelButtons();

    }

    void Start()
    {

    }

    private void AssignLevelButtons()
    {
        if (levelsContainer == null)
            return;

        foreach (Transform child in levelsContainer)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                string levelName = child.name; // Use the button's name as the level name
                button.onClick.RemoveAllListeners(); // Clear any existing listeners
                button.onClick.AddListener(() => LoadLevel(levelName));
            }
        }
    }

    private void LoadLevel(string levelName)
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
}
