using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelCompleteUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI finalTimeText;

    [Header("Stars")]
    public Image star1;
    public Image star2;
    public Image star3;

    public Sprite emptyStar;   // grey square
    public Sprite filledStar;  // actual star

    [Header("Animation")]
    public float starPopScale = 1.2f;
    public float popDuration = 0.15f;
    public float delayBetweenStars = 0.2f;

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void ShowResults(float finalTime, int stars)
    {
        if (panel != null)
            panel.SetActive(true);

        if (finalTimeText != null)
            finalTimeText.text = "Time: " + finalTime.ToString("F2") + "s";

        // Set all to empty first
        star1.sprite = emptyStar;
        star2.sprite = emptyStar;
        star3.sprite = emptyStar;

        // Start animation
        StartCoroutine(ShowStarsSequentially(stars));

        Time.timeScale = 0f;
    }

    IEnumerator ShowStarsSequentially(int stars)
    {
        yield return new WaitForSecondsRealtime(0.2f);

        if (stars >= 1)
        {
            SetStar(star1);
            yield return new WaitForSecondsRealtime(delayBetweenStars);
        }

        if (stars >= 2)
        {
            SetStar(star2);
            yield return new WaitForSecondsRealtime(delayBetweenStars);
        }

        if (stars >= 3)
        {
            SetStar(star3);
        }
    }

    void SetStar(Image star)
    {
        star.sprite = filledStar;
        StartCoroutine(PopAnimation(star.transform));
    }

    IEnumerator PopAnimation(Transform target)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = Vector3.one * starPopScale;

        float t = 0f;

        // Scale up
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            target.localScale = Vector3.Lerp(originalScale, targetScale, t / popDuration);
            yield return null;
        }

        t = 0f;

        // Scale back down
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            target.localScale = Vector3.Lerp(targetScale, originalScale, t / popDuration);
            yield return null;
        }

        target.localScale = originalScale;
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextScene);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}