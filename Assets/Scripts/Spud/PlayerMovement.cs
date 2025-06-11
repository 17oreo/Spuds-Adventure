using System.Collections;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using TMPro; 


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 1000f; //dash strength
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private bool disableGravityDuringDash = true;
    [SerializeField] private bool enableIFrames = true; //toggle invincibility

    [Header("Super")]
    [SerializeField] public float superCooldown = 30f; //30 seconds
    [SerializeField] private float recoilForce = 10f;
    [SerializeField] private TextMeshProUGUI superCooldownText;
    public float superTimer = 30f;
    public bool canUseSuper = false;

    //Components
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private SpudScript spudScript;
    private BoxCollider2D boxCollider;
    private Transform gunTransform;
    private Blaster blaster;

    //Booleans
    private bool isGrounded;
    private bool isCrouching;
    private bool isOnPlatform = false;
    private bool isDashing = false;
    private bool canDash = true;
    public bool isInvincible;
    

    

    //Floats
    private float originalGravityScale;

    //Vector 2
    private Vector2 standingColliderSize;
    private Vector2 standingOffset;
    [SerializeField] private Vector2 gunOffsetStanding; // Adjust in Editor
    [SerializeField] private Vector2 gunOffsetCrouching; // Adjust in Editor
    [SerializeField] private Vector2 crouchingOffset;
    [SerializeField] private Vector2 crouchingColliderSize;
    
    //Colors
    private Color dashColor = new Color(134f / 255f, 134f / 255f, 134f / 255f);
    private Color originalColor;

    [SerializeField] private Transform tutorialSpawnPoint;

    private bool tutorialStarted = false;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spudScript = GetComponent<SpudScript>();
        boxCollider = GetComponent<BoxCollider2D>();

        //blaster
        blaster = GetComponent<Blaster>();
        gunTransform = transform.Find("Blaster");
        gunOffsetStanding = gunTransform.localPosition;
        blaster = gunTransform.GetComponent<Blaster>();

       // Save standing collider size from initial BoxCollider2D
        standingColliderSize = boxCollider.size;
        standingOffset = boxCollider.offset;

        //dash
        originalGravityScale = rb.gravityScale;
        
        //color
        originalColor = spriteRenderer.color;

        tutorialStarted = false;
    }

    // Update is called once per frame

    void Update()
    {
        //if not in tutorial, plauing or in seleciton map, movement is disabled
        if (GameManager.Instance.CurrentState != GameState.Tutorial && GameManager.Instance.CurrentState != GameState.Playing && GameManager.Instance.CurrentState != GameState.Selection) return;

        if (isDashing) return;

        //pauses movement if the game is over
        if (GameManager.Instance.CurrentState == GameState.Victory || GameManager.Instance.CurrentState == GameState.Paused)
        {
            rb.linearVelocity = Vector2.zero;
            //animator.SetFloat("Speed", 0);
            return;
        }

        //moving left or right
        float move = isCrouching ? 0 : Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        animator.SetFloat("Speed", Mathf.Abs(move));

        // Drop-through input: if crouched and Jump is pressed
        if (isCrouching && isOnPlatform && Input.GetButtonDown("Jump"))
        {
            StartCoroutine(DropThroughPlatform());
            return; // Skip regular jump
        }


        //jumping
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
        

        if (Input.GetKey(KeyCode.UpArrow))
        {
            bool isMoving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f;

            //if the user is moving the gun aims at a 45 deg angle
            //if the user is standing still the gun aims straight up
            if (isMoving)
                gunTransform.localRotation = Quaternion.Euler(0, 0, 45f); // Diagonal up
            else
                gunTransform.localRotation = Quaternion.Euler(0, 0, 90f); // Straight up
        }
        else
        {
            gunTransform.localRotation = Quaternion.Euler(0, 0, 0f); // Aim forward
        }

        //shooting
        if (Input.GetKey(KeyCode.X) && GameManager.Instance.CurrentState != GameState.Selection)
        {

            blaster.TryShoot();
        }

        //super
        if (Input.GetKeyDown(KeyCode.C) && canUseSuper && GameManager.Instance.CurrentState != GameState.Selection)
        {
            StartCoroutine(recoilRoutineAndSuper());

        }
        //super timer
        if (!canUseSuper)
        {
            superTimer -= Time.deltaTime;
            superTimer = Mathf.Max(superTimer, 0f);
            superCooldownText.text = $"Super ready in: {superTimer:F0}";

            if (superTimer <= 0f)
                canUseSuper = true;
        }
        else
        {
            superCooldownText.text = "Super Ready!";
        }

        // 🚀 Dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator recoilRoutineAndSuper()
    {
        canUseSuper = false;
        superTimer = superCooldown;
        isDashing = true;

        blaster.TrySuper();

        float direction = (transform.eulerAngles.y == 0) ? 1f : -1f;

        if (disableGravityDuringDash) rb.gravityScale = 0;

        rb.AddForce(new Vector2(-direction * recoilForce, 0), ForceMode2D.Impulse);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;
        if (disableGravityDuringDash) rb.gravityScale = originalGravityScale;

        isDashing = false;
    }


    private IEnumerator DropThroughPlatform()
    {
        Debug.Log("Dropping through platform...");

        // Temporarily disable collisions between Player and Platform layers
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Platform"),
            true
        );

        // Force Spud to fall through
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -10f);

        yield return new WaitForSeconds(0.5f);

        // Re-enable collisions
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Platform"),
            false
        );
    }



    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        bool wasCrouching = isCrouching;

        // Determine direction based on Spud's rotation
        float direction = (transform.eulerAngles.y == 0) ? 1f : -1f; 

        if (enableIFrames)
        {
            isInvincible = true;
        }

        // Disable gravity if needed
        if (disableGravityDuringDash) rb.gravityScale = 0;

        animator.SetBool("Dashed", true); //dash animation
        // Apply dash force
        rb.AddForce(new Vector2(direction * dashForce, 0), ForceMode2D.Impulse);
        

        //change color
        //spriteRenderer.color = dashColor;
        
        yield return new WaitForSeconds(dashDuration);

        //go back to original color after dash
        //spriteRenderer.color = originalColor;
        
        
        // Stop dash
        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        
        if (wasCrouching)
        {
            animator.SetBool("Dashed", false); //dash animation end
            animator.SetBool("Crouched", true);
            animator.Play("Spud_Crouch");
            animator.Update(0); //update animator immediately
        }
        else 
        {
            animator.SetBool("Dashed", false); //dash animation end
        }
        // Remove invincibility after a short delay (prevents instant damage after dash)
        if (enableIFrames)
        {
            yield return new WaitForSeconds(0.1f); // Extra safety buffer
            isInvincible = false;
        }
        

        // Restore gravity
        if (disableGravityDuringDash) rb.gravityScale = originalGravityScale;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
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

    public void StartTutorial()
    {
        transform.position = tutorialSpawnPoint.position;
        transform.rotation = Quaternion.Euler(0, 0, 0); // Rotate Spud right
        transform.Find("Blaster").gameObject.SetActive(true);
        tutorialStarted = true;
    }

   void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
            animator.SetBool("Jumped", false);

            isOnPlatform = collision.gameObject.CompareTag("Platform");

            if (Input.GetKey(KeyCode.DownArrow))
            {
                isCrouching = true;
                animator.SetBool("Crouched", true);
                animator.Play("Spud_Crouch");
                boxCollider.size = crouchingColliderSize;
                boxCollider.offset = crouchingOffset;
                UpdateGunPosition(true);
            }
            else
            {
                animator.SetBool("Crouched", false);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;
            if (collision.gameObject.CompareTag("Platform"))
            {
                isOnPlatform = false;
            }
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
