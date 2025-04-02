using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpForce = 10.0f;

    private Rigidbody2D rb;
    private bool isGrounded;
    public bool isJumping;
    public bool isFalling;
    private bool isWalking;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    private PlayerAnimation playerAnimation;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    public void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (playerAnimation != null)
        {
            playerAnimation.SetWalking(moveInput != 0);
        }

        if (moveInput != 0)
        {
            GetComponent<SpriteRenderer>().flipX = moveInput < 0;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            //playerAnimation.PlayJumpAnimation();
            isJumping = true;
            isWalking = false;
            playerAnimation.SetJumping(isJumping);
            playerAnimation.SetWalking(isWalking);

        }


        bool isAirborne = !isGrounded;

        if (rb.linearVelocity.y < 0.1f && isJumping)
        {
            isFalling = true;
            isJumping = false;
            playerAnimation.SetFalling(isFalling);
        }

        if (isGrounded && !isJumping && isFalling)
        {
            playerAnimation.PlayLanding();
            isFalling = false;            
            isWalking = true;
 
            //Debug.Log("Grounded");
        }


    }

}