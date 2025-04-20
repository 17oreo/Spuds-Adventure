using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] public float speed = 5f;
    private Rigidbody2D rb;
    private SpudScript spudScript;
    private SgtSplatScript sgtSplatScript;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; //no gravity
        spudScript = FindAnyObjectByType<SpudScript>();
        sgtSplatScript = FindAnyObjectByType<SgtSplatScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sgtSplatScript.destroyKetchup)
        {
            Destroy(gameObject);
        }
    }

    public void Launch()
    {
        Debug.Log("Trying to Launch Laser");
        if (rb != null)
        {
            Debug.Log("Launching Laser");
            rb.linearVelocity = Vector2.left * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //ignore attack is spud is dashing
            if (spudScript.isInvincible || spudScript.invincibleAfterDamage)
            {
                return;
            }
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
