using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // UI ����Ѻ Pause Menu
    private bool isPaused = false; // ��������١��ش�������

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // �� ESC �����Դ/�Դ Pause Menu
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
        Time.timeScale = 0f; // ��ش���Ңͧ��
        isPaused = true;
        pauseMenuUI.SetActive(true); // �ʴ����� Pause
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // ��Ѻ����������
        isPaused = false;
        pauseMenuUI.SetActive(false); // ��͹���� Pause
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // �������һ���
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ��Ŵ�ҡ�������
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f; // �������һ���
        SceneManager.LoadScene(sceneName); // ��Ŵ�ҡ����˹�
    }
}
