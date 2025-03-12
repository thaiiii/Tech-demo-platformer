using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public bool isActivated = false;
    public float bounceForce = 20f; //Lưc bat cho nguoi choi
    public float bounceDelay = 1f;
    private Animator animator;
    public List<string> applicableTags;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (!isActivated && applicableTags.Contains(collision.gameObject.tag)) 
    //    {
    //        // Kích hoạt animation nhún xuống
    //        animator.SetTrigger("Bounce");

    //        //// Bật người chơi lên cao
    //        //Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
    //        //if (playerRb != null)
    //        //{
    //        //    playerRb.velocity = new Vector2(playerRb.velocity.x, bounceForce); // Chỉ thay đổi velocity Y
    //        //}
    //        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
    //        rb.velocity = Vector2.up * bounceForce;

    //        // Đặt block vào trạng thái "đã sử dụng" trong  giây
    //        isActivated = true;
    //        StartCoroutine(ResetTrampoline());
    //    }
    //}

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isActivated && applicableTags.Contains(collision.gameObject.tag))
        {
            if (collision.gameObject.name == "Trampoline")
                return;
            // Kích hoạt animation nhún xuống
            animator.SetTrigger("Bounce");
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.up * bounceForce;

            // Đặt block vào trạng thái "đã sử dụng" trong  giây
            isActivated = true;
            StartCoroutine(ResetTrampoline());
        }

    }


    private IEnumerator ResetTrampoline()
    {
        yield return new WaitForSeconds(bounceDelay);
        isActivated = false;
        animator.ResetTrigger("Bounce"); //Reset trang thai cho lan tiep theo
    }
}
