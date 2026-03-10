using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndOfLevel : MonoBehaviour
{
    public float delay = 1.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if (player != null)
        {
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