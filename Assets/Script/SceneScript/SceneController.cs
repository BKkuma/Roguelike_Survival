using UnityEngine;
using UnityEngine.SceneManagement; // ใช้สำหรับการจัดการฉาก

public class SceneController : MonoBehaviour
{
    // ฟังก์ชันโหลดฉากใหม่
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // โหลดฉากที่ระบุ
    }

    // ฟังก์ชันสำหรับออกจากเกม
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // สำหรับเล่นใน Unity Editor
#else
            Application.Quit(); // สำหรับในเกมที่ build ไว้
#endif
    }
}
