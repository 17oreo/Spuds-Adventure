using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed;
    private Rigidbody2D rb;
    private Animator animator;
    private CaptainCarrotScript captainCarrotScript;
    private SpudScript spudScript;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private int bulletDamage;
    private AudioSource explosionSound;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        captainCarrotScript = FindAnyObjectByType<CaptainCarrotScript>();
        spudScript = FindAnyObjectByType<SpudScript>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 10;

        explosionSound = GetComponent<AudioSource>();
        if (explosionSound == null)
        {
            Debug.Log("Audio Source Not Found");
        }

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
            rb.linearVelocity = Vector2.zero; //stop movement by setting velocity to zero
            rb.bodyType = RigidbodyType2D.Kinematic; // Prevents further physics interactions

            animator.SetBool("Exploded", true);
            explosionSound.PlayOneShot(explosionSound.clip);
        }
    }
    public void DestroyBullet()
    {
        
        Destroy(gameObject);
    }

    //if user misses destroy bullet when it goes off screen
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
