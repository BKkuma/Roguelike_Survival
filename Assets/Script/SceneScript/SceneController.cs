using UnityEngine;
using UnityEngine.SceneManagement; // ������Ѻ��èѴ��éҡ

public class SceneController : MonoBehaviour
{
    // �ѧ��ѹ��Ŵ�ҡ����
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // ��Ŵ�ҡ����к�
    }

    // �ѧ��ѹ����Ѻ�͡�ҡ��
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // ����Ѻ���� Unity Editor
#else
            Application.Quit(); // ����Ѻ������ build ���
#endif
    }
}
