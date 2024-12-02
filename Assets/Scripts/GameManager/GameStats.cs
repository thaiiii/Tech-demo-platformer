using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance;

    public int totalDeaths = 0; // Tổng số lần chết
    public float levelStartTime; // Thời gian bắt đầu màn chơi
    public float totalTime = 0; // Tổng thời gian chơi

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
    }

    public void StartLevel()
    {
        levelStartTime = Time.time; // Lưu thời gian bắt đầu
    }

    public void EndLevel()
    {
        totalTime += Time.time - levelStartTime; // Cộng thêm thời gian đã chơi
    }
}
