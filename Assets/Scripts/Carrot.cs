using UnityEngine;

public class Carrot : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D rb;
    private SpudScript spudScript;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; //no gravity
        spudScript = FindAnyObjectByType<SpudScript>();
    }


    void Update()
    {
        if (spudScript.gameEnd)
        {
            Destroy(gameObject);
        }
    }
    public void Launch()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.left * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //ignore attack is spud is dashing
            if (spudScript.isInvincible)
            {
                return;
            }
            Destroy(gameObject);
        }
    }

    //if the object goes off screen
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
