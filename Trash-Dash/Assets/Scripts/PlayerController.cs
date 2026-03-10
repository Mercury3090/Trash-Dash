using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;
    public float climbSpeed = 4f;
    public float normalGravity = 4f;

    [Header("Checks")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public Transform ladderCheck;
    public float ladderCheckRadius = 0.25f;
    public LayerMask ladderLayer;

    [Header("Health")]
    public int maxHealth = 3;

    private int currentHealth;
    private Rigidbody2D rb;
    private PlayerInputActions inputActions;

    private Vector2 moveInput;
    private bool isGrounded;
    private bool isOnLadder;
    private bool isClimbing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();

        inputActions.Player.Jump.performed += ctx => Jump();
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
        CheckLadder();
        UpdateClimbingState();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, moveInput.y * climbSpeed);
        }
        else
        {
            rb.gravityScale = normalGravity;
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    private void Jump()
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

    private void CheckLadder()
    {
        isOnLadder = Physics2D.OverlapCircle(ladderCheck.position, ladderCheckRadius, ladderLayer);
    }

    private void UpdateClimbingState()
    {
        if (isOnLadder && Mathf.Abs(moveInput.y) > 0.1f)
        {
            isClimbing = true;
        }
        else if (!isOnLadder)
        {
            isClimbing = false;
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

        if (ladderCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(ladderCheck.position, ladderCheckRadius);
        }
    }
}