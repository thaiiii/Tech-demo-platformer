﻿
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelCompleteMenu : MonoBehaviour
{
    [Header("Level Complete")]
    public bool isComplete = false;
    private GameObject levelSummaryUI; // UI tổng kết màn
    private TextMeshProUGUI deathsText; // Text hiển thị số lần chết
    private TextMeshProUGUI timeText;   // Text hiển thị thời gian chơi
    private Button nextLevelButton; // Nút qua màn
    private Button restartLevelButton;   // Nút restart
    private Button mainMenuButton;  // Nút về menu chính


    private GameTimer gameTimer; // Tham chiếu đến GameTimer
    private PauseMenu pauseMenu; // Tham chiếu PauseMenu

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Đăng ký sự kiện
    }
    private void InitializeUIReferences()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            //Tìm các UI tong scene
            GameObject UI = GameObject.Find("General UI");
            if (UI == null)
            {
                return;
            }
            levelSummaryUI = UI.transform.Find("LevelCompleteUI").gameObject;
            deathsText = levelSummaryUI.transform.Find("LevelDeath").GetComponent<TextMeshProUGUI>();
            timeText = levelSummaryUI.transform.Find("LevelTime").GetComponent<TextMeshProUGUI>();
            nextLevelButton = levelSummaryUI.transform.Find("NextButton").GetComponent<Button>();
            restartLevelButton = levelSummaryUI.transform.Find("RestartButton").GetComponent<Button>();
            mainMenuButton = levelSummaryUI.transform.Find("MenuButton").GetComponent<Button>();

            //Gắn sự kiện cho các nút
            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.RemoveAllListeners();
                nextLevelButton.onClick.AddListener(LoadNextLevel);
            }
            if (restartLevelButton != null)
            {
                restartLevelButton.onClick.RemoveAllListeners();
                restartLevelButton.onClick.AddListener(RestartLevel);
            }
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.RemoveAllListeners();
                mainMenuButton.onClick.AddListener(LoadMenu);
            }
        }
        else
        {
            levelSummaryUI = null;
            deathsText = null;
            timeText = null;
            nextLevelButton = null;
            restartLevelButton = null;
            mainMenuButton = null;
        }
    }
    private void InitiallizeGameObjectReferences()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            gameTimer = GetComponent<GameTimer>();
            pauseMenu = GetComponent<PauseMenu>();
        }
        else
        {
            gameTimer = null;
            pauseMenu = null;
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isComplete = false;
        // Cập nhật tham chiếu khi scene mới được tải
        InitiallizeGameObjectReferences();
        InitializeUIReferences();
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Hủy đăng ký sự kiện
    }


    #region Level Complete
    public void CompleteLevel()
    {
        isComplete = true;

        // Gọi kết thúc thời gian chơi
        gameTimer.StopTimer();
        GameStats.Instance.CalculateEndLevel();

        // Hiển thị tổng kết màn chơi
        levelSummaryUI.SetActive(true);
        Time.timeScale = 0; // Tạm dừng game

        // Hiển thị số lần chết và thời gian
        deathsText.text = $"Deaths: {GameStats.Instance.levelDeaths}";
        timeText.text = $"Time: {GameStats.Instance.levelTime:F2}";
    }
    public void LoadNextLevel()
    {
        Time.timeScale = 1; // Khôi phục tốc độ game

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            GameStats.Instance.ResetLevelDeath(); //Reset số liệu màn chơi
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            LoadMenu();
        }
    }
    public void LoadMenu()
    {
        Time.timeScale = 1; //Khoi phuc toc do game
        SceneManager.LoadScene("MainMenu"); //Tai lai menu
    }
    public void RestartLevel()
    {
        pauseMenu.RestartStage();
        levelSummaryUI.SetActive(false);
    }
    #endregion
}
