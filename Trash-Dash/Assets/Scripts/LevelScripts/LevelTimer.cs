using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    [Header("Star Thresholds")]
    public float threeStarTime = 20f;
    public float twoStarTime = 35f;

    private float currentTime = 0f;
    private bool timerRunning = true;

    public float CurrentTime => currentTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!timerRunning)
            return;

        currentTime += Time.deltaTime;
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = currentTime.ToString("F2") + "s";
        }
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void PauseTimer()
    {
        timerRunning = false;
    }

    public void ResumeTimer()
    {
        timerRunning = true;
    }

    public void ResetTimer()
    {
        currentTime = 0f;
        timerRunning = true;
        UpdateTimerUI();
    }

    public void SetTime(float newTime)
    {
        currentTime = newTime;
        UpdateTimerUI();
    }

    public int GetStarRating()
    {
        if (currentTime <= threeStarTime)
            return 3;
        else if (currentTime <= twoStarTime)
            return 2;
        else
            return 1;
    }
}