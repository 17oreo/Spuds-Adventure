using Unity.VisualScripting;
using UnityEngine;

public class HotSauceProjectile : MonoBehaviour
{

    //references
    [SerializeField] private GameObject firePeltsPrefab;
    [SerializeField] public Transform[] peltSpawnPoints;
    private Animator animator;
    private Rigidbody2D rb;

    //time
    [SerializeField] private float explosionDelay = 1.5f;

    //bool
    private bool hasExploded = false;
    

    private DonCalienteScript DCscript;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(Explode), explosionDelay); //schedules the explode method to be called after explosionDelay seconds
        DCscript = FindAnyObjectByType<DonCalienteScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate bottle around Z axis
        if (hasExploded) return;
        {
            transform.Rotate(0, 0, 500 * Time.deltaTime); // Adjust speed as needed (500)
        }
        if (DCscript.destroyBottle)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }


    void Explode()
    {
        if (hasExploded)
        {
            return;
        }

        hasExploded = true;


        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;

        animator.SetBool("explosion", true);

    }

    public void SpawnFirePelts()
    {
        for (int i = 0; i < peltSpawnPoints.Length; i++)
        {
            float angleZ = 0f;

            switch (i)
            {
                case 0: angleZ = 135f; break; //up - left
                case 1: angleZ = 225f; break; //down - left
                case 2: angleZ = 315f; break; //down - right
                case 3: angleZ = 45f; break; //up - right
            }

            Instantiate(firePeltsPrefab, peltSpawnPoints[i].position, Quaternion.Euler(0, 0, angleZ));
        }


        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
