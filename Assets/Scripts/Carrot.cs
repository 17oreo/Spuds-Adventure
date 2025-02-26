using UnityEngine;

public class Carrot : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.Log("Rigidbody not found on start");
        }
        rb.gravityScale = 0; //no gravity
    }

    public void launch()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.left * speed;
        }
        else 
        {
            Debug.Log("RigibBody2D is null when trying to launch");
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Spud Got Hit!");
            Destroy(gameObject);
        }
    }

    //if the object goes off screen
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
