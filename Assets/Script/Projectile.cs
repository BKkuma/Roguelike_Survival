using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage;
    public bool isSpecialBullet = false; // เพิ่มตัวแปรเช็คกระสุนพิเศษ

    void Start()
    {
        // ดึงค่า damage จาก Player หรือจากค่าที่กำหนด
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            damage = player.damage;
        }

        if (isSpecialBullet) // ถ้าเป็นกระสุนพิเศษ
        {
            damage *= 2;  // เพิ่มดาเมจ 2 เท่า
        }

        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            Monster monster = collision.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
