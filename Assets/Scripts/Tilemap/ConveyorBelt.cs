using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float conveyorSpeed = 5f; // Tốc độ di chuyển của băng chuyền
    public bool moveRight = true; // Chiều của băng chuyền, true = sang phải, false = sang trái

    private void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Tính toán hướng và tốc độ dựa trên chiều của băng chuyền
                float direction = moveRight ? 1 : -1;
                Vector2 conveyorForce = new Vector2(conveyorSpeed * direction, 0);

                // Kiểm tra hướng di chuyển của người chơi để tăng/giảm tốc độ
                float playerDirection = Mathf.Sign(playerRb.velocity.x);

                if (playerDirection == direction)
                {
                    // Người chơi đi cùng chiều với băng chuyền -> tăng tốc
                    playerRb.velocity = new Vector2(playerRb.velocity.x + conveyorSpeed * Time.deltaTime, playerRb.velocity.y);
                }
                else if (playerDirection != 0)
                {
                    // Người chơi đi ngược chiều với băng chuyền -> giảm tốc
                    playerRb.velocity = new Vector2(playerRb.velocity.x - conveyorSpeed * Time.deltaTime, playerRb.velocity.y);
                }
                else
                {
                    // Nếu người chơi không di chuyển, chỉ thêm lực của băng chuyền
                    playerRb.AddForce(conveyorForce);
                }
            }
        }
    }
}
