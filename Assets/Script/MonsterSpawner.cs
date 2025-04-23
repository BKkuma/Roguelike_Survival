using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // Prefab ของมอนสเตอร์
    public GameObject monsterPrefab2;
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
        GameObject prefabToSpawn = monsterPrefab; // เริ่มจากตัวธรรมดา

        PlayerController player = FindObjectOfType<PlayerController>();
        int playerLevel = player != null ? player.level : 1;
        float gameTime = Time.timeSinceLevelLoad;

        // เงื่อนไข spawn ตัวที่ 2:
        if (playerLevel >= 10 || gameTime > 60f) // เลเวล 5 หรือเล่นเกิน 60 วิ
        {
            if (Random.value < 0.2f) // 20% โอกาส spawn ตัวที่ 2
            {
                prefabToSpawn = monsterPrefab2;
            }
        }

        GameObject monsterGO = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        currentMonsterCount++;

        // ปรับค่าพลังตามเลเวลผู้เล่น
        if (player != null)
        {
            Monster monster = monsterGO.GetComponent<Monster>();
            if (monster != null)
            {
                monster.health += playerLevel * 2;
                monster.damage += playerLevel * 2;
            }
        }
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


