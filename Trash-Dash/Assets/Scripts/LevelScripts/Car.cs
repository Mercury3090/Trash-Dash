using UnityEngine;

public class Car : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public bool moveLeft = true;

    [Header("Loop Points")]
    public float leftEdge = -15f;
    public float rightEdge = 15f;

    private void Update()
    {
        float direction = moveLeft ? 1f : -1f;

        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        if (moveLeft && transform.position.x > rightEdge)
        {
            transform.position = new Vector3(leftEdge, transform.position.y, transform.position.z);
        }
        else if (!moveLeft && transform.position.x < leftEdge)
        {
            transform.position = new Vector3(rightEdge, transform.position.y, transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerHealth = collision.GetComponent<PlayerController>();

            if (playerHealth != null)
            {
                playerHealth.Die();
            }
        }
    }
}