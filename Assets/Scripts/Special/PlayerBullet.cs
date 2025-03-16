using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 10f;
    public float maxDistance = 10f;
    public float bulletDamage = 20f;
    private Vector3 startPosition;
    private float startDirection; // Lưu hướng bắn
    public LayerMask destroyableLayer; //Các layer chặn đc đạn

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = FindFirstObjectByType<Player>().gameObject;
        startPosition = player.transform.position;
        startDirection = Mathf.Sign(player.transform.localScale.x);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * startDirection * speed * Time.deltaTime);

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
        else if (collision.GetComponent<Enemy>() != null)
        {
            //Trừ máu ke thu
            collision.GetComponent<Enemy>().healthComponent.TakeDamage(bulletDamage);
            Destroy(gameObject);
        }
        else if (((1 << collision.gameObject.layer) & destroyableLayer) != 0)
        {
            Destroy(gameObject); //Huy dan sau khi va cham
        }
    }

}
