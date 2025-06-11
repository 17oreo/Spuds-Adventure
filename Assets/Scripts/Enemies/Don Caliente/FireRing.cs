using UnityEngine;

public class FireRing : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    [SerializeField] private float homingDuration = .4f;

    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private Transform player;
    private bool lockedOn = false;
    private float homingTimer;

    private DonCalienteScript DCscript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        DCscript = FindAnyObjectByType<DonCalienteScript>();
        homingTimer = homingDuration;


        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
        }

        if (player != null)
        {
            moveDirection = (player.position - transform.position).normalized;
            rb.linearVelocity = moveDirection * speed;
        }

    }

    void Update()
    {
        transform.Rotate(0, 0, 500 * Time.deltaTime);

        if (DCscript.destroyRing)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (lockedOn || player == null) return;

        homingTimer -= Time.fixedDeltaTime;

        if (homingTimer > 0f)
        {
            moveDirection = (player.position - transform.position).normalized;
            rb.linearVelocity = moveDirection * speed;
        }
        else
        {
            lockedOn = true;
            rb.linearVelocity = moveDirection * speed;
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //if Spud is not invincible then start the fog
            if (!collision.GetComponent<SpudScript>().isInvincible)
            {
                Destroy(gameObject);
            }
        }
    }
    
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
