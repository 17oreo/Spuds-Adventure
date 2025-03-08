using System.Runtime.InteropServices;
using UnityEngine;

public class Carrot : MonoBehaviour
{
    [SerializeField] public float speed = 5f;
    private Rigidbody2D rb;
    private SpudScript spudScript;
    private CaptainCarrotScript captainCarrotScript;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; //no gravity
        spudScript = FindAnyObjectByType<SpudScript>();
        captainCarrotScript = FindAnyObjectByType<CaptainCarrotScript>();
    }


    void Update()
    {
        if (spudScript.gameEnd)
        {
            Destroy(gameObject);
        }
        if (captainCarrotScript.destroyCarrot)
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

    public void ShootUp()
    {
        rb.linearVelocity = Vector2.up * speed;
    }

    public void Drop()
    {
        rb.gravityScale = 1;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ground"))
        {
            //ignore attack is spud is dashing
            if (spudScript.isInvincible || spudScript.invincibleAfterDamage)
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
    
    public void DestroyCarrot()
    {
        Destroy(gameObject);
    }
}
