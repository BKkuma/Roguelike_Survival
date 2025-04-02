using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour
{
    public Text timerText;
    private float gameTime = 0f;
    private bool isRunning = true;

    void Update()
    {
        if (isRunning)
        {
            gameTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60);
            int seconds = Mathf.FloorToInt(gameTime % 60);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        gameTime = 0f;
        isRunning = true;
    }
}
