using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // UI สำหรับ Pause Menu
    private bool isPaused = false; // เช็คว่าเกมถูกหยุดหรือไม่

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // กด ESC เพื่อเปิด/ปิด Pause Menu
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // หยุดเวลาของเกม
        isPaused = true;
        pauseMenuUI.SetActive(true); // แสดงเมนู Pause
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // กลับมาเล่นเกมต่อ
        isPaused = false;
        pauseMenuUI.SetActive(false); // ซ่อนเมนู Pause
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // รีเซ็ตเวลาปกติ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // โหลดฉากเดิมใหม่
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f; // รีเซ็ตเวลาปกติ
        SceneManager.LoadScene(sceneName); // โหลดฉากที่กำหนด
    }
}
