using UnityEngine;
using System.Collections;

public class EndOfLevel : MonoBehaviour
{
    public float delay = 0.5f;
    private bool levelEnding = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (levelEnding) return;

        Debug.Log("Something entered goal: " + collision.name);

        PlayerController player = collision.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            levelEnding = true;
            StartCoroutine(FinishLevel(player));
        }
    }

    IEnumerator FinishLevel(PlayerController player)
    {
        Debug.Log("Level Complete!");

        LevelTimer timer = FindFirstObjectByType<LevelTimer>();
        LevelCompleteUI levelCompleteUI = FindFirstObjectByType<LevelCompleteUI>();

        float finalTime = 0f;
        int stars = 1;

        if (timer != null)
        {
            timer.StopTimer();
            finalTime = timer.CurrentTime;
            stars = timer.GetStarRating();
        }
        else
        {
            Debug.LogWarning("No LevelTimer found in scene.");
        }

        yield return new WaitForSeconds(delay);

        if (levelCompleteUI != null)
        {
            levelCompleteUI.ShowResults(finalTime, stars);
        }
        else
        {
            Debug.LogWarning("No LevelCompleteUI found in scene.");
        }
    }
}