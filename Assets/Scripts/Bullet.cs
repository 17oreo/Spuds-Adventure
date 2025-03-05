using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed;
    private Rigidbody2D rb;
    private Animator animator;
    private CaptainCarrotScript captainCarrotScript;
    private SpudScript spudScript;
    [SerializeField] private int bulletDamage;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        captainCarrotScript = FindAnyObjectByType<CaptainCarrotScript>();
        spudScript = FindAnyObjectByType<SpudScript>();
        animator = GetComponent<Animator>();
        SetDamage(bulletDamage);
    }   

    // Update is called once per frame
    void Update()
    {
        if (spudScript.gameEnd)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(float direction)
    {
        rb.linearVelocity = new Vector2(speed * direction, 0); // Use linearVelocity for Unity 6
        transform.rotation = Quaternion.Euler(0, direction == -1 ? 180 : 0, 0); // Flip bullet visually
    }
    public int SetDamage(int newDamage)
    {
        bulletDamage = newDamage;
        return bulletDamage;
    }
    public int GetDamage()
    {
        return bulletDamage;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            //do the animation then destroy the game object
            animator.SetBool("Exploded", true);
            Destroy(gameObject);
        }
    }

    //if user misses destroy bullet when it goes off screen
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
