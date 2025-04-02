using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGameUI : MonoBehaviour
{
    public Text finalTimeText;
    public Text finalLevelText;

    void Start()
    {
        // �֧��ҷ��ѹ�֡���
        float finalTime = PlayerPrefs.GetFloat("FinalTime", 0);
        int finalLevel = PlayerPrefs.GetInt("FinalLevel", 1);

        // �ʴ��ź� UI
        finalTimeText.text = "Time Played: " + finalTime.ToString("F2") + " sec";
        finalLevelText.text = "Final Level: " + finalLevel;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("MainScene"); // ��Ŵ�ҡ����ѡ
    }
}
