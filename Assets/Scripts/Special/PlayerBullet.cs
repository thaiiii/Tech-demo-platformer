using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 10f;
    public float maxDistance = 10f;
    private Vector3 startPosition;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindFirstObjectByType<Player>().gameObject;
        startPosition = player.transform.position;  
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * player.transform.localScale.x * speed * Time.deltaTime);

        //Destroy if travel too far
        if (Vector3.Distance(startPosition, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
        }
        else if (collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject); //Tieu diet ke thu
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject); //Huy dan sau khi va cham
        }
    }
}
