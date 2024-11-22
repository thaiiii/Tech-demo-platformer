using UnityEngine;
using UnityEngine.UIElements;

public class BoostTilemap : MonoBehaviour
{
    public float boostForce = 20f;  // Lực đẩy cố định khi chạm vào BoostTilemap
    public Rigidbody2D playerRb;
    public Vector2 boostDirection;

    private void Start()
    {
        playerRb = FindObjectOfType<Player>().GetComponent<Rigidbody2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (boostDirection == Vector2.up)
            {
                Vector2 boostvelocity = boostDirection * boostForce;
                playerRb.velocity = boostvelocity;
            }
            else if (boostDirection == Vector2.right || boostDirection == Vector2.left)
            {
                Vector2 boostvelocity = boostDirection * boostForce;
                playerRb.velocity = boostvelocity;
            }

        }
    }
}
