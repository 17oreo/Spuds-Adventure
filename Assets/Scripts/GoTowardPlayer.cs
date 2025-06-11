using UnityEngine;

public class GoTowardPlayer : MonoBehaviour
{
    [SerializeField] public float speed = 7f;
    [SerializeField] private float homingDuration = .6f;

    private Rigidbody2D rb;

    private Transform player;

    private Vector2 moveDirection;    
    private bool lockedOn = false;
    private float homingTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }

        if (player != null)
        {
            moveDirection = (player.position - transform.position).normalized;
            rb.linearVelocity = moveDirection * speed;
        }
    }

    // Update is called once per frame
    void Update()
    {

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

}
