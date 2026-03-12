using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndOfLevel : MonoBehaviour
{
    public float delay = 1.5f;
    private bool levelEnding = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (levelEnding) return;

        Debug.Log("Something entered goal: " + collision.name);

        PlayerController player = collision.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            levelEnding = true;
            StartCoroutine(FinishLevel());
        }
    }

    IEnumerator FinishLevel()
    {
        Debug.Log("Level Complete!");

        yield return new WaitForSeconds(delay);

        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextScene);
    }
}