using System.Collections;
using UnityEngine;

public class FirePelt : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Rigidbody2D rb;
    private DonCalienteScript DCscript;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        DCscript = FindAnyObjectByType<DonCalienteScript>();
        rb.linearVelocity = transform.right * speed; // shoots in the direction it’s facing
    }

    void Update()
    {
        if (DCscript.destroyPelt)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Player"))
        {
            StartCoroutine(wait());
        }
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }
}
