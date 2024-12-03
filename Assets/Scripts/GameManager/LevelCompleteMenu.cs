using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteMenu : MonoBehaviour
{
    [Header("Level Complete")]
    public GameObject levelSummaryUI; // UI tổng kết màn
    public TextMeshProUGUI deathsText; // Text hiển thị số lần chết
    public TextMeshProUGUI timeText;   // Text hiển thị thời gian chơi
    public GameObject nextLevelButton; // Nút qua màn
    public GameObject restartButton;   // Nút restart
    public GameObject mainMenuButton;  // Nút về menu chính

    private GameTimer gameTimer; // Tham chiếu đến GameTimer
    private PauseMenu pauseMenu; // Tham chiếu PauseMenu

    private void Awake()
    {
        gameTimer = GetComponent<GameTimer>();
        pauseMenu = GetComponent<PauseMenu>();
    }

    #region Level Complete
    public void CompleteLevel()
    {
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
            GameStats.Instance.CalculateStartLevel(); //Reset số liệu màn chơi
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels!");
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
