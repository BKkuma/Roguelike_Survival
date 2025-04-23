using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage;
    public bool isSpecialBullet = false; // ����������礡���ع�����

    void Start()
    {
        // �֧��� damage �ҡ Player ���ͨҡ��ҷ���˹�
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
