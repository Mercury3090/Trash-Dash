using UnityEngine;

public class ArrowFadeTrigger : MonoBehaviour
{
    [Header("Fade Settings")]
    [Range(0f, 1f)] public float fadedAlphaMultiplier = 0.3f;
    public float fadeSpeed = 8f;

    [Header("Bobbing Settings")]
    public float bobSpeed = 2f;
    public float bobAmount = 0.08f;

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    private float targetMultiplier = 1f;
    private float currentMultiplier = 1f;

    private Vector3 startPos;

    private void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        startPos = transform.position;

        if (spriteRenderers.Length == 0)
        {
            Debug.LogWarning("No SpriteRenderers found on arrow: " + gameObject.name);
            return;
        }

        originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }

        ApplyAlphaMultiplier(1f); // start fully visible
    }

    private void Update()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
            return;

        currentMultiplier = Mathf.MoveTowards(currentMultiplier, targetMultiplier, fadeSpeed * Time.deltaTime);
        ApplyAlphaMultiplier(currentMultiplier);

        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlayerController>() != null)
        {
            targetMultiplier = fadedAlphaMultiplier;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlayerController>() != null)
        {
            targetMultiplier = 1f;
        }
    }

    private void ApplyAlphaMultiplier(float multiplier)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null) continue;

            Color c = originalColors[i];
            c.a = originalColors[i].a * multiplier;
            spriteRenderers[i].color = c;
        }
    }
}