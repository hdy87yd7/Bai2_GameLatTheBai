using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float gameDuration = 300f;

    private float timeRemaining;
    private bool isRunning;

    private void Awake()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;
        }

        UpdateTimerText();
    }

    public void StartTimer()
    {
        if (timeRemaining > 0f)
        {
            isRunning = true;
        }
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        timeRemaining = gameDuration;
        UpdateTimerText();
    }

    public bool HasTimeRunOut()
    {
        return timeRemaining <= 0f;
    }

    private void UpdateTimerText()
    {
        int totalSeconds = Mathf.CeilToInt(timeRemaining);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
