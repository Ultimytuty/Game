using UnityEngine;
using UnityEngine.InputSystem;

public class JohnMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float groundDragDistance = 0.5f;
    public LayerMask groundLayer;
    private AK47main ak47;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private int groundContactCount;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ak47 = GetComponent<AK47main>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGroundContact();
        movement();
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            Jump();
        }
        
    }

    private void movement()
    {
        // Get the horizontal input using the new Input System
        float horizontalInput = 0f;
        
        // Detect A/D key presses for horizontal movement
        if (Keyboard.current.dKey.isPressed) horizontalInput += 1f;
        if (Keyboard.current.aKey.isPressed) horizontalInput -= 1f;
        
        // Only apply horizontal velocity if there's input; preserve knockback velocity otherwise
        if (rb != null)
        {
            if (horizontalInput != 0)
            {
                rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
            }
            // If no input, let existing velocity (including knockback) persist and decay
        }
        
        // Flip character sprite based on direction
        if (horizontalInput != 0 && spriteRenderer != null)
        {
            spriteRenderer.flipX = horizontalInput < 0;
        }
    }
    
    private void CheckGroundContact()
    {
        // Use raycasts from the bottom of the character to detect ground
        Collider2D collider = GetComponent<Collider2D>();
        Bounds bounds = collider.bounds;
        
        // Cast rays from bottom-left, bottom-center, and bottom-right
        float rayDistance = 0.1f; // distance to check below character
        isGrounded = false;
        
        // Left ray
        Vector2 leftRayStart = new Vector2(bounds.min.x, bounds.min.y);
        RaycastHit2D leftHit = Physics2D.Raycast(leftRayStart, Vector2.down, rayDistance, groundLayer);
        
        // Center ray
        Vector2 centerRayStart = new Vector2(bounds.center.x, bounds.min.y);
        RaycastHit2D centerHit = Physics2D.Raycast(centerRayStart, Vector2.down, rayDistance, groundLayer);
        
        // Right ray
        Vector2 rightRayStart = new Vector2(bounds.max.x, bounds.min.y);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayStart, Vector2.down, rayDistance, groundLayer);
        
        // Grounded if any ray hits and doesn't hit the character itself
        if (leftHit.collider != null && leftHit.collider.gameObject != gameObject)
            isGrounded = true;
        if (centerHit.collider != null && centerHit.collider.gameObject != gameObject)
            isGrounded = true;
        if (rightHit.collider != null && rightHit.collider.gameObject != gameObject)
            isGrounded = true;
    }
    
    private void Jump()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}