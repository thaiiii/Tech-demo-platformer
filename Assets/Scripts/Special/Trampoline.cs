using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public bool isActivated = false;
    public float bounceForce = 20f; //Lưc bat cho nguoi choi
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActivated && collision.gameObject.CompareTag("Player"))
        {
            // Kích hoạt animation nhún xuống
            animator.SetTrigger("Bounce");

            // Bật người chơi lên cao
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, bounceForce); // Chỉ thay đổi velocity Y
            }

            // Đặt block vào trạng thái "đã sử dụng" trong 1 giây
            isActivated = true;
            StartCoroutine(ResetTrampoline());
        }
    }



    private IEnumerator ResetTrampoline()
    {
        yield return new WaitForSeconds(0.5f);
        isActivated = false;
        animator.ResetTrigger("Bounce"); //Reset trang thai cho lan tiep theo
    }
}
