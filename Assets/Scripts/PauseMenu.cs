using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    private Player player;
    private GameTimer gameTimer; // Tham chiếu đến GameTimer

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        gameTimer = FindAnyObjectByType<GameTimer>();
    }


    // Update is called once per frame
    void Update()
    {
        //Nhan ESC de tam dung
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeStage();
            else
                PauseStage();
        }
    }

    public void PauseStage()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0; //Tam dung game
        isPaused = true;

        //Timer tam dung
        gameTimer.PauseTimer();

    }

    public void ResumeStage()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; //Tiep tuc game
        isPaused = false;

        //Timer tiep tuc
        gameTimer.ResumeTimer();

    }

    public void LoadMenu()
    {
        Time.timeScale = 1; //Khoi phuc toc do game
        SceneManager.LoadScene("MainMenu"); //Tai lai menu
    }

    public void RestartStage()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        //Reset objects method here

        gameTimer.StartTimer();

        player.ResetPosision();
        
    }
}
