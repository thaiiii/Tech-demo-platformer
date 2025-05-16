using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using static Enemy;

public class PlayerDeath : MonoBehaviour
{
    private GameObject gameManager;
    private CinemachineVirtualCamera virtualCamera;

    [Header("Death UI")]
    private GameObject UI;
    private GameObject deathUI;
    private Button restartButton;
    private PlayerAbilities playerAbilities;

    public bool isDead = false;

    private void Awake()
    {
        gameManager = FindObjectOfType<PauseMenu>().gameObject;
        playerAbilities = gameObject.GetComponent<PlayerAbilities>();
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>(); // Tìm Cinemachine Camera

        // Tìm trong Scene các thành phần UI
        UI = GameObject.Find("General UI");
        deathUI = UI.transform.Find("DeathUI")?.gameObject;
        restartButton = deathUI.transform.Find("RestartButton")?.GetComponent<Button>();
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners(); // Xóa sự kiện cũ (nếu có)
            restartButton.onClick.AddListener(DeathRestart); // Gắn hàm DeathRestart
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Kiểm tra nếu người chơi chạm vào tag Trap
        if (collision.gameObject.CompareTag("Trap") && !HiddenStatus())
        {
            GetComponent<HealthComponent>().TakeDamage(GetComponent<HealthComponent>().maxHealth);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.GetComponent<Enemy>().type == EnemyType.Pointy)
                KillPlayer();
            if (collision.GetComponent<Enemy>().type == EnemyType.Normal && !HiddenStatus())
                KillPlayer();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null && collision.gameObject.CompareTag("Enemy")) // Kiểm tra Enemy không null trước
        {
            if (enemy.type == EnemyType.Normal && !HiddenStatus())
                KillPlayer();
            if (enemy.type == EnemyType.Pointy)
                KillPlayer();
        }
    }

    public void KillPlayer()
    {
        // Load lại màn chơi hoặc thực hiện các hành động khi người chơi chết
        if (!isDead)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            // Xử lý cái chết của người chơi, ví dụ: hiển thị hiệu ứng, tải lại màn chơi
            virtualCamera.Follow = null;
            //Nếu isHidden = true, gọi ExitTower, bỏ hết Parent
            if (playerAbilities.isHidden)
                playerAbilities.ExitTower();
            if (playerAbilities.isInCannon)
                playerAbilities.ExitCannon(Vector2.zero, 0f);

            isDead = true;
            GameStats.Instance.AddDeath();

            //tam dung time
            gameManager.GetComponent<GameTimer>().PauseTimer();
            AudioManager.Instance.PlaySFX("death");
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

        FindAnyObjectByType<PauseMenu>().RestartStage(false);
        virtualCamera.Follow = transform;

    }

    public bool HiddenStatus()
    {
        return GetComponent<PlayerAbilities>().isHidden;
    }

}
