using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    public float height = 3f;      // How far it moves up/down
    public float speed = 2f;       // Movement speed
    public float startOffset = 0f; // Timing offset for different platforms

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin((Time.time + startOffset) * speed) * height;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}