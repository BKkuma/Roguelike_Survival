using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // Prefab ของมอนสเตอร์
    public float spawnInterval = 5f; // ระยะเวลาการเกิดของมอนสเตอร์
    public int maxMonsters = 10; // จำนวนมอนสเตอร์สูงสุดในฉาก
    public float spawnDistanceFromCamera = 2f; // ระยะห่างจากขอบจอที่จะเกิดมอนสเตอร์

    private Camera mainCamera;
    private int currentMonsterCount = 0;
    private float elapsedTime = 0f;

    void Start()
    {
        // อ้างอิงถึงกล้องหลักในฉาก
        mainCamera = Camera.main;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= spawnInterval && currentMonsterCount < maxMonsters)
        {
            SpawnMonster();
            elapsedTime = 0f;

            // เพิ่มจำนวนมอนสเตอร์สูงสุดเมื่อเวลาผ่านไป
            maxMonsters += Mathf.FloorToInt(Time.timeSinceLevelLoad / 30); // เพิ่มทุก 30 วินาที
            spawnInterval = Mathf.Max(0.5f, spawnInterval - 0.1f); // ลดเวลาการเกิด
        }
    }

    private void SpawnMonster()
    {
        Vector3 spawnPosition = GetRandomPositionOutsideCamera();
        Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        currentMonsterCount++;
    }

    private Vector3 GetRandomPositionOutsideCamera()
    {
        // ขอบของกล้องในหน่วยโลก (World Space)
        Vector3 screenMin = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 screenMax = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        // สุ่มตำแหน่งเกิดนอกขอบจอ
        float x, y;

        // สุ่มเกิดจากขอบแนวนอนหรือแนวตั้ง
        if (Random.value > 0.5f)
        {
            // เกิดนอกขอบแนวนอน
            x = Random.Range(screenMin.x - spawnDistanceFromCamera, screenMax.x + spawnDistanceFromCamera);
            y = Random.value > 0.5f ? screenMin.y - spawnDistanceFromCamera : screenMax.y + spawnDistanceFromCamera;
        }
        else
        {
            // เกิดนอกขอบแนวตั้ง
            y = Random.Range(screenMin.y - spawnDistanceFromCamera, screenMax.y + spawnDistanceFromCamera);
            x = Random.value > 0.5f ? screenMin.x - spawnDistanceFromCamera : screenMax.x + spawnDistanceFromCamera;
        }

        return new Vector3(x, y, 0);
    }
}


