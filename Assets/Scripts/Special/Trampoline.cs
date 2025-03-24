using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public bool isActivated = false;
    public float bounceForce = 20f; //Lưc bat cho nguoi choi
    public float bounceDelay = 1f;
    public float boostAngle;
    private Animator animator;
    public List<string> applicableTags;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isActivated && applicableTags.Contains(collision.gameObject.tag))
        {
            if (collision.gameObject.name == "Trampoline")
                return;

            // Kích hoạt animation nhún xuống
            animator.SetTrigger("Bounce");
            AudioManager.Instance.PlaySFX("trampoline");
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (boostAngle != transform.rotation.eulerAngles.z)
                boostAngle = transform.rotation.eulerAngles.z;

            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<Player>(); 
                if (boostAngle % 180 != 0 )
                    StartCoroutine(collision.gameObject.GetComponent<PlayerAbilities>().TemporaryStopUpdatingHorizontalVelocity());
                rb.velocity = new Vector2(-Mathf.Sin(boostAngle * Mathf.Deg2Rad), Mathf.Cos(boostAngle * Mathf.Deg2Rad)).normalized * bounceForce;
            }
            else if (collision.gameObject.CompareTag("Robot"))
            {
                collision.gameObject.GetComponent<Robot>();
                if (boostAngle % 180 != 0)
                    StartCoroutine(collision.gameObject.GetComponent<Robot>().TemporaryStopUpdatingHorizontalVelocity());
                rb.velocity = new Vector2(-Mathf.Sin(boostAngle * Mathf.Deg2Rad), Mathf.Cos(boostAngle * Mathf.Deg2Rad)).normalized * bounceForce;
            }
            else
            {
                rb.velocity = new Vector2(-Mathf.Sin(boostAngle * Mathf.Deg2Rad), Mathf.Cos(boostAngle * Mathf.Deg2Rad)).normalized * bounceForce;
            }

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
