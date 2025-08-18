using UnityEngine;

public class WatermelonSeed : MonoBehaviour
{

    private Rigidbody2D rb;
    private SpudScript spudScript;
    private MadameMelon madameMelonScript;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spudScript = FindAnyObjectByType<SpudScript>();
        madameMelonScript = FindAnyObjectByType<MadameMelon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentState == GameState.GameOver)
        {
            Destroy(gameObject);
        }
        if (madameMelonScript.destroySeed)
        {
            Destroy(gameObject);
        }
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
}
