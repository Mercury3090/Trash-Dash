using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;

    private Rigidbody2D rb;
    private PlayerInputActions inputActions;

    private Vector2 moveInput;
    private bool isGrounded;
    private bool isOnLadder;
    private bool isClimbing;
    private bool jumpHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();

        inputActions.Player.Jump.performed += ctx =>
        {
            jumpHeld = true;
            TryJump();
        };

        inputActions.Player.Jump.canceled += ctx =>
        {
            jumpHeld = false;
        };
    }

    private void Start()
    {
        currentHealth = maxHealth;
        rb.gravityScale = normalGravity;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        CheckGround();

        if (isOnLadder && Mathf.Abs(moveInput.y) > 0.1f)
        {
            isClimbing = true;
        }
        else if (!isOnLadder)
        {
            isClimbing = false;
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleBetterJump();
    }

    private void HandleMovement()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(0f, moveInput.y * climbSpeed);
        }
        else
        {
            rb.gravityScale = normalGravity;
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
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
        if (isGrounded && !isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isOnLadder = true;
            Debug.Log("Entered ladder");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isOnLadder = false;
            isClimbing = false;
            rb.gravityScale = normalGravity;
            Debug.Log("Exited ladder");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took damage. Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}