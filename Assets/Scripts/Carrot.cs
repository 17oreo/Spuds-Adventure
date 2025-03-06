using System.Runtime.InteropServices;
using UnityEngine;

public class Carrot : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
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
            Destroy(gameObject);s
        }
    }
    public void Launch()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.left * speed;
        }
    }


    public void Drop()
    {
        rb.gravityScale = 2;
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
    
    public void DestroyCarrot()
    {
        Destroy(gameObject);
    }
}
