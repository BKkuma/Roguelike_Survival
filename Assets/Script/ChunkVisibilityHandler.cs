using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkVisibilityHandler : MonoBehaviour
{
    public TerrainGenerator terrainGenerator;
    public Vector2Int chunkPosition;
    private Renderer chunkRenderer;

    void Start()
    {
        // ตรวจสอบว่า GameObject นี้มี Renderer หรือไม่
        chunkRenderer = GetComponent<Renderer>();

        if (chunkRenderer == null)
        {
            Debug.LogWarning("No Renderer found on chunk: " + gameObject.name);
        }
    }

    void Update()
    {
        if (chunkRenderer != null)
        {
            if (IsChunkVisible())
            {
                // ถ้า Chunk อยู่ในมุมมอง, ให้แสดง
                chunkRenderer.enabled = true;
            }
            else
            {
                // ถ้า Chunk หลุดจากมุมมอง, ให้ซ่อน
                chunkRenderer.enabled = false;
            }
        }
    }

    // ฟังก์ชันตรวจสอบว่า Chunk อยู่ในมุมมองกล้องหรือไม่
    bool IsChunkVisible()
    {
        if (chunkRenderer != null)
        {
            // ตรวจสอบว่า Renderer สามารถมองเห็นได้จากกล้องหรือไม่
            return chunkRenderer.isVisible;
        }
        return false;
    }
}
