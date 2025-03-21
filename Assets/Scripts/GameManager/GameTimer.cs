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
    public float savedTime = 0f;
    [SerializeField] private bool isTiming = false;
    [SerializeField] public bool hasStarted = false; //Kiểm tra xem người chơi đã di chuyển chưa

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Đăng ký sự kiện
    }
    void Update()
    {
        //Nếu bộ đếm hoạt động, chạy thơi gian
        if (hasStarted && isTiming)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimeDisplay();
        }
    }
    
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasStarted = false;
        isTiming = false;
        elapsedTime = 0f;
        savedTime = 0f;
        InitializeUIReferences();
    }
    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void InitializeUIReferences()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
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
        else
        {
            timerText = null;
            pausedTimerText = null;
        }
    }


    public void StartTimer()
    {
        hasStarted = true;
        isTiming = true;
        UpdateTimeDisplay();
    }
    public void ResetTimer()
    {
        hasStarted = false;
        isTiming = false;
        elapsedTime = savedTime;
        UpdateTimeDisplay();
    }
    public void PauseTimer()
    {
        isTiming = false;
        pausedTimerText.text = timerText.text;
    }
    public void ResumeTimer() => isTiming = true;
    public void StopTimer() => isTiming = false;
    private void UpdateTimeDisplay() => timerText.text = $"TIME: {elapsedTime:F2}";
}
