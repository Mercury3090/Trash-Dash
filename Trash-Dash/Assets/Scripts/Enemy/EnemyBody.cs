using UnityEngine;

public class EnemyBodyKill : MonoBehaviour
{
    public Enemy enemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemy == null || enemy.isDead) return;

        PlayerController player = collision.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                // If the player is falling downward, likely stomping
                if (playerRb.linearVelocity.y <= 0f && player.transform.position.y > enemy.transform.position.y)
                {
                    return;
                }
            }

            player.Die();
        }
    }
}