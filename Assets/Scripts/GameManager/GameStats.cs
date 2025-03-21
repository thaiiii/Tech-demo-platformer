using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance;
    private GameTimer gameTimer;

    [Header("Level Stats")]
    public int levelDeaths = 0; // Số lần chết của màn chơi hiện tại
    public float levelTime = 0; // Thời gian của màn chơi hiện tại

    [Header("Total Stats")]
    public int totalDeaths = 0; // Tổng số lần chết
    public float totalTime = 0; // Tổng thời gian chơi


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Đảm bảo GameStats tồn tại giữa các scene
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // Tránh tạo nhiều instance
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        levelDeaths = 0;
        levelTime = 0f;
        InitiallizeGameObjectReferences();
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void InitiallizeGameObjectReferences()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
            gameTimer = GetComponent<GameTimer>();
        else
            gameTimer = null;
    }

    public void ResetLevelDeath() => levelDeaths = 0;    //Đặt lại số lần chết
    public void CalculateEndLevel()
    {
        levelTime = gameTimer.elapsedTime;  // Lấy thời gian đã chơi màn hiện tại
        totalTime += levelTime;              // Cộng thời gian màn hiện tại vào tổng thời gian
        totalDeaths += levelDeaths;          // Cộng số lần chết màn hiện tại vào tổng số lần chết
    }

    public void AddDeath() => levelDeaths++; // Tăng số lần chết màn hiện tại
}
