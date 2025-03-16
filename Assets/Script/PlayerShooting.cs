using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject firePoint; // ตำแหน่งของการยิงกระสุน
    public GameObject projectilePrefab; // Prefab ของกระสุน
    public float projectileSpeed = 10f; // ความเร็วของกระสุน
    public float fireRate = 0.5f; // ระยะเวลาระหว่างการยิงแต่ละครั้ง (ยิงอัตโนมัติ)

    private float nextFireTime = 0f; // เวลาถัดไปที่สามารถยิงได้

    void Update()
    {
        // เลื่อนตำแหน่ง Fire Point ไปยังตำแหน่งของเมาส์
        AimAtMouse();

        // หากถึงเวลาที่กำหนดในการยิง (อัตโนมัติ)
        if (Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + fireRate; // อัปเดตเวลาในการยิงถัดไป
        }
    }

    private void AimAtMouse()
    {
        // คำนวณทิศทางจาก FirePoint ไปยังตำแหน่งเมาส์
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // กำหนดค่า Z ให้เป็น 0 เพราะเราใช้ 2D
        Vector2 direction = (mousePosition - firePoint.transform.position).normalized;

        // คำนวณมุมที่ FirePoint ต้องหมุนเพื่อเล็งไปยังเมาส์
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.transform.rotation = Quaternion.Euler(0, 0, angle); // หมุน FirePoint ตามมุมที่คำนวณ
    }

    private void FireProjectile()
    {
        // ถ้า Prefab ของกระสุนและ FirePoint มีค่า
        if (projectilePrefab != null && firePoint != null)
        {
            // สร้างกระสุนจาก Prefab ที่ตำแหน่งของ FirePoint
            GameObject projectile = Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            // กำหนดความเร็วและทิศทางของกระสุน
            if (rb != null)
            {
                rb.velocity = firePoint.transform.right * projectileSpeed;
            }
        }
    }
}
