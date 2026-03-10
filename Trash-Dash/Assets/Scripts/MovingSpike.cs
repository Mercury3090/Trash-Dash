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

    void Start()
    {
        originalScale = transform.localScale;

        transform.localScale = new Vector3(
            originalScale.x,
            minHeight,
            originalScale.z
        );

        waitTimer = waitAtBottom;
    }

    void Update()
    {
        if (waitTimer > 0)
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

        transform.localScale = new Vector3(
            originalScale.x,
            currentHeight,
            originalScale.z
        );
    }
}

