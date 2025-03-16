using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public GameObject terrainPrefab; // Prefab ของ Terrain
    public int chunkSize = 20; // ขนาดของ Chunk
    public int poolSize = 10; // ขนาดของ Object Pool

    private Transform player; // ตำแหน่งของ Player
    private Camera mainCamera; // กล้องหลัก
    private HashSet<Vector2Int> activeChunks = new HashSet<Vector2Int>(); // ตำแหน่ง Chunk ที่แสดงผลอยู่
    private Queue<GameObject> chunkPool = new Queue<GameObject>(); // Object Pool สำหรับ Chunk

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;

        // เตรียม Object Pool สำหรับ Chunk
        for (int i = 0; i < poolSize; i++)
        {
            GameObject chunk = Instantiate(terrainPrefab);
            chunk.SetActive(false); // ตั้งค่าให้ไม่แสดงในตอนเริ่มต้น
            chunkPool.Enqueue(chunk);
        }

        UpdateChunks(); // สร้าง Chunk เริ่มต้น
    }

    void Update()
    {
        UpdateChunks(); // ตรวจสอบและจัดการ Chunk ในทุกเฟรม
    }

    private void UpdateChunks()
    {
        HashSet<Vector2Int> chunksInView = GetChunksInCameraView();

        // สร้าง Chunk ใหม่ที่จำเป็น
        foreach (Vector2Int chunkPosition in chunksInView)
        {
            if (!activeChunks.Contains(chunkPosition))
            {
                SpawnChunk(chunkPosition);
            }
        }

        // ลบ Chunk ที่หลุดมุมกล้อง
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (Vector2Int chunkPosition in activeChunks)
        {
            if (!chunksInView.Contains(chunkPosition))
            {
                chunksToRemove.Add(chunkPosition);
            }
        }

        foreach (Vector2Int chunkPosition in chunksToRemove)
        {
            RemoveChunk(chunkPosition);
        }
    }

    private HashSet<Vector2Int> GetChunksInCameraView()
    {
        HashSet<Vector2Int> chunksInView = new HashSet<Vector2Int>();

        Vector3 cameraBottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 cameraTopRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        int minX = Mathf.FloorToInt(cameraBottomLeft.x / chunkSize);
        int maxX = Mathf.FloorToInt(cameraTopRight.x / chunkSize);
        int minY = Mathf.FloorToInt(cameraBottomLeft.y / chunkSize);
        int maxY = Mathf.FloorToInt(cameraTopRight.y / chunkSize);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                chunksInView.Add(new Vector2Int(x, y));
            }
        }

        return chunksInView;
    }

    private void SpawnChunk(Vector2Int chunkPosition)
    {
        Vector3 worldPosition = new Vector3(
            chunkPosition.x * chunkSize,
            chunkPosition.y * chunkSize,
            0
        );

        GameObject chunk;
        if (chunkPool.Count > 0)
        {
            chunk = chunkPool.Dequeue(); // ใช้ Chunk จาก Pool
        }
        else
        {
            chunk = Instantiate(terrainPrefab); // สร้างใหม่ถ้า Pool หมด
        }

        chunk.transform.position = worldPosition;
        chunk.SetActive(true);

        ChunkVisibilityHandler visibilityHandler = chunk.AddComponent<ChunkVisibilityHandler>();
        visibilityHandler.terrainGenerator = this;
        visibilityHandler.chunkPosition = chunkPosition;

        activeChunks.Add(chunkPosition);
    }

    // เปลี่ยนจาก private เป็น public หรือ internal ตามความต้องการของคุณ
    public void RemoveChunk(Vector2Int chunkPosition)
    {
        // ทำการลบ Chunk จาก activeChunks และค้นหาจากตำแหน่งที่เหมาะสม
        foreach (GameObject chunk in FindObjectsOfType<GameObject>())
        {
            if (chunk.activeSelf && chunk.transform.position == new Vector3(chunkPosition.x * chunkSize, chunkPosition.y * chunkSize, 0))
            {
                // ลบ chunk ทันทีที่ตำแหน่งตรงกับ chunkPosition
                Destroy(chunk);

                // ลบ chunk จาก activeChunks
                activeChunks.Remove(chunkPosition);
                break;  // ลบแค่หนึ่ง chunk ต่อครั้ง
            }
        }
    }


}

