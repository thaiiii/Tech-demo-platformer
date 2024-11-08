using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [Header("Death UI")]
    public GameObject deathUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu người chơi chạm vào layer của gai
        if (collision.gameObject.CompareTag("Trap"))
        {
            Debug.Log("chet");
            Die();
        }
    }

    void Die()
    {
        // Xử lý cái chết của người chơi, ví dụ: hiển thị hiệu ứng, tải lại màn chơi
        

        // Load lại màn chơi hoặc thực hiện các hành động khi người chơi chết
        StartCoroutine(DeathMenu());
            
    }


    IEnumerator DeathMenu()
    {
        FindAnyObjectByType<Player>().Death();
        yield return new WaitForSeconds(1.5f);
        deathUI.SetActive(true);

    }

    public void DeathRestart()
    {
        FindAnyObjectByType<PauseMenu>().RestartStage();
        deathUI.SetActive(false);
    }

}
