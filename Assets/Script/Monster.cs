using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    // พารามิเตอร์ของมอนสเตอร์
    public int health = 20;
    public GameObject coinPrefab; // Prefab ของเหรียญ
    public GameObject[] itemPrefabs; // ไอเทมที่สามารถดรอปได้
    public float itemDropChance = 0.3f; // โอกาสในการดรอปไอเทม
    public float speed = 2f; // ความเร็วในการเคลื่อนที่
    public int damage = 10; // ดาเมจที่มอนสเตอร์ทำให้ Player
    public float damageInterval = 1f; // เวลาระหว่างการทำดาเมจ
    private float damageTimer = 0f; // ตัวจับเวลาสำหรับการทำดาเมจ
    private Transform target; // เป้าหมายของมอนสเตอร์ (Player)

    private bool isCollidingWithPlayer = false; // เช็คว่ามอนสเตอร์ชนกับ Player หรือไม่

    void Start()
    {
        // ค้นหา Player ใน scene
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // ทำให้มอนสเตอร์เคลื่อนที่เข้าหา Player
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }

        // ทำดาเมจซ้ำหากมอนสเตอร์ชนกับ Player
        if (isCollidingWithPlayer)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                // ส่งดาเมจให้ Player
                PlayerController playerController = target.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(damage); // ทำดาเมจให้ Player
                    damageTimer = 0f; // รีเซ็ตตัวจับเวลา
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // เมื่อมอนสเตอร์ชนกับ Player
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = true; // ตั้งค่าให้มอนสเตอร์กำลังชนกับ Player
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // เมื่อมอนสเตอร์ออกจากการชนกับ Player
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = false; // ตั้งค่าให้มอนสเตอร์ไม่ชนกับ Player อีกต่อไป
        }
    }

    // ฟังก์ชันที่ใช้ในการรับดาเมจจากการโจมตี
    public void TakeDamage(int damage)
    {
        health -= damage; // ลดเลือดมอนสเตอร์
        if (health <= 0)
        {
            Die(); // มอนสเตอร์ตาย
        }
    }

    // ฟังก์ชันที่ทำเมื่อมอนสเตอร์ตาย
    private void Die()
    {
        // ให้ EXP กับผู้เล่น
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.GainEXP(10); // EXP 50 หรือเปลี่ยนเป็นค่าที่ต้องการ
        }

        // ดรอปเหรียญ
        Instantiate(coinPrefab, transform.position, Quaternion.identity);

        // สุ่มดรอปไอเทม
        if (Random.value < itemDropChance)
        {
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            Instantiate(itemPrefabs[randomIndex], transform.position, Quaternion.identity);
        }

        // ทำลาย GameObject ของมอนสเตอร์
        Destroy(gameObject);
    }

}
