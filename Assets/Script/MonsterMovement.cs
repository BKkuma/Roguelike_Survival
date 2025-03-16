using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public float speed = 2f; // ความเร็วในการเคลื่อนที่
    public float separationDistance = 1.5f; // ระยะห่างที่ต้องการระหว่างมอนสเตอร์
    public float separationForce = 2f; // แรงที่ใช้แยกมอนสเตอร์ออกจากกัน

    private Transform player; // ตำแหน่งผู้เล่น
    private Rigidbody2D rb; // Rigidbody มอนสเตอร์
    private SpriteRenderer spriteRenderer; // ใช้เพื่อหมุนทิศทางของมอนสเตอร์

    void Start()
    {
        // หา Tag "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // ใช้ SpriteRenderer ในการหมุนทิศทาง
    }

    void Update()
    {
        if (player != null)
        {
            // คำนวณการเคลื่อนที่เข้าหาผู้เล่น
            Vector2 directionToPlayer = (player.position - transform.position).normalized;

            // เพิ่มแรงผลักเพื่อแยกมอนสเตอร์
            Vector2 separation = GetSeparationForce();

            // รวมการเคลื่อนที่เข้าหาผู้เล่นและการเว้นระยะห่าง
            Vector2 finalMovement = (directionToPlayer * speed + separation * separationForce) * Time.deltaTime;

            rb.MovePosition(rb.position + finalMovement);

            // หมุนมอนสเตอร์ตามทิศทางการเคลื่อนที่
            FlipMonster(finalMovement.x);
        }
    }

    private Vector2 GetSeparationForce()
    {
        Vector2 separationForce = Vector2.zero;

        // หา Collider รอบตัวมอนสเตอร์ในระยะ separationDistance
        Collider2D[] nearbyMonsters = Physics2D.OverlapCircleAll(transform.position, separationDistance);

        foreach (Collider2D collider in nearbyMonsters)
        {
            if (collider.gameObject != gameObject && collider.CompareTag("Monster"))
            {
                // คำนวณทิศทางผลักออกจากมอนสเตอร์ใกล้เคียง
                Vector2 directionAway = (transform.position - collider.transform.position).normalized;
                separationForce += directionAway;
            }
        }

        return separationForce.normalized; // ปรับให้มีขนาด 1
    }

    // ฟังก์ชันหันมอนสเตอร์ตามทิศทางการเคลื่อนที่
    private void FlipMonster(float movementX)
    {
        if (movementX > 0)
        {
            spriteRenderer.flipX = false; // หันขวา
        }
        else if (movementX < 0)
        {
            spriteRenderer.flipX = true; // หันซ้าย
        }
    }

    void OnDrawGizmosSelected()
    {
        // แสดงระยะเว้นห่างของมอนสเตอร์ใน Scene View
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationDistance);
    }
}
