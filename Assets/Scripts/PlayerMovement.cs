using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private SpudScript spudScript;
    private BoxCollider2D boxCollider;
    private Vector2 standingColliderSize;
    private Vector2 standingOffset;
    private bool isCrouching;

    // Expose crouching size & offset for easy adjustment in Unity Editor
    [Header("Crouching Collider Settings")]
    [SerializeField] private Vector2 crouchingOffset;
    [SerializeField] private Vector2 crouchingColliderSize;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spudScript = GetComponent<SpudScript>();
        boxCollider = GetComponent<BoxCollider2D>();

       // Save standing collider size from initial BoxCollider2D
        standingColliderSize = boxCollider.size;
        standingOffset = boxCollider.offset;

    }

    // Update is called once per frame
    void Update()
    {   
        //pauses movement if the game is over
        if (spudScript.gameEnd)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
            return;
        }

        //moving left or right
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move*speed, rb.linearVelocity.y);


        //jumpinh
        if (Input.GetButtonDown("Jump") && isGrounded)
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

        //crouching 
        if (Input.GetKey(KeyCode.DownArrow))
        {
            animator.SetBool("Crouched", true);
            boxCollider.size = crouchingColliderSize;
            boxCollider.offset = crouchingOffset;
        }
        // Uncrouch when the player releases the down arrow
        else 
        {
            StartCoroutine(UncrouchWithDelay());
        }
    }

    private IEnumerator UncrouchWithDelay()
    {
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("Crouched", false);
        boxCollider.size = standingColliderSize;
        boxCollider.offset = standingOffset;
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

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); //stops sticking
        }
    }
}
