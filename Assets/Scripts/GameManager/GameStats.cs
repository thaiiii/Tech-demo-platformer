using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance;
    public static GameTimer gameTimer;

    public int totalDeaths = 0; // Tổng số lần chết
    public float totalTime = 0; // Tổng thời gian chơi
    public int levelDeaths = 0; // Số lần chết của màn chơi hiện tại
    public float levelTime = 0; // Thời gian của màn chơi hiện tại

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

        gameTimer = GetComponent<GameTimer>();
    }

    public void CalculateStartLevel()
    {
        levelDeaths = 0;    //Đặt lại số lần chết
    }

    public void CalculateEndLevel()
    {
        levelTime = gameTimer.elapsedTime;  // Lấy thời gian đã chơi màn hiện tại

        totalTime += levelTime;              // Cộng thời gian màn hiện tại vào tổng thời gian
        totalDeaths += levelDeaths;          // Cộng số lần chết màn hiện tại vào tổng số lần chết
    }

    public void AddDeath()
    {
        levelDeaths++; // Tăng số lần chết màn hiện tại
    }
}
