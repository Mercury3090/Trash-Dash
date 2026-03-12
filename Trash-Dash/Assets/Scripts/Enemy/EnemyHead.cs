using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    public Enemy enemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemy == null || enemy.isDead) return;

        PlayerController player = collision.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

            if (playerRb != null && playerRb.linearVelocity.y <= 0f)
            {
                enemy.Die();
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, enemy.stompBounceForce);
            }
        }
    }
}