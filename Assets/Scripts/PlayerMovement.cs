using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move*speed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump")&& isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("Jumped", true);
            isGrounded = false;
        }

        // Detect if falling (velocity.y < 0) or jumping (velocity.y > 0)
        if (rb.linearVelocity.y > 0.1f)
        {
            animator.SetBool("Jumped", true);
        }
        else if (rb.linearVelocity.y < -0.1f)
        {
            animator.SetBool("Jumped", true); // Keep Jump animation if falling
        }

        //flips character if they are going left or right
        if (move > 0)
        {
            spriteRenderer.flipX = false; //face right
        }
        else if (move < 0)
        {
            spriteRenderer.flipX = true; //faces left
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("Jumped", false);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
