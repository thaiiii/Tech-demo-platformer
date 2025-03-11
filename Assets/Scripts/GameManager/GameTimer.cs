using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;


public class GameTimer : MonoBehaviour
{
    [Header("UI")]
    private TextMeshProUGUI timerText; // UI Text hiển thị thời gian chơi
    private TextMeshProUGUI pausedTimerText; // UI Text hiển thị thời gian lúc tạm dừng
    public float elapsedTime = 0f; //Tổng thời gian để trôi qua
    [SerializeField] private bool isTiming = false;
    [SerializeField] public bool hasStarted = false; //Kiểm tra xem người chơi đã di chuyển chưa

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Đăng ký sự kiện
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasStarted = false;
        isTiming = false;
        InitializeUIReferences();
    }

    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeUIReferences()
    {
        GameObject UI = GameObject.Find("General UI");
        if (UI == null)
        {
            return;
        }
        GameObject pauseMenuUI = UI.transform.Find("PauseMenu").gameObject;
        pausedTimerText = pauseMenuUI.transform.Find("PauseTimer").GetComponent<TextMeshProUGUI>();

        GameObject stageUI = UI.transform.Find("StageUI").gameObject;
        timerText = stageUI.transform.Find("StageTimer").GetComponent<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {
        //Nếu bộ đếm hoạt động, chạy thơi gian
        if (hasStarted && isTiming)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimeDisplay();
        }
    }
    //Hàm bắt đầu khi người chơi di chuyển lần đầu
    public void StartTimer()
    {
        {
            hasStarted = true;
            isTiming = true;
            UpdateTimeDisplay();
        }
    }

    //Reset đồng hồ
    public void ResetTimer()
    {
        hasStarted = false;
        isTiming = false;
        elapsedTime = 0f;
        UpdateTimeDisplay();
    }
    //public void ContinueTimer()
    //{

    //}
    //Hàm tạm dừng bộ đến
    public void PauseTimer()
    {
        isTiming = false;
        pausedTimerText.text = timerText.text;
    }

    //Hàm tiếp tục chạy bộ đếm
    public void ResumeTimer()
    {
        isTiming = true;
    }

    //Hàm dừng bộ đếm
    public void StopTimer()
    {
        isTiming = false;

    }

    //Hàm cập nhật hiển thị thời gian
    private void UpdateTimeDisplay()
    {
        //int seconds = Mathf.FloorToInt(elapsedTime);
        //int miliseconds = Mathf.FloorToInt((elapsedTime * 100) % 100);

        //timerText.text = string.Format("TIME    {0:00}.{1:00}", seconds, miliseconds);
        timerText.text = $"TIME: {elapsedTime:F2}";
    }
}
