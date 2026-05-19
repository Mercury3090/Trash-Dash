using UnityEngine;

public class MovingSpike : MonoBehaviour
{
    [Header("Spike Size")]
    public float minHeight = 0.1f;
    public float maxHeight = 1f;

    [Header("Timing")]
    public float growSpeed = 4f;
    public float waitAtTop = 1f;
    public float waitAtBottom = 1f;

    private bool growing = true;
    private float waitTimer;

    private Vector3 originalScale;
    private float fixedBottomY;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;

        if (sr == null)
        {
            Debug.LogError("MovingSpike needs a SpriteRenderer on the same object.");
            return;
        }

        // Store the world-space bottom of the sprite
        fixedBottomY = sr.bounds.min.y;

        SetSpikeHeight(minHeight);
        waitTimer = waitAtBottom;
    }

    void Update()
    {
        if (sr == null) return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        float currentHeight = transform.localScale.y;

        if (growing)
        {
            currentHeight += growSpeed * Time.deltaTime;

            if (currentHeight >= maxHeight)
            {
                currentHeight = maxHeight;
                growing = false;
                waitTimer = waitAtTop;
            }
        }
        else
        {
            currentHeight -= growSpeed * Time.deltaTime;

            if (currentHeight <= minHeight)
            {
                currentHeight = minHeight;
                growing = true;
                waitTimer = waitAtBottom;
            }
        }

        SetSpikeHeight(currentHeight);
    }

    private void SetSpikeHeight(float newHeight)
    {
        // Apply new Y scale
        transform.localScale = new Vector3(
            originalScale.x,
            newHeight,
            originalScale.z
        );

        // After scaling, get the sprite's current bottom in world space
        float currentBottomY = sr.bounds.min.y;

        // Move the object so the bottom stays where it started
        float difference = fixedBottomY - currentBottomY;
        transform.position += new Vector3(0f, difference, 0f);
    }
}