using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 10f;
    public float climbSpeed = 4f;
    public float normalGravity = 4f;

    [Header("Jump Feel")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);

    [Header("Wall Checks")]
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public Vector2 wallCheckSize = new Vector2(0.1f, 0.8f);

    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;

    private Rigidbody2D rb;

    private float moveInput;
    private float verticalInput;
    private bool isGrounded;
    private bool isOnLadder;
    private bool isClimbing;
    private bool jumpHeld;

    private bool touchingLeftWall;
    private bool touchingRightWall;

    private Vector3 spawnPoint;
    private Vector3 respawnPoint;
    private bool hasCheckpoint = false;
    private bool isRespawning = false;

    private float checkpointTime = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        rb.gravityScale = normalGravity;

        spawnPoint = transform.position;
        respawnPoint = spawnPoint;
        checkpointTime = 0f;
    }

    private void Update()
    {
        if (isRespawning) return;

        moveInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        jumpHeld = Input.GetButton("Jump");

        isGrounded = Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0f,
            groundLayer
        );

        touchingLeftWall = Physics2D.OverlapBox(
            wallCheckLeft.position,
            wallCheckSize,
            0f,
            groundLayer
        );

        touchingRightWall = Physics2D.OverlapBox(
            wallCheckRight.position,
            wallCheckSize,
            0f,
            groundLayer
        );

        if (Input.GetButtonDown("Jump"))
        {
            TryJump();
        }

        if (isOnLadder && Mathf.Abs(verticalInput) > 0.1f)
        {
            isClimbing = true;
        }

        if (!isOnLadder)
        {
            isClimbing = false;
        }
    }

    private void FixedUpdate()
    {
        if (isRespawning) return;

        HandleMovement();
        HandleBetterJump();
    }

    private void HandleMovement()
    {
        float horizontalVelocity = moveInput * moveSpeed;

        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(horizontalVelocity, verticalInput * climbSpeed);
        }
        else
        {
            rb.gravityScale = normalGravity;

            if (!isGrounded)
            {
                if ((touchingLeftWall && moveInput < 0f) || (touchingRightWall && moveInput > 0f))
                {
                    horizontalVelocity = 0f;
                }
            }

            rb.linearVelocity = new Vector2(horizontalVelocity, rb.linearVelocity.y);
        }
    }

    private void HandleBetterJump()
    {
        if (isClimbing)
            return;

        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0f && !jumpHeld)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    private void TryJump()
    {
        if (isClimbing)
        {
            isClimbing = false;
            rb.gravityScale = normalGravity;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            return;
        }

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isOnLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isOnLadder = false;
            isClimbing = false;
            rb.gravityScale = normalGravity;
        }
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        respawnPoint = newCheckpoint;
        hasCheckpoint = true;

        if (LevelTimer.Instance != null)
        {
            checkpointTime = LevelTimer.Instance.CurrentTime;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isRespawning) return;

        currentHealth -= damage;
        Debug.Log("Player took damage. Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (!isRespawning)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        Debug.Log("Player died");

        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.PauseTimer();
        }

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = normalGravity;

        yield return new WaitForSeconds(0.2f);

        if (!hasCheckpoint)
        {
            respawnPoint = spawnPoint;

            if (LevelTimer.Instance != null)
            {
                LevelTimer.Instance.ResetTimer();
            }
        }
        else
        {
            if (LevelTimer.Instance != null)
            {
                LevelTimer.Instance.SetTime(checkpointTime);
            }
        }

        transform.position = respawnPoint;
        currentHealth = maxHealth;

        isClimbing = false;
        isOnLadder = false;

        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.ResumeTimer();
        }

        isRespawning = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }

        if (wallCheckLeft != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(wallCheckLeft.position, wallCheckSize);
        }

        if (wallCheckRight != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(wallCheckRight.position, wallCheckSize);
        }
    }
}