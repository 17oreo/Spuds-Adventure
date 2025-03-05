using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private Vector2 gunOffsetStanding; // Adjust in Editor
    [SerializeField] private Vector2 gunOffsetCrouching; // Adjust in Editor
    private Transform gunTransform;
    private Blaster blaster;


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
        blaster = GetComponent<Blaster>();

        gunTransform = transform.Find("Blaster");
        gunOffsetStanding = gunTransform.localPosition;
        blaster = gunTransform.GetComponent<Blaster>();

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
        float move = isCrouching ? 0 : Input.GetAxis("Horizontal"); 
        rb.linearVelocity = new Vector2(move*speed, rb.linearVelocity.y);


        //jumpinh
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            if (isCrouching)
            {
                animator.SetBool("Crouched", false);
                isCrouching = false;
                UpdateGunPosition(false);
            }
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("Jumped", true);
            isGrounded = false;
        }

        //flips character if they are going left or right
        if (move > 0)
        {
            Flip(false);//face right
            
        }
        else if (move < 0)
        {
            Flip(true);//faces left
            
        }

        //crouching 
        if (Input.GetKey(KeyCode.DownArrow) /*&& isGrounded*/)
        {   

            isCrouching = true; 
            animator.SetBool("Crouched", true);
            animator.Update(0);
            boxCollider.size = crouchingColliderSize;
            boxCollider.offset = crouchingOffset;
            if (isGrounded)
            {
                 UpdateGunPosition(true);
            }
        }
        // Uncrouch when the player releases the down arrow
        else 
        {
            StartCoroutine(UncrouchWithDelay());
        }


        //shooting
        if (Input.GetKey(KeyCode.X))
        {
            blaster.TryShoot();
        }
    }



    private IEnumerator UncrouchWithDelay()
    {
        animator.SetBool("Crouched", false);
        yield return new WaitForSeconds(0.1f);
        animator.Update(0);
        isCrouching = false;
        boxCollider.size = standingColliderSize;
        boxCollider.offset = standingOffset;
        UpdateGunPosition(false); // Move gun back to standing position
    }

    //flip Spud left or right
    private void Flip(bool facingLeft)
    {
        if (facingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); // Rotate Spud left
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // Rotate Spud right
        }
    }

    private void UpdateGunPosition(bool isCrouching)
    {
        if (gunTransform != null)
        {
            Vector2 targetOffset = isCrouching ? gunOffsetCrouching : gunOffsetStanding;
            Vector3 targetPosition = new Vector3(targetOffset.x, targetOffset.y, gunTransform.localPosition.z);

            // Only update if the gun's position actually needs to change
            if (gunTransform.localPosition != targetPosition)
            {
                gunTransform.localPosition = targetPosition;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("Jumped", false);

            if (Input.GetKey(KeyCode.DownArrow))
            {
                isCrouching = true;
                animator.SetBool("Crouched", true); // Immediately switch to crouch animation
                animator.Play("Spud_Crouch"); // Directly force the animation
                boxCollider.size = crouchingColliderSize;
                boxCollider.offset = crouchingOffset;
                UpdateGunPosition(true);
            }
            else
            {
                animator.SetBool("Crouched", false); // Ensure crouch animation is off
            }
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
