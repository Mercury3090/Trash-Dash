using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    [Header("Visual Feedback")]
    public ParticleSystem checkpointEffect;
    public GameObject checkpointText;
    public float textDuration = 1.2f;
    public Color activatedColor = Color.green;

    private bool activated = false;
    private SpriteRenderer sr;
    private Vector3 originalScale;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;

        if (checkpointText != null)
        {
            checkpointText.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;

        PlayerController player = collision.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            activated = true;
            player.SetCheckpoint(transform.position);

            Debug.Log("Checkpoint reached!");

            if (sr != null)
            {
                sr.color = activatedColor;
            }

            if (checkpointEffect != null)
            {
                checkpointEffect.Play();
            }

            if (checkpointText != null)
            {
                StartCoroutine(ShowCheckpointText());
            }

            StartCoroutine(PopCheckpoint());
        }
    }

    private IEnumerator ShowCheckpointText()
    {
        checkpointText.SetActive(true);

        Vector3 startPos = checkpointText.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0f, 0.5f, 0f);

        float timer = 0f;

        while (timer < textDuration)
        {
            timer += Time.deltaTime;
            checkpointText.transform.localPosition = Vector3.Lerp(startPos, endPos, timer / textDuration);
            yield return null;
        }

        checkpointText.transform.localPosition = startPos;
        checkpointText.SetActive(false);
    }

    private IEnumerator PopCheckpoint()
    {
        Vector3 popScale = originalScale * 1.15f;
        float duration = 0.12f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, popScale, t / duration);
            yield return null;
        }

        t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(popScale, originalScale, t / duration);
            yield return null;
        }

        transform.localScale = originalScale;
    }
}