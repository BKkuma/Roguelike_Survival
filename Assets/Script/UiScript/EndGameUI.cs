using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGameUI : MonoBehaviour
{
    public Text finalTimeText;
    public Text finalLevelText;

    void Start()
    {
        // ดึงค่าที่บันทึกไว้
        float finalTime = PlayerPrefs.GetFloat("FinalTime", 0);
        int finalLevel = PlayerPrefs.GetInt("FinalLevel", 1);

        // แสดงผลบน UI
        finalTimeText.text = "Time Played: " + finalTime.ToString("F2") + " sec";
        finalLevelText.text = "Final Level: " + finalLevel;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("MainScene"); // โหลดฉากเกมหลัก
    }
}
