using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;        // ตัวแปรสำหรับเก็บตำแหน่ง Player
    public float smoothSpeed = 0.125f;  // ความนุ่มนวลในการเคลื่อนที่ของกล้อง
    public Vector3 offset;            // ระยะห่างระหว่างกล้องกับ Player

    void LateUpdate()
    {
        if (player == null) return;  // ถ้าไม่ได้กำหนด Player ให้หยุดการทำงาน

        // ตำแหน่งที่ต้องการให้กล้องไป (ตำแหน่งของ Player + ระยะห่างที่กำหนด)
        Vector3 desiredPosition = player.position + offset;

        // เคลื่อนที่กล้องไปที่ตำแหน่งใหม่ด้วยการเคลื่อนที่แบบนุ่มนวล
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // ตั้งตำแหน่งกล้องใหม่
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z); // เราไม่ให้กล้องเคลื่อนที่ในแกน Z

        transform.rotation = Quaternion.identity;  // หยุดการหมุนของกล้องให้เป็น (0, 0, 0)
    }
}

