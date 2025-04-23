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

        

        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable target = collision.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }


    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
