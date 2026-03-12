using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Ranges")]
    public float detectionRange = 6f;
    public float losePlayerRange = 8f;
    public float attackRange = 1.2f;

    [Header("Checks")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.2f;

    public Transform groundCheck;
    public float groundCheckDistance = 0.8f;

    public LayerMask groundLayer;

    [Header("Attack")]
    public int damage = 1;
    public float attackCooldown = 1f;

    [Header("Stomp")]
    public bool isDead = false;
    public float stompBounceForce = 10f;

    [Header("References")]
    public Transform player;

    private Rigidbody2D rb;
    private Vector3 originalScale;

    private bool movingRight = true;
    private bool chasingPlayer = false;
    private float lastAttackTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (!chasingPlayer && distanceToPlayer <= detectionRange)
        {
            chasingPlayer = true;
        }

        if (chasingPlayer && distanceToPlayer >= losePlayerRange)
        {
            chasingPlayer = false;
        }
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            FacePlayer();
        }
        else if (chasingPlayer)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        float direction = movingRight ? 1f : -1f;

        bool wallAhead = Physics2D.Raycast(
            wallCheck.position,
            Vector2.right * direction,
            wallCheckDistance,
            groundLayer
        );

        Vector2 groundRayOrigin = new Vector2(
            groundCheck.position.x + direction * 0.5f,
            groundCheck.position.y
        );

        bool groundAhead = Physics2D.Raycast(
            groundRayOrigin,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        if (wallAhead || !groundAhead)
        {
            movingRight = !movingRight;
            direction = movingRight ? 1f : -1f;
        }

        rb.linearVelocity = new Vector2(direction * patrolSpeed, rb.linearVelocity.y);
        Flip(direction);
    }

    private void ChasePlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        bool wallAhead = Physics2D.Raycast(
            wallCheck.position,
            Vector2.right * direction,
            wallCheckDistance,
            groundLayer
        );

        Vector2 groundRayOrigin = new Vector2(
            groundCheck.position.x + direction * 0.5f,
            groundCheck.position.y
        );

        bool groundAhead = Physics2D.Raycast(
            groundRayOrigin,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        if (wallAhead || !groundAhead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);
        Flip(direction);
    }

    private void AttackPlayer()
    {
        FacePlayer();

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.Die();
            }

            lastAttackTime = Time.time;
        }
    }

    private void FacePlayer()
    {
        float direction = player.position.x - transform.position.x;
        Flip(direction);
    }

    private void Flip(float direction)
    {
        if (direction > 0)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
        else if (direction < 0)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, losePlayerRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (wallCheck != null)
        {
            float direction = movingRight ? 1f : -1f;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(
                wallCheck.position,
                wallCheck.position + Vector3.right * direction * wallCheckDistance
            );
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                groundCheck.position,
                groundCheck.position + Vector3.down * groundCheckDistance
            );
        }
    }
}