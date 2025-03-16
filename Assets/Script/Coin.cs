using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1; // จำนวนเหรียญที่เพิ่มเมื่อเก็บ

    private void OnBecameInvisible()
    {
        // ตรวจสอบว่า GameObject ยัง active อยู่ก่อนเริ่ม Coroutine
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(HandleCoinOutOfBounds());
        }
    }

    private IEnumerator HandleCoinOutOfBounds()
    {
        // รอ 3 วินาที
        yield return new WaitForSeconds(3f);

        // ตรวจสอบอีกครั้งว่า GameObject ยังคง active อยู่หลังจาก 3 วินาที
        if (gameObject.activeInHierarchy)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.coins += value; // เพิ่มเหรียญ
                Debug.Log("Coins (auto pickup after 3 seconds): " + player.coins);
            }

            // ทำลายเหรียญหลังจากที่นับว่าเก็บแล้ว
            Destroy(gameObject);
        }
    }
}
