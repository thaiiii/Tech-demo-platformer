using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


public class GameTimer : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timerText; // UI Text hiển thị thời gian chơi
    public TextMeshProUGUI pausedTimerText; // UI Text hiển thị thời gian lúc tạm dừng
    public float elapsedTime = 0f; //Tổng thời gian để trôi qua
    public bool isTiming = false;
    public bool hasStarted = false; //Kiểm tra xem người chơi đã di chuyển chưa



    // Update is called once per frame
    void Update()
    {
        //Nếu bộ đếm hoạt động, chạy thơi gian
        if(hasStarted && isTiming)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimeDisplay();
        }
    }
    //Hàm bắt đầu khi gười chơi di chuyển lần đầu
    public void StartTimer()
    {
        if (hasStarted == false)
        {
            hasStarted = true;
            isTiming = true;
        }
        else
        {
            hasStarted = false;
            elapsedTime = 0f;
            UpdateTimeDisplay();
        }
        
    }
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
