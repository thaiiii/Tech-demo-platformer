using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using static Enemy;

public class PlayerDeath : MonoBehaviour
{
    [Header("Death UI")]
    public GameObject deathUI;
    public Camera m_camera;
    public GameObject gameManager;
    public bool isDead = false;

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu người chơi chạm vào tag Trap
        if (collision.gameObject.CompareTag("Trap") && !HiddenStatus())
        {
            KillPlayer();
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.GetComponent<Enemy>().type == EnemyType.Pointy) 
                KillPlayer();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null) // Kiểm tra Enemy không null trước
        {
            if (enemy.type == EnemyType.Normal && !HiddenStatus())
                KillPlayer();
            if (enemy.type == EnemyType.Pointy)
                KillPlayer();
        }
    }

    public void KillPlayer()
    {
        // Xử lý cái chết của người chơi, ví dụ: hiển thị hiệu ứng, tải lại màn chơi
        m_camera.GetComponent<CameraFollow>().isFollowing = false;
        // Load lại màn chơi hoặc thực hiện các hành động khi người chơi chết
        if (!isDead)
        {
            //Nếu isHidden = true, gọi ExitTower, bỏ hết Parent
            if(GetComponent<PlayerAbilities>().isHidden)
                GetComponent<PlayerAbilities>().ExitTower();
            
            isDead = true;  
            //tam dung time
            gameManager.GetComponent<GameTimer>().PauseTimer();
            StartCoroutine(DeathMenu());
        }
    }

    IEnumerator DeathMenu()
    {
        FindAnyObjectByType<Player>().Death();
        yield return new WaitForSeconds(1.5f);
        deathUI.SetActive(true);

    }

    public void DeathRestart()
    {
        deathUI.SetActive(false);
        isDead = false;

        FindAnyObjectByType<PauseMenu>().RestartStage();
        m_camera.GetComponent<CameraFollow>().isFollowing = true ;
        
    }

    public bool HiddenStatus()
    {
        return GetComponent<PlayerAbilities>().isHidden;
    }

}
