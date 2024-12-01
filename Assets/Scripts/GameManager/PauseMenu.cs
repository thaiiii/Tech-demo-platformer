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
    private NPCDialogue npcDialogue;  // Tham chiếu đến hội thoại NPC

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        gameTimer = FindAnyObjectByType<GameTimer>();
        npcDialogue = FindAnyObjectByType<NPCDialogue>(); 
    }


    // Update is called once per frame
    void Update()
    {
        //Nhan ESC de tam dung
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (npcDialogue != null && npcDialogue.isInConversation)
            {
                // Nếu đang trong hội thoại, kết thúc nó
                npcDialogue.EndConversation();
                return;
            }

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

        player.ResetPosition();

        //Reset objects position
        ResetTeleportTower();   //Teleport towers
        SwitchCannonActivation(); //Clear all missiles
    }

    private void ResetTeleportTower()
    {
        // Reset vị trí tất cả các TeleportTower
        TeleportTower[] teleportTowers = FindObjectsOfType<TeleportTower>();
        foreach (TeleportTower tower in teleportTowers)
        {
            tower.ResetTower();
        }
    }

    private void SwitchCannonActivation()
    {
        // bật/tắt tất cả Cannon
        Cannon[] cannons = FindObjectsOfType<Cannon>();
        foreach (Cannon cannon in cannons)
        {
            cannon.ClearAllMissiles();
        }
    }

}
